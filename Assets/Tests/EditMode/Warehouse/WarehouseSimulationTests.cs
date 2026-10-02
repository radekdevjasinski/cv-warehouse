using System;
using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class WarehouseSimulationTests
    {
        private const int Seed = 12345;
        private const int MaxTicks = 400000;
        private const int SmallGridSide = 6;
        private const int SmallBoxPieces = 3;
        private const int TicksToPark = 3000;
        private const int EveryBotSlot = int.MaxValue;
        private const float TestTickSeconds = 0.1f;

        [Test]
        public void Tick_SingleBot_DeliversEveryPieceOfTheBox()
        {
            WarehouseSimulation simulation = NewSmallSimulation(new SimulationSettings());
            simulation.TryAddBot();

            RunUntilComplete(simulation);

            Assert.IsTrue(simulation.IsComplete);
            Assert.IsTrue(simulation.Layout.Boxes[0].IsEmpty);
            Assert.AreEqual(SmallBoxPieces * 2, simulation.Score.Points);
        }

        [Test]
        public void Tick_BoxEmptied_OpensItsCellsForBots()
        {
            WarehouseSimulation simulation = NewSmallSimulation(new SimulationSettings());
            simulation.TryAddBot();

            RunUntilComplete(simulation);

            Assert.IsTrue(simulation.Layout.Grid.IsWalkable(simulation.Layout.Boxes[0].Origin));
        }

        [Test]
        public void Tick_BotCarryingTwoPieces_DeliversTwoOnTheFirstTrip()
        {
            WarehouseSimulation simulation = NewSmallSimulation(new SimulationSettings { CarryPieces = 2 });
            simulation.TryAddBot();

            for (int tick = 0; tick < MaxTicks && simulation.Score.DeliveredPieces == 0; tick++)
                simulation.Tick();

            Assert.AreEqual(2, simulation.Score.DeliveredPieces);
        }

        [Test]
        public void Tick_FirstJob_GoesToABoxNearestTheTruck()
        {
            WarehouseSimulation simulation = NewGeneratedSimulation(1);

            simulation.Tick();

            Box firstBox = simulation.Bots[0].JobBox;
            Assert.IsNotNull(firstBox);
            foreach (Box box in simulation.Layout.Boxes)
                Assert.LessOrEqual(firstBox.TruckDistance, box.TruckDistance);
        }

        [Test]
        public void Tick_EveryBotSlotUsed_NeverPutsTwoBotsOnOneCellOrInsideABox()
        {
            WarehouseSimulation simulation = NewGeneratedSimulation(EveryBotSlot);
            int expectedPoints = PointsOfEveryPiece(simulation.Layout);
            var takenCells = new HashSet<GridPosition>();

            for (int tick = 0; tick < MaxTicks && !simulation.IsComplete; tick++)
            {
                simulation.Tick();
                AssertNoSharedCells(simulation, takenCells);
            }

            Assert.IsTrue(simulation.IsComplete, "The bots got stuck before emptying the warehouse.");
            Assert.AreEqual(expectedPoints, simulation.Score.Points);
        }

        [Test]
        public void Tick_AfterTheLastDelivery_EveryBotParks()
        {
            WarehouseSimulation simulation = NewGeneratedSimulation(EveryBotSlot);
            RunUntilComplete(simulation);

            for (int tick = 0; tick < TicksToPark; tick++)
                simulation.Tick();

            foreach (Bot bot in simulation.Bots)
            {
                Assert.AreEqual(BotState.Parked, bot.State);
                Assert.AreEqual(CellType.Parking, simulation.Layout.Grid.GetCellType(bot.Cell));
            }
        }

        [Test]
        public void Tick_SameSeed_FinishesOnTheSameTick()
        {
            WarehouseSimulation firstRun = NewGeneratedSimulation(EveryBotSlot);
            WarehouseSimulation secondRun = NewGeneratedSimulation(EveryBotSlot);

            RunUntilComplete(firstRun);
            RunUntilComplete(secondRun);

            Assert.AreEqual(firstRun.TickCount, secondRun.TickCount);
        }

        [Test]
        public void TryAddBot_EveryParkingCellTaken_ReturnsFalse()
        {
            WarehouseSimulation simulation = NewGeneratedSimulation(EveryBotSlot);

            Assert.AreEqual(simulation.MaxBots, simulation.Bots.Count);
            Assert.IsFalse(simulation.TryAddBot());
        }

        [Test]
        public void TryRemoveBot_WhileBotsAreWorking_StillDeliversEveryPiece()
        {
            WarehouseSimulation simulation = NewGeneratedSimulation(EveryBotSlot);
            int botsBefore = simulation.Bots.Count;
            for (int removal = 0; removal < 4; removal++)
            {
                for (int tick = 0; tick < 150; tick++)
                    simulation.Tick();
                Assert.IsTrue(simulation.TryRemoveBot());
            }

            RunUntilComplete(simulation);

            Assert.IsTrue(simulation.IsComplete);
            Assert.AreEqual(botsBefore - 4, simulation.Bots.Count);
        }

        [Test]
        public void TryRemoveBot_NoBots_ReturnsFalse()
        {
            Assert.IsFalse(NewGeneratedSimulation(0).TryRemoveBot());
        }

        [Test]
        public void Advance_DeltaTime_RunsWholeTicksAndKeepsTheRemainder()
        {
            WarehouseSimulation simulation = NewSmallSimulation(new SimulationSettings { TickSeconds = TestTickSeconds });

            simulation.Advance(0.25f);
            Assert.AreEqual(2, simulation.TickCount);

            simulation.Advance(0.06f);
            Assert.AreEqual(3, simulation.TickCount);
        }

        [Test]
        public void Advance_HugeDeltaTime_RunsAtMostTheTickLimit()
        {
            var settings = new SimulationSettings { TickSeconds = TestTickSeconds, MaxTicksPerAdvance = 5 };
            WarehouseSimulation simulation = NewSmallSimulation(settings);

            simulation.Advance(100f);
            simulation.Advance(0f);

            Assert.AreEqual(5, simulation.TickCount);
        }

        private static WarehouseSimulation NewSmallSimulation(SimulationSettings settings)
        {
            WarehouseLayout layout = new WarehouseLayoutBuilder(SmallGridSide, SmallGridSide)
                .AddDropCell(new GridPosition(0, 5))
                .AddParkingCell(new GridPosition(5, 5))
                .AddBox(new GridPosition(3, 2), WeightClass.Medium, SmallBoxPieces)
                .Build();
            return new WarehouseSimulation(layout, settings, new Random(Seed));
        }

        private static WarehouseSimulation NewGeneratedSimulation(int botCount)
        {
            var generatorSettings = new GeneratorSettings
            {
                Width = 36,
                Height = 26,
                TruckWidth = 6,
                DropCells = 4,
                ParkingPerSide = 4,
                HeavyBoxes = 5,
                MediumBoxes = 14,
                LightBoxes = 45,
                BigClusterRadius = 4
            };
            var random = new Random(Seed);
            WarehouseLayout layout = new WarehouseGenerator(generatorSettings, random).Generate();
            var simulation = new WarehouseSimulation(layout, new SimulationSettings(), random);
            while (simulation.Bots.Count < botCount && simulation.TryAddBot())
            {
            }

            return simulation;
        }

        private static void RunUntilComplete(WarehouseSimulation simulation)
        {
            for (int tick = 0; tick < MaxTicks && !simulation.IsComplete; tick++)
                simulation.Tick();
        }

        private static int PointsOfEveryPiece(WarehouseLayout layout)
        {
            var settings = new SimulationSettings();
            int points = 0;
            foreach (Box box in layout.Boxes)
                points += box.TotalPieces * settings.PointsPerPiece(box.WeightClass);
            return points;
        }

        private static void AssertNoSharedCells(WarehouseSimulation simulation, HashSet<GridPosition> takenCells)
        {
            takenCells.Clear();
            foreach (Bot bot in simulation.Bots)
            {
                Assert.IsTrue(simulation.Layout.Grid.IsWalkable(bot.Cell), "Bot inside a box at " + bot.Cell);
                Assert.IsTrue(takenCells.Add(bot.Cell), "Two bots on " + bot.Cell + " at tick " + simulation.TickCount);
                if (bot.IsMoving)
                    Assert.IsTrue(takenCells.Add(bot.NextCell), "Two bots on " + bot.NextCell + " at tick " + simulation.TickCount);
            }
        }
    }
}
