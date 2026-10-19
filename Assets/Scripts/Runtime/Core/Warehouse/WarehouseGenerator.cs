using System;
using System.Collections.Generic;
using CvWarehouse.Core.Cv;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class WarehouseGenerator
    {
        public const int WallHeight = 4;
        private const int ClusterAttempts = 24;
        private const int FreeAttempts = 400;
        private const int MaxGrowthSteps = 8;
        private const int Percent = 100;

        private readonly GeneratorSettings settings;
        private readonly Random random;
        private readonly WarehouseSizer sizer;
        private readonly List<GridPosition> largeOrigins = new List<GridPosition>();
        private readonly List<GridPosition> mediumOrigins = new List<GridPosition>();
        private WarehouseLayoutBuilder builder;
        private GridSize storage;

        public WarehouseGenerator(GeneratorSettings settings, Random random)
        {
            this.settings = settings;
            this.random = random;
            sizer = new WarehouseSizer(settings);
        }

        private int WallY => storage.Height + settings.RampDepth;

        private int RampY => WallY - 1;

        public bool TryGenerate(IReadOnlyList<CvBlock> blocks, out WarehouseLayout layout)
        {
            int boxCells = sizer.CountBoxCells(blocks);
            for (int growthSteps = 0; growthSteps <= MaxGrowthSteps; growthSteps++)
            {
                if (!TryPlaceEverything(blocks, sizer.StorageSizeFor(boxCells, growthSteps)))
                    continue;

                layout = builder.Build();
                return true;
            }

            layout = null;
            return false;
        }

        private bool TryPlaceEverything(IReadOnlyList<CvBlock> blocks, GridSize storageSize)
        {
            storage = storageSize;
            builder = new WarehouseLayoutBuilder(storage.Width, WallY + WallHeight);
            largeOrigins.Clear();
            mediumOrigins.Clear();
            AddTruckWall();
            AddRamp();
            return TryPlaceOpeningBoxes(blocks)
                && TryPlaceEntryBoxes(blocks, BoxSizeClass.Large)
                && TryPlaceEntryBoxes(blocks, BoxSizeClass.Medium)
                && TryPlaceEntryBoxes(blocks, BoxSizeClass.Small)
                && builder.CanEmptyEveryBox();
        }

        private void AddTruckWall()
        {
            int truckX = (storage.Width - settings.TruckWidth) / 2;
            int truckEndX = truckX + settings.TruckWidth;
            builder.Block(new GridPosition(0, WallY), new GridSize(truckX, WallHeight));
            builder.PlaceTruck(new GridPosition(truckX, WallY), new GridSize(settings.TruckWidth, WallHeight));
            builder.Block(new GridPosition(truckEndX, WallY), new GridSize(storage.Width - truckEndX, WallHeight));
        }

        private void AddRamp()
        {
            int firstDropX = (storage.Width - settings.DropCells) / 2;
            for (int drop = 0; drop < settings.DropCells; drop++)
                builder.AddDropCell(new GridPosition(firstDropX + drop, RampY));

            for (int parking = 0; parking < settings.ParkingPerSide; parking++)
            {
                builder.AddParkingCell(new GridPosition(parking, RampY));
                builder.AddParkingCell(new GridPosition(storage.Width - 1 - parking, RampY));
            }
        }

        private bool TryPlaceOpeningBoxes(IReadOnlyList<CvBlock> blocks)
        {
            foreach (CvBlock block in blocks)
            {
                if (block.Role == CvBlockRole.Entry)
                    continue;

                BoxKind kind = settings.KindOf(block);
                if (!TryFindSpotByTheRamp(kind.SizeClass, out GridPosition origin))
                    return false;

                builder.AddBox(origin, kind, block);
            }

            return true;
        }

        private bool TryPlaceEntryBoxes(IReadOnlyList<CvBlock> blocks, BoxSizeClass sizeClass)
        {
            foreach (CvBlock block in blocks)
                if (block.Role == CvBlockRole.Entry && settings.SizeClassOf(block) == sizeClass && !TryPlaceEntryBox(block))
                    return false;
            return true;
        }

        private bool TryPlaceEntryBox(CvBlock block)
        {
            BoxKind kind = settings.KindOf(block);
            if (!TryFindSpotInACluster(kind.SizeClass, out GridPosition origin) && !TryFindSpotAnywhere(kind.SizeClass, out origin))
                return false;

            builder.AddBox(origin, kind, block);
            if (kind.SizeClass == BoxSizeClass.Large)
                largeOrigins.Add(origin);
            else if (kind.SizeClass == BoxSizeClass.Medium)
                mediumOrigins.Add(origin);
            return true;
        }

        private bool TryFindSpotByTheRamp(BoxSizeClass sizeClass, out GridPosition origin)
        {
            int topRow = storage.Height - BoxFootprints.For(sizeClass).Height;
            for (int attempt = 0; attempt < FreeAttempts; attempt++)
            {
                int reach = settings.OpeningRadius + attempt / ClusterAttempts;
                origin = new GridPosition(storage.Width / 2 + random.Next(-reach, reach + 1), topRow - random.Next(reach));
                if (CanPlace(origin, sizeClass))
                    return true;
            }

            origin = default;
            return false;
        }

        private bool TryFindSpotInACluster(BoxSizeClass sizeClass, out GridPosition origin)
        {
            origin = default;
            if (!TryPickCluster(sizeClass, out GridPosition clusterOrigin, out int radius))
                return false;

            for (int attempt = 0; attempt < ClusterAttempts; attempt++)
            {
                origin = clusterOrigin.Offset(OffsetWithin(radius), OffsetWithin(radius));
                if (CanPlace(origin, sizeClass))
                    return true;
            }

            return false;
        }

        private bool TryPickCluster(BoxSizeClass sizeClass, out GridPosition clusterOrigin, out int radius)
        {
            clusterOrigin = default;
            radius = settings.BigClusterRadius;
            if (sizeClass == BoxSizeClass.Large)
                return false;

            int roll = random.Next(Percent);
            int nearLargePercent = sizeClass == BoxSizeClass.Medium ? settings.MediumNearLargePercent : settings.SmallNearLargePercent;
            if (largeOrigins.Count > 0 && roll < nearLargePercent)
            {
                clusterOrigin = PickFrom(largeOrigins);
                return true;
            }

            if (sizeClass != BoxSizeClass.Small || mediumOrigins.Count == 0 || roll >= nearLargePercent + settings.SmallNearMediumPercent)
                return false;

            clusterOrigin = PickFrom(mediumOrigins);
            radius = settings.MediumClusterRadius;
            return true;
        }

        private bool TryFindSpotAnywhere(BoxSizeClass sizeClass, out GridPosition origin)
        {
            for (int attempt = 0; attempt < FreeAttempts; attempt++)
            {
                origin = new GridPosition(random.Next(storage.Width), random.Next(storage.Height));
                if (CanPlace(origin, sizeClass))
                    return true;
            }

            origin = default;
            return false;
        }

        private bool CanPlace(GridPosition origin, BoxSizeClass sizeClass)
        {
            GridSize size = BoxFootprints.For(sizeClass);
            return origin.Y + size.Height <= storage.Height && builder.IsFree(origin, size);
        }

        private GridPosition PickFrom(List<GridPosition> origins)
        {
            return origins[random.Next(origins.Count)];
        }

        private int OffsetWithin(int radius)
        {
            return (random.Next(-radius, radius + 1) + random.Next(-radius, radius + 1)) / 2;
        }
    }
}
