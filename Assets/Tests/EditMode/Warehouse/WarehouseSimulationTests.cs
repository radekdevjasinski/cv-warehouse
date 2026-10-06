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
        public void Tick_BotCarryingBoxes_EmptiesTheBoxInOneTrip()
        {
            WarehouseSimulation simulation = NewSmallSimulation(new SimulationSettings());
            simulation.TryAddBot();
            simulation.SetCarrySize(simulation.Bots[0], CarrySize.Box);

            for (int tick = 0; tick < MaxTicks && simulation.Score.DeliveredPieces == 0; tick++)
                simulation.Tick();

            Assert.AreEqual(SmallBoxPieces, simulation.Score.DeliveredPieces);
            Assert.IsTrue(simulation.IsComplete);
        }

        [Test]
        public void Tick_BotCarryingWords_DeliversAWordOnTheFirstTrip()
        {
            WarehouseSimulation simulation = NewSmallSimulation(new SimulationSettings { WordPieces = 2 });
            simulation.TryAddBot();
            simulation.SetCarrySize(simulation.Bots[0], CarrySize.Word);

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
        public void Tick_EveryBotSlotUsed_NeverStopsTwoBotsOnOneCellOrPutsABotInsideABox()
        {
            WarehouseSimulation simulation = NewGeneratedSimulation(EveryBotSlot);
            int expectedPoints = PointsOfEveryPiece(simulation.Layout);
            var standingCells = new HashSet<GridPosition>();

            for (int tick = 0; tick < MaxTicks && !simulation.IsComplete; tick++)
            {
                simulation.Tick();
                AssertNoSharedStandingCells(simulation, standingCells);
            }

            Assert.IsTrue(simulation.IsComplete, "The bots got stuck before emptying the warehouse.");
            Assert.AreEqual(expectedPoints, simulation.Score.Points);
            Assert.AreEqual(expectedPoints, simulation.Score.MaxPoints);
        }

        [Test]
        public void Tick_EveryBotSlotUsed_NoBotEverWaitsOnItsWay()
        {
            WarehouseSimulation simulation = NewGeneratedSimulation(EveryBotSlot);

            for (int tick = 0; tick < MaxTicks && !simulation.IsComplete; tick++)
            {
                simulation.Tick();
                foreach (Bot bot in simulation.Bots)
                    Assert.IsTrue(bot.IsMoving || bot.IsAtDestination, "Bot " + bot.Id + " waited at " + bot.Cell + " on tick " + simulation.TickCount);
            }
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
                Assert.IsTrue(simulation.TryRemoveBot(simulation.Bots[removal]));
            }

            RunUntilComplete(simulation);

            Assert.IsTrue(simulation.IsComplete);
            Assert.AreEqual(botsBefore - 4, simulation.Bots.Count);
        }

        [Test]
        public void TryRemoveBot_BotInTheMiddleOfTheCrew_RemovesOnlyThatBot()
        {
            WarehouseSimulation simulation = NewGeneratedSimulation(3);
            Bot firstBot = simulation.Bots[0];
            Bot middleBot = simulation.Bots[1];
            Bot lastBot = simulation.Bots[2];

            Assert.IsTrue(simulation.TryRemoveBot(middleBot));

            Assert.AreEqual(2, simulation.Bots.Count);
            Assert.AreSame(firstBot, simulation.Bots[0]);
            Assert.AreSame(lastBot, simulation.Bots[1]);
        }

        [Test]
        public void TryRemoveBot_BotAlreadyRemoved_ReturnsFalse()
        {
            WarehouseSimulation simulation = NewGeneratedSimulation(3);
            Bot removedBot = simulation.Bots[1];
            simulation.TryRemoveBot(removedBot);

            Assert.IsFalse(simulation.TryRemoveBot(removedBot));
            Assert.AreEqual(2, simulation.Bots.Count);
        }

        [Test]
        public void TryRemoveBot_LastBot_KeepsItSoTheGameCanStillBeFinished()
        {
            WarehouseSimulation simulation = NewGeneratedSimulation(1);

            Assert.IsFalse(simulation.CanRemoveBot);
            Assert.IsFalse(simulation.TryRemoveBot(simulation.Bots[0]));
            Assert.AreEqual(1, simulation.Bots.Count);
        }

        [Test]
        public void Tick_BotsWithDifferentCarrySizes_EachTakesItsOwnAmount()
        {
            WarehouseSimulation simulation = NewGeneratedSimulation(2);
            simulation.SetCarrySize(simulation.Bots[1], CarrySize.Box);

            simulation.Tick();

            Bot letterBot = simulation.Bots[0];
            Bot boxBot = simulation.Bots[1];
            Assert.AreEqual(1, letterBot.ClaimedPieces);
            Assert.Greater(boxBot.ClaimedPieces, 0);
            Assert.AreEqual(0, boxBot.JobBox.UnclaimedPieces);
        }

        [Test]
        public void RouteLength_BotSentToABox_LeadsStepByStepToItsDestination()
        {
            WarehouseSimulation simulation = NewGeneratedSimulation(1);
            Bot bot = simulation.Bots[0];

            simulation.Tick();

            Assert.IsTrue(bot.IsMoving);
            GridPosition previous = bot.NextCell;
            for (int index = 0; index < bot.RouteLength; index++)
            {
                GridPosition cell = bot.RouteCell(index);
                Assert.LessOrEqual(Math.Abs(cell.X - previous.X), 1);
                Assert.LessOrEqual(Math.Abs(cell.Y - previous.Y), 1);
                previous = cell;
            }

            Assert.AreEqual(bot.Destination, previous);
        }

        [Test]
        public void RouteLength_BotStandingAtItsDestination_IsZero()
        {
            WarehouseSimulation simulation = NewGeneratedSimulation(1);
            Bot bot = simulation.Bots[0];

            Assert.AreEqual(0, bot.RouteLength);

            for (int tick = 0; tick < MaxTicks && bot.State != BotState.Extracting; tick++)
                simulation.Tick();

            Assert.AreEqual(BotState.Extracting, bot.State);
            Assert.AreEqual(0, bot.RouteLength);
        }

        [Test]
        public void CentreX_BotHalfwayThroughAStep_LiesBetweenTheTwoCells()
        {
            WarehouseSimulation simulation = NewGeneratedSimulation(1);
            Bot bot = simulation.Bots[0];
            simulation.Tick();

            while (bot.MoveTicks * 2 < bot.MoveDuration)
                simulation.Tick();

            float expectedX = (bot.Cell.X + bot.NextCell.X) * 0.5f + 0.5f;
            float expectedY = (bot.Cell.Y + bot.NextCell.Y) * 0.5f + 0.5f;
            Assert.AreEqual(expectedX, bot.CentreX, 0.2f);
            Assert.AreEqual(expectedY, bot.CentreY, 0.2f);
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

        [Test]
        public void Advance_Paused_RunsNoTicks()
        {
            WarehouseSimulation simulation = NewSmallSimulation(new SimulationSettings { TickSeconds = TestTickSeconds });
            simulation.TogglePause();

            simulation.Advance(0.35f);

            Assert.IsTrue(simulation.IsPaused);
            Assert.AreEqual(0, simulation.TickCount);
        }

        [Test]
        public void Advance_ResumedAfterAPause_DoesNotCatchUpOnThePausedTime()
        {
            WarehouseSimulation simulation = NewSmallSimulation(new SimulationSettings { TickSeconds = TestTickSeconds });
            simulation.TogglePause();
            simulation.Advance(0.35f);

            simulation.TogglePause();
            simulation.Advance(0.25f);

            Assert.IsFalse(simulation.IsPaused);
            Assert.AreEqual(2, simulation.TickCount);
        }

        [Test]
        public void SetCarrySize_WhilePaused_StillApplies()
        {
            WarehouseSimulation simulation = NewSmallSimulation(new SimulationSettings());
            simulation.TryAddBot();
            simulation.TogglePause();

            simulation.SetCarrySize(simulation.Bots[0], CarrySize.Word);
            simulation.SetFocus(simulation.Bots[0], BotFocus.BiggestBox);

            Assert.AreEqual(CarrySize.Word, simulation.Bots[0].CarrySize);
            Assert.AreEqual(BotFocus.BiggestBox, simulation.Bots[0].Focus);
        }

        private static WarehouseSimulation NewSmallSimulation(SimulationSettings settings)
        {
            WarehouseLayout layout = new WarehouseLayoutBuilder(SmallGridSide, SmallGridSide)
                .AddDropCell(new GridPosition(0, 5))
                .AddParkingCell(new GridPosition(5, 5))
                .AddBox(new GridPosition(3, 2), WeightClass.Medium, SmallBoxPieces)
                .Build();
            return new WarehouseSimulation(layout, settings);
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
            WarehouseLayout layout = new WarehouseGenerator(generatorSettings, new Random(Seed)).Generate();
            var simulation = new WarehouseSimulation(layout, new SimulationSettings());
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

        private static void AssertNoSharedStandingCells(WarehouseSimulation simulation, HashSet<GridPosition> standingCells)
        {
            standingCells.Clear();
            foreach (Bot bot in simulation.Bots)
            {
                Assert.IsTrue(simulation.Layout.Grid.IsWalkable(bot.Cell), "Bot inside a box at " + bot.Cell);
                if (!bot.IsMoving)
                    Assert.IsTrue(standingCells.Add(bot.Cell), "Two bots stand on " + bot.Cell + " at tick " + simulation.TickCount);
            }
        }
    }
}
