using System;
using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Tests.EditMode.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class WarehouseSimulationTests
    {
        private const int Seed = 12345;
        private const int MaxTicks = 400000;
        private const int SmallGridSide = 6;
        private const int SmallBoxPieces = 3;
        private const string SmallBoxText = "a b c";
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
            Assert.AreEqual(SmallBoxPieces, simulation.Score.Points);
        }

        [TestCase(Importance.Normal, 1)]
        [TestCase(Importance.Important, 2)]
        [TestCase(Importance.Key, 3)]
        public void Tick_BoxDelivered_ScoresEveryLetterByItsImportance(Importance importance, int pointsPerLetter)
        {
            WarehouseSimulation simulation = NewSimulationWithOneBox(CvTestBlocks.WithLetters(SmallBoxPieces), new SimulationSettings(), TestBoxKinds.Of(importance));
            simulation.TryAddBot();

            RunUntilComplete(simulation);

            Assert.AreEqual(SmallBoxPieces * pointsPerLetter, simulation.Score.Points);
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
        public void Tick_BotUpgradedToCarryEveryWord_EmptiesTheBoxInOneTrip()
        {
            WarehouseSimulation simulation = NewSmallSimulation(new SimulationSettings());
            simulation.TryAddBot();
            simulation.Upgrade(simulation.Bots[0], BotUpgrade.Carry);
            simulation.Upgrade(simulation.Bots[0], BotUpgrade.Carry);

            for (int tick = 0; tick < MaxTicks && simulation.Score.DeliveredPieces == 0; tick++)
                simulation.Tick();

            Assert.AreEqual(SmallBoxPieces, simulation.Score.DeliveredPieces);
            Assert.IsTrue(simulation.IsComplete);
        }

        [Test]
        public void Tick_NewBot_DeliversAWholeWordOnTheFirstTrip()
        {
            WarehouseSimulation simulation = NewSimulationWithOneBox(CvTestBlocks.FromText("ab cdef"), new SimulationSettings());
            var deliveredLetters = new List<int>();
            simulation.Score.Delivered += (block, letters) => deliveredLetters.AddRange(letters);
            simulation.TryAddBot();

            for (int tick = 0; tick < MaxTicks && simulation.Score.DeliveredPieces == 0; tick++)
                simulation.Tick();

            if (deliveredLetters.Count == 2)
                CollectionAssert.AreEqual(new[] { 0, 1 }, deliveredLetters);
            else
                CollectionAssert.AreEqual(new[] { 2, 3, 4, 5 }, deliveredLetters);
        }

        [Test]
        public void Tick_NewBot_MakesOneTripPerWordAndDeliversEveryLetterExactlyOnce()
        {
            CvBlock block = CvTestBlocks.FromText("ab cdef");
            WarehouseSimulation simulation = NewSimulationWithOneBox(block, new SimulationSettings());
            var deliveredLetters = new List<int>();
            int trips = 0;
            simulation.Score.Delivered += (deliveredBlock, letters) =>
            {
                Assert.AreSame(block, deliveredBlock);
                deliveredLetters.AddRange(letters);
                trips++;
            };
            simulation.TryAddBot();

            RunUntilComplete(simulation);

            Assert.AreEqual(block.Words.Count, trips);
            CollectionAssert.AreEquivalent(new[] { 0, 1, 2, 3, 4, 5 }, deliveredLetters);
        }

        [Test]
        public void Tick_BotWithMoreSpeed_FinishesSooner()
        {
            int slowTicks = TicksToEmptyTheSmallWarehouse(Bot.FirstLevel);
            int fasterTicks = TicksToEmptyTheSmallWarehouse(Bot.FirstLevel + 1);
            int fastestTicks = TicksToEmptyTheSmallWarehouse(Bot.FirstLevel + 4);

            Assert.Less(fasterTicks, slowTicks);
            Assert.Less(fastestTicks, fasterTicks);
        }

        [TestCase(1, 8, 11)]
        [TestCase(2, 6, 8)]
        [TestCase(3, 5, 7)]
        [TestCase(5, 4, 5)]
        public void StepTicks_HigherSpeedLevel_TakesFewerTicksPerStep(int speedLevel, int straightTicks, int diagonalTicks)
        {
            var settings = new SimulationSettings { TicksPerCell = 8, SpeedStepPercent = 25 };

            Assert.AreEqual(straightTicks, settings.StepTicks(speedLevel, GridDirections.StraightCost));
            Assert.AreEqual(diagonalTicks, settings.StepTicks(speedLevel, GridDirections.DiagonalCost));
        }

        [Test]
        public void StepTicks_AbsurdSpeed_StillTakesOneTick()
        {
            var settings = new SimulationSettings { TicksPerCell = 1, SpeedStepPercent = 1000 };

            Assert.AreEqual(1, settings.StepTicks(5, GridDirections.StraightCost));
        }

        [TestCase(Importance.Normal, 20)]
        [TestCase(Importance.Important, 30)]
        [TestCase(Importance.Key, 45)]
        public void Tick_BotAtABox_SpendsLongerTakingTextOutOfAMoreImportantBox(Importance importance, int expectedExtractTicks)
        {
            var settings = new SimulationSettings { NormalExtractTicks = 20, ImportantExtractTicks = 30, KeyExtractTicks = 45 };
            WarehouseSimulation simulation = NewSimulationWithOneBox(CvTestBlocks.WithLetters(SmallBoxPieces), settings, TestBoxKinds.Of(importance));
            simulation.TryAddBot();
            Bot bot = simulation.Bots[0];
            for (int tick = 0; tick < MaxTicks && bot.State != BotState.Extracting; tick++)
                simulation.Tick();

            int extractTicks = 0;
            for (int tick = 0; tick < MaxTicks && bot.State == BotState.Extracting; tick++)
            {
                simulation.Tick();
                extractTicks++;
            }

            Assert.AreEqual(expectedExtractTicks + 1, extractTicks);
        }

        [Test]
        public void ExtractProgress_WhileTakingTextOut_GrowsFromZeroToOneAndResetsWhenDone()
        {
            var settings = new SimulationSettings { NormalExtractTicks = 10 };
            WarehouseSimulation simulation = NewSimulationWithOneBox(CvTestBlocks.WithLetters(SmallBoxPieces), settings, TestBoxKinds.Of(Importance.Normal));
            simulation.TryAddBot();
            Bot bot = simulation.Bots[0];
            Assert.AreEqual(0f, bot.ExtractProgress);
            for (int tick = 0; tick < MaxTicks && bot.State != BotState.Extracting; tick++)
                simulation.Tick();

            Assert.AreEqual(0f, bot.ExtractProgress);
            for (int tick = 0; tick < 5; tick++)
                simulation.Tick();
            Assert.AreEqual(0.5f, bot.ExtractProgress, 0.001f);

            for (int tick = 0; tick < MaxTicks && bot.State == BotState.Extracting; tick++)
                simulation.Tick();
            Assert.AreEqual(0f, bot.ExtractProgress);
        }

        [Test]
        public void TryRemoveBot_BotCarryingText_CountsItsLettersAsDelivered()
        {
            WarehouseSimulation simulation = NewSmallSimulation(new SimulationSettings());
            simulation.TryAddBot();
            simulation.TryAddBot();
            Bot carrier = simulation.Bots[0];
            for (int tick = 0; tick < MaxTicks && carrier.State != BotState.GoingToDrop; tick++)
                simulation.Tick();

            Assert.IsTrue(simulation.TryRemoveBot(carrier));

            Assert.AreEqual(1, simulation.Score.DeliveredPieces);
        }

        [Test]
        public void TryRemoveBot_BotOnItsWayToABox_PutsItsLettersBackInTheBox()
        {
            WarehouseSimulation simulation = NewSmallSimulation(new SimulationSettings());
            simulation.TryAddBot();
            simulation.TryAddBot();
            simulation.Tick();
            Box box = simulation.Layout.Boxes[0];
            Assert.AreEqual(SmallBoxPieces - 2, box.UnclaimedPieces);

            Assert.IsTrue(simulation.TryRemoveBot(simulation.Bots[0]));

            Assert.AreEqual(SmallBoxPieces - 1, box.UnclaimedPieces);
            Assert.AreEqual(0, simulation.Score.DeliveredPieces);
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
        public void Tick_BotsWithDifferentCarryLevels_EachTakesItsOwnNumberOfWords()
        {
            WarehouseSimulation simulation = NewSimulationWithOneBox(CvTestBlocks.FromText("a b c d e f"), new SimulationSettings());
            simulation.TryAddBot();
            simulation.TryAddBot();
            simulation.Upgrade(simulation.Bots[1], BotUpgrade.Carry);
            simulation.Upgrade(simulation.Bots[1], BotUpgrade.Carry);

            simulation.Tick();

            Assert.AreEqual(1, simulation.Bots[0].ClaimedPieces);
            Assert.AreEqual(3, simulation.Bots[1].ClaimedPieces);
            Assert.AreEqual(2, simulation.Layout.Boxes[0].UnclaimedPieces);
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
        public void Upgrade_WhilePaused_StillApplies()
        {
            WarehouseSimulation simulation = NewSmallSimulation(new SimulationSettings());
            simulation.TryAddBot();
            simulation.TogglePause();

            simulation.Upgrade(simulation.Bots[0], BotUpgrade.Carry);
            simulation.Upgrade(simulation.Bots[0], BotUpgrade.Speed);
            simulation.SetFocus(simulation.Bots[0], BotFocus.BiggestBox);

            Assert.AreEqual(2, simulation.Bots[0].CarryLevel);
            Assert.AreEqual(2, simulation.Bots[0].SpeedLevel);
            Assert.AreEqual(BotFocus.BiggestBox, simulation.Bots[0].Focus);
        }

        [Test]
        public void TryAddBot_CrewNamesGiven_NamesBotsInRosterOrder()
        {
            WarehouseSimulation simulation = NewSmallSimulation(new SimulationSettings { CrewNames = new[] { "PIP", "BLIP" } });

            simulation.TryAddBot();
            simulation.TryAddBot();

            Assert.AreEqual("PIP", simulation.Bots[0].Name);
            Assert.AreEqual("BLIP", simulation.Bots[1].Name);
            Assert.AreEqual(1, simulation.Bots[1].CrewSlot);
        }

        [Test]
        public void TryAddBot_AfterABotWasRemoved_GivesItsNameToTheNewBot()
        {
            WarehouseSimulation simulation = NewSmallSimulation(new SimulationSettings { CrewNames = new[] { "PIP", "BLIP" } });
            simulation.TryAddBot();
            simulation.TryAddBot();
            simulation.TryRemoveBot(simulation.Bots[0]);

            simulation.TryAddBot();

            Assert.AreEqual("BLIP", simulation.Bots[0].Name);
            Assert.AreEqual("PIP", simulation.Bots[1].Name);
            Assert.AreEqual(0, simulation.Bots[1].CrewSlot);
        }

        [Test]
        public void TryAddBot_MoreBotsThanCrewNames_NumbersTheRest()
        {
            WarehouseSimulation simulation = NewSmallSimulation(new SimulationSettings { CrewNames = new[] { "PIP" } });

            simulation.TryAddBot();
            simulation.TryAddBot();

            Assert.AreEqual("PIP", simulation.Bots[0].Name);
            Assert.AreEqual("BOT 2", simulation.Bots[1].Name);
        }

        [Test]
        public void Tick_BotCarryingTwoWords_CountsTheWordsOfItsCargoUntilItUnloads()
        {
            WarehouseSimulation simulation = NewSmallSimulation(new SimulationSettings());
            simulation.TryAddBot();
            Bot bot = simulation.Bots[0];
            simulation.Upgrade(bot, BotUpgrade.Carry);
            Assert.AreEqual(0, bot.CargoWords);

            TickUntilState(simulation, bot, BotState.GoingToDrop);
            Assert.AreEqual(2, bot.CargoWords);

            TickUntilState(simulation, bot, BotState.GoingToBox);
            Assert.AreEqual(0, bot.CargoWords);

            TickUntilState(simulation, bot, BotState.GoingToDrop);
            Assert.AreEqual(1, bot.CargoWords);
        }

        private static void TickUntilState(WarehouseSimulation simulation, Bot bot, BotState state)
        {
            for (int tick = 0; tick < MaxTicks && bot.State != state; tick++)
                simulation.Tick();

            Assert.AreEqual(state, bot.State);
        }

        private static WarehouseSimulation NewSmallSimulation(SimulationSettings settings)
        {
            return NewSimulationWithOneBox(CvTestBlocks.FromText(SmallBoxText), settings);
        }

        private static int TicksToEmptyTheSmallWarehouse(int speedLevel)
        {
            WarehouseSimulation simulation = NewSmallSimulation(new SimulationSettings());
            simulation.TryAddBot();
            while (simulation.Bots[0].SpeedLevel < speedLevel)
                simulation.Upgrade(simulation.Bots[0], BotUpgrade.Speed);

            RunUntilComplete(simulation);

            Assert.IsTrue(simulation.IsComplete);
            return simulation.TickCount;
        }

        private static WarehouseSimulation NewSimulationWithOneBox(CvBlock block, SimulationSettings settings)
        {
            return NewSimulationWithOneBox(block, settings, TestBoxKinds.Medium);
        }

        private static WarehouseSimulation NewSimulationWithOneBox(CvBlock block, SimulationSettings settings, BoxKind kind)
        {
            WarehouseLayout layout = new WarehouseLayoutBuilder(SmallGridSide, SmallGridSide)
                .AddDropCell(new GridPosition(0, 5))
                .AddParkingCell(new GridPosition(5, 5))
                .AddParkingCell(new GridPosition(4, 5))
                .AddBox(new GridPosition(2, 2), kind, block)
                .Build();
            return new WarehouseSimulation(layout, settings, new Random(Seed));
        }

        private static WarehouseSimulation NewGeneratedSimulation(int botCount)
        {
            var generatorSettings = new GeneratorSettings
            {
                FillPercent = 12,
                MinWidth = 1,
                TruckWidth = 6,
                DropCells = 4,
                ParkingPerSide = 4,
                BigClusterRadius = 4
            };
            var random = new Random(Seed);
            Assert.IsTrue(new WarehouseGenerator(generatorSettings, random).TryGenerate(CvTestBlocks.BuildCv(5, 14, 45), out WarehouseLayout layout));
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
                points += box.TotalPieces * settings.PointsPerPiece(box.Importance);
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
