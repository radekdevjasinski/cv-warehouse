using System;
using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class WarehouseGeneratorTests
    {
        private const int Seed = 12345;
        private const int SeedsToCheck = 40;

        [Test]
        public void Generate_DefaultSettings_PlacesEveryBoxAndBotSlot()
        {
            var settings = new GeneratorSettings();

            WarehouseLayout layout = Generate(settings, Seed);

            Assert.AreEqual(settings.HeavyBoxes + settings.MediumBoxes + settings.LightBoxes, layout.Boxes.Count);
            Assert.AreEqual(settings.HeavyBoxes, CountBoxes(layout, WeightClass.Heavy));
            Assert.AreEqual(settings.MediumBoxes, CountBoxes(layout, WeightClass.Medium));
            Assert.AreEqual(settings.ParkingPerSide * 2, layout.ParkingCells.Count);
            Assert.AreEqual(settings.DropCells, layout.DropCells.Count);
        }

        [Test]
        public void Generate_SameSeed_PlacesBoxesOnTheSameCells()
        {
            WarehouseLayout first = Generate(new GeneratorSettings(), Seed);
            WarehouseLayout second = Generate(new GeneratorSettings(), Seed);

            for (int box = 0; box < first.Boxes.Count; box++)
                Assert.AreEqual(first.Boxes[box].Origin, second.Boxes[box].Origin);
        }

        [Test]
        public void Generate_DifferentSeed_PlacesBoxesElsewhere()
        {
            WarehouseLayout first = Generate(new GeneratorSettings(), Seed);
            WarehouseLayout second = Generate(new GeneratorSettings(), Seed + 1);

            int movedBoxes = 0;
            for (int box = 0; box < first.Boxes.Count; box++)
                if (first.Boxes[box].Origin != second.Boxes[box].Origin)
                    movedBoxes++;

            Assert.Greater(movedBoxes, first.Boxes.Count / 2);
        }

        [Test]
        public void Generate_ManySeeds_KeepsBoxesOffTheRampAndApart()
        {
            var settings = new GeneratorSettings();
            int storageTop = settings.Height - 2 - settings.RampDepth;
            for (int seed = 0; seed < SeedsToCheck; seed++)
            {
                WarehouseLayout layout = Generate(settings, seed);
                var takenCells = new HashSet<GridPosition>();
                foreach (Box box in layout.Boxes)
                {
                    Assert.LessOrEqual(box.Origin.Y + box.Size.Height, storageTop, "Seed " + seed);
                    AssertFootprintIsNew(box, takenCells, seed);
                }
            }
        }

        [Test]
        public void Generate_ManySeeds_EveryBoxOpensUpOnceItsNeighboursAreEmptied()
        {
            for (int seed = 0; seed < SeedsToCheck; seed++)
            {
                WarehouseLayout layout = Generate(DenseSettings(), seed);

                Assert.AreEqual(0, EmptyEveryReachableBox(layout), "Boxes left buried with seed " + seed);
            }
        }

        [Test]
        public void Generate_TooManyBoxesForTheMap_Throws()
        {
            var settings = new GeneratorSettings { Width = 24, Height = 12, HeavyBoxes = 200 };

            Assert.Throws<InvalidOperationException>(() => Generate(settings, Seed));
        }

        [Test]
        public void Generate_MapNarrowerThanTheRamp_Throws()
        {
            var settings = new GeneratorSettings { Width = 12 };

            Assert.Throws<ArgumentException>(() => Generate(settings, Seed));
        }

        private static GeneratorSettings DenseSettings()
        {
            return new GeneratorSettings
            {
                Width = 30,
                Height = 20,
                TruckWidth = 6,
                DropCells = 4,
                ParkingPerSide = 3,
                HeavyBoxes = 6,
                MediumBoxes = 20,
                LightBoxes = 70,
                BigClusterRadius = 3,
                MediumClusterRadius = 2
            };
        }

        private static WarehouseLayout Generate(GeneratorSettings settings, int seed)
        {
            return new WarehouseGenerator(settings, new Random(seed)).Generate();
        }

        private static int CountBoxes(WarehouseLayout layout, WeightClass weightClass)
        {
            int count = 0;
            foreach (Box box in layout.Boxes)
                if (box.WeightClass == weightClass)
                    count++;
            return count;
        }

        private static void AssertFootprintIsNew(Box box, HashSet<GridPosition> takenCells, int seed)
        {
            for (int y = 0; y < box.Size.Height; y++)
                for (int x = 0; x < box.Size.Width; x++)
                    Assert.IsTrue(takenCells.Add(box.Origin.Offset(x, y)), "Boxes overlap with seed " + seed);
        }

        private static int EmptyEveryReachableBox(WarehouseLayout layout)
        {
            int boxesLeft = layout.Boxes.Count;
            bool emptiedAny = true;
            while (emptiedAny)
            {
                emptiedAny = false;
                foreach (Box box in layout.Boxes)
                {
                    if (box.IsEmpty || box.AccessCells.Count == 0)
                        continue;

                    box.TakeClaimed(box.Claim(box.RemainingPieces));
                    layout.RemoveEmptyBox(box);
                    boxesLeft--;
                    emptiedAny = true;
                }
            }

            return boxesLeft;
        }
    }
}
