using System;
using System.Collections.Generic;
using CvWarehouse.Core.Cv;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class WarehouseGenerator
    {
        private const int WallHeight = 2;
        private const int ClusterAttempts = 24;
        private const int FreeAttempts = 400;
        private const int MaxGrowthSteps = 8;
        private const int Percent = 100;

        private readonly GeneratorSettings settings;
        private readonly Random random;
        private readonly WarehouseSizer sizer;
        private readonly List<GridPosition> heavyOrigins = new List<GridPosition>();
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

        public WarehouseLayout Generate(IReadOnlyList<CvBlock> blocks)
        {
            int boxCells = sizer.CountBoxCells(blocks);
            for (int growthSteps = 0; growthSteps <= MaxGrowthSteps; growthSteps++)
                if (TryGenerate(blocks, sizer.StorageSizeFor(boxCells, growthSteps)))
                    return builder.Build();

            throw new InvalidOperationException("The warehouse could not fit its boxes.");
        }

        private bool TryGenerate(IReadOnlyList<CvBlock> blocks, GridSize storageSize)
        {
            storage = storageSize;
            builder = new WarehouseLayoutBuilder(storage.Width, WallY + WallHeight);
            heavyOrigins.Clear();
            mediumOrigins.Clear();
            AddTruckWall();
            AddRamp();
            return TryPlaceOpeningBoxes(blocks)
                && TryPlaceEntryBoxes(blocks, WeightClass.Heavy)
                && TryPlaceEntryBoxes(blocks, WeightClass.Medium)
                && TryPlaceEntryBoxes(blocks, WeightClass.Light);
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

                WeightClass weightClass = settings.WeightOf(block);
                if (!TryFindSpotByTheRamp(weightClass, out GridPosition origin))
                    return false;

                builder.AddBox(origin, weightClass, block);
            }

            return true;
        }

        private bool TryPlaceEntryBoxes(IReadOnlyList<CvBlock> blocks, WeightClass weightClass)
        {
            foreach (CvBlock block in blocks)
                if (block.Role == CvBlockRole.Entry && block.Weight == weightClass && !TryPlaceEntryBox(block))
                    return false;
            return true;
        }

        private bool TryPlaceEntryBox(CvBlock block)
        {
            if (!TryFindSpotInACluster(block.Weight, out GridPosition origin) && !TryFindSpotAnywhere(block.Weight, out origin))
                return false;

            builder.AddBox(origin, block.Weight, block);
            if (block.Weight == WeightClass.Heavy)
                heavyOrigins.Add(origin);
            else if (block.Weight == WeightClass.Medium)
                mediumOrigins.Add(origin);
            return true;
        }

        private bool TryFindSpotByTheRamp(WeightClass weightClass, out GridPosition origin)
        {
            int topRow = storage.Height - BoxFootprints.For(weightClass).Height;
            for (int attempt = 0; attempt < FreeAttempts; attempt++)
            {
                int reach = settings.OpeningRadius + attempt / ClusterAttempts;
                origin = new GridPosition(storage.Width / 2 + random.Next(-reach, reach + 1), topRow - random.Next(reach));
                if (CanPlace(origin, weightClass))
                    return true;
            }

            origin = default;
            return false;
        }

        private bool TryFindSpotInACluster(WeightClass weightClass, out GridPosition origin)
        {
            origin = default;
            if (!TryPickCluster(weightClass, out GridPosition clusterOrigin, out int radius))
                return false;

            for (int attempt = 0; attempt < ClusterAttempts; attempt++)
            {
                origin = clusterOrigin.Offset(OffsetWithin(radius), OffsetWithin(radius));
                if (CanPlace(origin, weightClass))
                    return true;
            }

            return false;
        }

        private bool TryPickCluster(WeightClass weightClass, out GridPosition clusterOrigin, out int radius)
        {
            clusterOrigin = default;
            radius = settings.BigClusterRadius;
            if (weightClass == WeightClass.Heavy)
                return false;

            int roll = random.Next(Percent);
            int nearHeavyPercent = weightClass == WeightClass.Medium ? settings.MediumNearHeavyPercent : settings.LightNearHeavyPercent;
            if (heavyOrigins.Count > 0 && roll < nearHeavyPercent)
            {
                clusterOrigin = PickFrom(heavyOrigins);
                return true;
            }

            if (weightClass != WeightClass.Light || mediumOrigins.Count == 0 || roll >= nearHeavyPercent + settings.LightNearMediumPercent)
                return false;

            clusterOrigin = PickFrom(mediumOrigins);
            radius = settings.MediumClusterRadius;
            return true;
        }

        private bool TryFindSpotAnywhere(WeightClass weightClass, out GridPosition origin)
        {
            for (int attempt = 0; attempt < FreeAttempts; attempt++)
            {
                origin = new GridPosition(random.Next(storage.Width), random.Next(storage.Height));
                if (CanPlace(origin, weightClass))
                    return true;
            }

            origin = default;
            return false;
        }

        private bool CanPlace(GridPosition origin, WeightClass weightClass)
        {
            GridSize size = BoxFootprints.For(weightClass);
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
