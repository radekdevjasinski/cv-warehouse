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
        private const int Percent = 100;

        private readonly GeneratorSettings settings;
        private readonly Random random;
        private readonly List<GridPosition> heavyOrigins = new List<GridPosition>();
        private readonly List<GridPosition> mediumOrigins = new List<GridPosition>();
        private WarehouseLayoutBuilder builder;

        public WarehouseGenerator(GeneratorSettings settings, Random random)
        {
            this.settings = settings;
            this.random = random;
        }

        private int WallY => settings.Height - WallHeight;

        private int RampY => WallY - 1;

        private int StorageHeight => WallY - settings.RampDepth;

        public WarehouseLayout Generate()
        {
            RequireRoomForTheRamp();
            builder = new WarehouseLayoutBuilder(settings.Width, settings.Height);
            heavyOrigins.Clear();
            mediumOrigins.Clear();
            AddTruckWall();
            AddRamp();
            PlaceHeavyBoxes();
            PlaceMediumBoxes();
            PlaceLightBoxes();
            return builder.Build();
        }

        private void RequireRoomForTheRamp()
        {
            int rampWidth = Math.Max(settings.TruckWidth, settings.DropCells) + settings.ParkingPerSide * 2;
            if (settings.Width < rampWidth || StorageHeight <= 0)
                throw new ArgumentException("The warehouse is too small for the truck, the drop cells and the parking cells.");
        }

        private void AddTruckWall()
        {
            int truckX = (settings.Width - settings.TruckWidth) / 2;
            int truckEndX = truckX + settings.TruckWidth;
            builder.Block(new GridPosition(0, WallY), new GridSize(truckX, WallHeight));
            builder.PlaceTruck(new GridPosition(truckX, WallY), new GridSize(settings.TruckWidth, WallHeight));
            builder.Block(new GridPosition(truckEndX, WallY), new GridSize(settings.Width - truckEndX, WallHeight));
        }

        private void AddRamp()
        {
            int firstDropX = (settings.Width - settings.DropCells) / 2;
            for (int drop = 0; drop < settings.DropCells; drop++)
                builder.AddDropCell(new GridPosition(firstDropX + drop, RampY));

            for (int parking = 0; parking < settings.ParkingPerSide; parking++)
            {
                builder.AddParkingCell(new GridPosition(parking, RampY));
                builder.AddParkingCell(new GridPosition(settings.Width - 1 - parking, RampY));
            }
        }

        private void PlaceHeavyBoxes()
        {
            for (int box = 0; box < settings.HeavyBoxes; box++)
                heavyOrigins.Add(PlaceAnywhere(WeightClass.Heavy, settings.HeavyPieces));
        }

        private void PlaceMediumBoxes()
        {
            for (int box = 0; box < settings.MediumBoxes; box++)
            {
                bool joinsHeavy = heavyOrigins.Count > 0 && random.Next(Percent) < settings.MediumNearHeavyPercent;
                mediumOrigins.Add(joinsHeavy
                    ? PlaceNear(PickFrom(heavyOrigins), settings.BigClusterRadius, WeightClass.Medium)
                    : PlaceAnywhere(WeightClass.Medium, settings.MediumPieces));
            }
        }

        private void PlaceLightBoxes()
        {
            for (int box = 0; box < settings.LightBoxes; box++)
            {
                int roll = random.Next(Percent);
                if (heavyOrigins.Count > 0 && roll < settings.LightNearHeavyPercent)
                    PlaceNear(PickFrom(heavyOrigins), settings.BigClusterRadius, WeightClass.Light);
                else if (mediumOrigins.Count > 0 && roll < settings.LightNearHeavyPercent + settings.LightNearMediumPercent)
                    PlaceNear(PickFrom(mediumOrigins), settings.MediumClusterRadius, WeightClass.Light);
                else
                    PlaceAnywhere(WeightClass.Light, settings.LightPieces);
            }
        }

        private GridPosition PickFrom(List<GridPosition> origins)
        {
            return origins[random.Next(origins.Count)];
        }

        private GridPosition PlaceNear(GridPosition clusterOrigin, int radius, WeightClass weightClass)
        {
            int pieces = PiecesOf(weightClass);
            for (int attempt = 0; attempt < ClusterAttempts; attempt++)
            {
                GridPosition origin = clusterOrigin.Offset(OffsetWithin(radius), OffsetWithin(radius));
                if (TryPlace(origin, weightClass, pieces))
                    return origin;
            }

            return PlaceAnywhere(weightClass, pieces);
        }

        private GridPosition PlaceAnywhere(WeightClass weightClass, int pieces)
        {
            for (int attempt = 0; attempt < FreeAttempts; attempt++)
            {
                var origin = new GridPosition(random.Next(settings.Width), random.Next(StorageHeight));
                if (TryPlace(origin, weightClass, pieces))
                    return origin;
            }

            throw new InvalidOperationException("The warehouse is too small for its boxes.");
        }

        private bool TryPlace(GridPosition origin, WeightClass weightClass, int pieces)
        {
            GridSize size = BoxFootprints.For(weightClass);
            if (origin.Y + size.Height > StorageHeight || !builder.IsFree(origin, size))
                return false;

            builder.AddBox(origin, weightClass, pieces);
            return true;
        }

        private int OffsetWithin(int radius)
        {
            return (random.Next(-radius, radius + 1) + random.Next(-radius, radius + 1)) / 2;
        }

        private int PiecesOf(WeightClass weightClass)
        {
            return weightClass == WeightClass.Medium ? settings.MediumPieces : settings.LightPieces;
        }
    }
}
