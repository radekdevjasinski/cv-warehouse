using System;
using System.Collections.Generic;
using System.IO;
using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Tests.EditMode.Cv;
using NUnit.Framework;
using UnityEngine;
using Random = System.Random;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class WarehouseGeneratorTests
    {
        private const int Seed = 12345;
        private const int SeedsToCheck = 40;
        private const int WallHeight = 2;
        private const int OpeningBlocks = 4;

        private static readonly int[] EntriesPerWeight = { 1, 4, 12, 40 };

        [Test]
        public void Generate_Cv_PlacesOneBoxPerBlockHoldingItsLetters()
        {
            List<CvBlock> blocks = CvTestBlocks.BuildCv(3, 5, 7);

            WarehouseLayout layout = Generate(blocks, new GeneratorSettings(), Seed);

            Assert.AreEqual(blocks.Count, layout.Boxes.Count);
            int totalLetters = 0;
            foreach (CvBlock block in blocks)
            {
                Assert.AreEqual(block.LetterCount, BoxOf(layout, block).TotalPieces);
                totalLetters += block.LetterCount;
            }

            Assert.AreEqual(totalLetters, layout.TotalPieces);
        }

        [Test]
        public void Generate_Cv_GivesEveryBoxTheWeightOfItsBlock()
        {
            List<CvBlock> blocks = CvTestBlocks.BuildCv(3, 5, 7);
            var settings = new GeneratorSettings();

            WarehouseLayout layout = Generate(blocks, settings, Seed);

            Assert.AreEqual(WeightClass.Heavy, BoxOf(layout, blocks[0]).WeightClass);
            Assert.AreEqual(WeightClass.Medium, BoxOf(layout, blocks[1]).WeightClass);
            Assert.AreEqual(WeightClass.Light, BoxOf(layout, blocks[2]).WeightClass);
            for (int index = OpeningBlocks; index < blocks.Count; index++)
                Assert.AreEqual(blocks[index].Weight, BoxOf(layout, blocks[index]).WeightClass);
            Assert.AreEqual(settings.ParkingPerSide * 2, layout.ParkingCells.Count);
            Assert.AreEqual(settings.DropCells, layout.DropCells.Count);
        }

        [Test]
        public void Generate_HeaderWeightsChanged_SizesTheOpeningBoxesToMatch()
        {
            List<CvBlock> blocks = CvTestBlocks.BuildCv(1, 1, 1);
            var settings = new GeneratorSettings { NameWeight = WeightClass.Light, ContactWeight = WeightClass.Heavy };

            WarehouseLayout layout = Generate(blocks, settings, Seed);

            Assert.AreEqual(WeightClass.Light, BoxOf(layout, blocks[0]).WeightClass);
            Assert.AreEqual(WeightClass.Heavy, BoxOf(layout, blocks[2]).WeightClass);
        }

        [Test]
        public void Generate_ManySeeds_PutsTheOpeningBoxesNearerTheTruckThanTheAverageBox()
        {
            List<CvBlock> blocks = CvTestBlocks.BuildCv(6, 12, 30);
            for (int seed = 0; seed < SeedsToCheck; seed++)
            {
                WarehouseLayout layout = Generate(blocks, new GeneratorSettings(), seed);

                long openingDistance = 0;
                long entryDistance = 0;
                foreach (Box box in layout.Boxes)
                {
                    if (box.Block.Role == CvBlockRole.Entry)
                        entryDistance += DistanceToTheTruck(layout, box);
                    else
                        openingDistance += DistanceToTheTruck(layout, box);
                }

                Assert.Less(openingDistance / OpeningBlocks, entryDistance / (blocks.Count - OpeningBlocks), "Seed " + seed);
            }
        }

        [Test]
        public void Generate_BiggerCv_MakesABiggerMap()
        {
            WarehouseLayout small = Generate(CvTestBlocks.BuildCv(2, 4, 8), new GeneratorSettings(), Seed);
            WarehouseLayout big = Generate(CvTestBlocks.BuildCv(20, 40, 80), new GeneratorSettings(), Seed);

            Assert.Greater(big.Grid.Width * big.Grid.Height, small.Grid.Width * small.Grid.Height);
        }

        [Test]
        public void Generate_SameSeedAndCv_PlacesBoxesOnTheSameCells()
        {
            WarehouseLayout first = Generate(CvTestBlocks.BuildCv(3, 5, 7), new GeneratorSettings(), Seed);
            WarehouseLayout second = Generate(CvTestBlocks.BuildCv(3, 5, 7), new GeneratorSettings(), Seed);

            Assert.AreEqual(first.Grid.Width, second.Grid.Width);
            Assert.AreEqual(first.Grid.Height, second.Grid.Height);
            for (int box = 0; box < first.Boxes.Count; box++)
            {
                Assert.AreEqual(first.Boxes[box].Origin, second.Boxes[box].Origin);
                Assert.AreEqual(first.Boxes[box].Block.Id, second.Boxes[box].Block.Id);
            }
        }

        [Test]
        public void Generate_DifferentSeed_PlacesBoxesElsewhere()
        {
            List<CvBlock> blocks = CvTestBlocks.BuildCv(6, 12, 30);
            WarehouseLayout first = Generate(blocks, new GeneratorSettings(), Seed);
            WarehouseLayout second = Generate(blocks, new GeneratorSettings(), Seed + 1);

            int movedBoxes = 0;
            for (int box = 0; box < first.Boxes.Count; box++)
                if (first.Boxes[box].Origin != second.Boxes[box].Origin)
                    movedBoxes++;

            Assert.Greater(movedBoxes, first.Boxes.Count / 2);
        }

        [Test]
        public void Generate_ManySeedsAndCvSizes_KeepsBoxesOffTheRampAndApart()
        {
            var settings = new GeneratorSettings();
            foreach (int entries in EntriesPerWeight)
            {
                List<CvBlock> blocks = CvTestBlocks.BuildCv(entries, entries, entries);
                for (int seed = 0; seed < SeedsToCheck; seed++)
                {
                    WarehouseLayout layout = Generate(blocks, settings, seed);
                    int storageTop = layout.Grid.Height - WallHeight - settings.RampDepth;
                    var takenCells = new HashSet<GridPosition>();
                    foreach (Box box in layout.Boxes)
                    {
                        Assert.LessOrEqual(box.Origin.Y + box.Size.Height, storageTop, "Seed " + seed);
                        AssertFootprintIsNew(box, takenCells, seed);
                    }
                }
            }
        }

        [Test]
        public void Generate_ManySeedsAndCvSizes_EveryBoxOpensUpOnceItsNeighboursAreEmptied()
        {
            foreach (int entries in EntriesPerWeight)
            {
                List<CvBlock> blocks = CvTestBlocks.BuildCv(entries, entries, entries);
                for (int seed = 0; seed < SeedsToCheck; seed++)
                {
                    WarehouseLayout layout = Generate(blocks, DenseSettings(), seed);

                    Assert.AreEqual(blocks.Count, layout.Boxes.Count);
                    Assert.AreEqual(0, EmptyEveryReachableBox(layout), "Boxes left buried with seed " + seed + " and " + entries + " entries");
                }
            }
        }

        [Test]
        public void Generate_MapTooCrowdedAtFirst_GrowsUntilEveryBoxFits()
        {
            List<CvBlock> blocks = CvTestBlocks.BuildCv(40, 40, 40);
            var settings = new GeneratorSettings { FillPercent = 100, MinWidth = 1, MinStorageHeight = 1 };

            WarehouseLayout layout = Generate(blocks, settings, Seed);

            Assert.AreEqual(blocks.Count, layout.Boxes.Count);
        }

        [Test]
        public void Generate_TinyCv_StillHasRoomForTheRamp()
        {
            List<CvBlock> blocks = CvTestBlocks.Build(CvTestBlocks.NameAndProfileCv);
            var settings = new GeneratorSettings { MinWidth = 1 };

            WarehouseLayout layout = Generate(blocks, settings, Seed);

            Assert.AreEqual(2, layout.Boxes.Count);
            Assert.AreEqual(settings.ParkingPerSide * 2, layout.ParkingCells.Count);
            Assert.AreEqual(0, EmptyEveryReachableBox(layout));
        }

        [Test]
        public void Generate_SampleCvFile_PlacesABoxForEveryBlock()
        {
            string json = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "cv_en.json"));
            List<CvBlock> blocks = CvTestBlocks.Build(json);

            WarehouseLayout layout = Generate(blocks, new GeneratorSettings(), Seed);

            Assert.AreEqual(blocks.Count, layout.Boxes.Count);
            Assert.AreEqual(0, EmptyEveryReachableBox(layout));
        }

        private static GeneratorSettings DenseSettings()
        {
            return new GeneratorSettings
            {
                FillPercent = 35,
                MinWidth = 1,
                MinStorageHeight = 4,
                TruckWidth = 6,
                DropCells = 4,
                ParkingPerSide = 3,
                BigClusterRadius = 3,
                MediumClusterRadius = 2
            };
        }

        private static WarehouseLayout Generate(IReadOnlyList<CvBlock> blocks, GeneratorSettings settings, int seed)
        {
            return new WarehouseGenerator(settings, new Random(seed)).Generate(blocks);
        }

        private static Box BoxOf(WarehouseLayout layout, CvBlock block)
        {
            foreach (Box box in layout.Boxes)
                if (ReferenceEquals(box.Block, block))
                    return box;

            Assert.Fail("No box holds block " + block.Id);
            return null;
        }

        private static int DistanceToTheTruck(WarehouseLayout layout, Box box)
        {
            return layout.Grid.Height - box.Origin.Y + Math.Abs(layout.Grid.Width / 2 - box.Origin.X);
        }

        private static void AssertFootprintIsNew(Box box, HashSet<GridPosition> takenCells, int seed)
        {
            for (int y = 0; y < box.Size.Height; y++)
                for (int x = 0; x < box.Size.Width; x++)
                    Assert.IsTrue(takenCells.Add(box.Origin.Offset(x, y)), "Boxes overlap with seed " + seed);
        }

        private static int EmptyEveryReachableBox(WarehouseLayout layout)
        {
            var random = new Random(Seed);
            var letters = new List<int>();
            int boxesLeft = layout.Boxes.Count;
            bool emptiedAny = true;
            while (emptiedAny)
            {
                emptiedAny = false;
                foreach (Box box in layout.Boxes)
                {
                    if (box.IsEmpty || box.AccessCells.Count == 0)
                        continue;

                    box.TakeClaimed(box.Claim(CarrySize.Box, random, letters));
                    layout.RemoveEmptyBox(box);
                    boxesLeft--;
                    emptiedAny = true;
                }
            }

            return boxesLeft;
        }
    }
}
