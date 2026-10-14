using System;
using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Tests.EditMode.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class BotFocusTests
    {
        private const int GridSide = 10;
        private const int Seed = 12345;
        private const int MaxTicks = 400000;

        private WarehouseSimulation simulation;
        private Box nearSmallBox;
        private Box farMediumBox;
        private Box nearerLargeBox;
        private Box furtherLargeBox;

        private Bot FirstBot => simulation.Bots[0];

        [SetUp]
        public void SetUp()
        {
            WarehouseLayout layout = new WarehouseLayoutBuilder(GridSide, GridSide)
                .AddDropCell(new GridPosition(0, 9))
                .AddParkingCell(new GridPosition(8, 9))
                .AddParkingCell(new GridPosition(9, 9))
                .AddBox(new GridPosition(1, 7), TestBoxKinds.Small, CvTestBlocks.WithLetters(1))
                .AddBox(new GridPosition(5, 5), TestBoxKinds.Medium, CvTestBlocks.WithLetters(1))
                .AddBox(new GridPosition(1, 3), TestBoxKinds.Large, CvTestBlocks.WithLetters(1))
                .AddBox(new GridPosition(6, 0), TestBoxKinds.Large, CvTestBlocks.WithLetters(1))
                .Build();
            nearSmallBox = layout.Boxes[0];
            farMediumBox = layout.Boxes[1];
            nearerLargeBox = layout.Boxes[2];
            furtherLargeBox = layout.Boxes[3];
            simulation = new WarehouseSimulation(layout, new SimulationSettings(), new System.Random(Seed));
            simulation.TryAddBot();
        }

        [Test]
        public void Tick_NewBot_FocusesOnTheClosestBox()
        {
            simulation.Tick();

            Assert.AreEqual(BotFocus.ClosestBox, FirstBot.Focus);
            Assert.AreSame(nearSmallBox, FirstBot.JobBox);
        }

        [Test]
        public void Tick_BotFocusedOnTheBiggestBox_GoesToTheLargeBoxNearestTheTruck()
        {
            simulation.SetFocus(FirstBot, BotFocus.BiggestBox);

            simulation.Tick();

            Assert.Less(nearerLargeBox.TruckDistance, furtherLargeBox.TruckDistance);
            Assert.AreSame(nearerLargeBox, FirstBot.JobBox);
        }

        [Test]
        public void Tick_BotFocusedOnTheBiggestBox_WorksDownFromLargeToSmall()
        {
            simulation.SetFocus(FirstBot, BotFocus.BiggestBox);

            Assert.AreSame(nearerLargeBox, NextJobBox());
            Assert.AreSame(furtherLargeBox, NextJobBox());
            Assert.AreSame(farMediumBox, NextJobBox());
            Assert.AreSame(nearSmallBox, NextJobBox());
        }

        [Test]
        public void Tick_BotFocusedOnTheSmallestBox_WorksUpFromSmallToLarge()
        {
            simulation.SetFocus(FirstBot, BotFocus.SmallestBox);

            Assert.AreSame(nearSmallBox, NextJobBox());
            Assert.AreSame(farMediumBox, NextJobBox());
            Assert.AreSame(nearerLargeBox, NextJobBox());
            Assert.AreSame(furtherLargeBox, NextJobBox());
        }

        [Test]
        public void SetFocus_FromBiggestToSmallestOnTheWay_TurnsToTheSmallBox()
        {
            simulation.SetFocus(FirstBot, BotFocus.BiggestBox);
            simulation.Tick();
            Assert.AreSame(nearerLargeBox, FirstBot.JobBox);

            simulation.SetFocus(FirstBot, BotFocus.SmallestBox);

            Assert.AreSame(nearSmallBox, FirstBot.JobBox);
        }

        [Test]
        public void Tick_BotFocusedOnTheSmallestBox_StillEmptiesTheWholeWarehouse()
        {
            simulation.SetFocus(FirstBot, BotFocus.SmallestBox);

            for (int tick = 0; tick < MaxTicks && !simulation.IsComplete; tick++)
                simulation.Tick();

            Assert.IsTrue(simulation.IsComplete);
        }

        [Test]
        public void Tick_BotsWithDifferentFocus_EachFollowsItsOwn()
        {
            simulation.TryAddBot();
            simulation.SetFocus(simulation.Bots[1], BotFocus.BiggestBox);

            simulation.Tick();

            Assert.AreSame(nearSmallBox, FirstBot.JobBox);
            Assert.AreSame(nearerLargeBox, simulation.Bots[1].JobBox);
        }

        [Test]
        public void SetFocus_BotOnItsWayToABox_TurnsToTheBoxOfTheNewFocusStraightAway()
        {
            simulation.Tick();
            Assert.AreEqual(BotState.GoingToBox, FirstBot.State);
            Assert.AreSame(nearSmallBox, FirstBot.JobBox);

            simulation.SetFocus(FirstBot, BotFocus.BiggestBox);

            Assert.AreEqual(BotState.GoingToBox, FirstBot.State);
            Assert.AreSame(nearerLargeBox, FirstBot.JobBox);
            Assert.AreEqual(FirstBot.AccessCell, FirstBot.Destination);
            Assert.AreEqual(1, FirstBot.ClaimedPieces);
            Assert.AreEqual(nearSmallBox.TotalPieces, nearSmallBox.UnclaimedPieces);
        }

        [Test]
        public void SetFocus_BotOnItsWayToABox_FreesTheOldBoxForAnotherBot()
        {
            simulation.Tick();
            simulation.SetFocus(FirstBot, BotFocus.BiggestBox);

            simulation.TryAddBot();
            simulation.Tick();

            Assert.AreSame(nearSmallBox, simulation.Bots[1].JobBox);
        }

        [Test]
        public void SetFocus_BotInTheMiddleOfAStep_PlansTheNewRouteFromTheCellItIsSteppingOnto()
        {
            simulation.Tick();
            simulation.Tick();
            Assert.IsTrue(FirstBot.IsMoving);

            simulation.SetFocus(FirstBot, BotFocus.BiggestBox);

            Assert.Greater(FirstBot.RouteLength, 0);
            GridPosition firstRouteCell = FirstBot.RouteCell(0);
            Assert.LessOrEqual(Math.Abs(firstRouteCell.X - FirstBot.NextCell.X), 1);
            Assert.LessOrEqual(Math.Abs(firstRouteCell.Y - FirstBot.NextCell.Y), 1);
            Assert.AreEqual(FirstBot.Destination, FirstBot.RouteCell(FirstBot.RouteLength - 1));
        }

        [Test]
        public void SetFocus_BotRetargeted_ReachesTheNewBoxAndDeliversIt()
        {
            simulation.Tick();
            simulation.SetFocus(FirstBot, BotFocus.BiggestBox);

            for (int tick = 0; tick < MaxTicks && !nearerLargeBox.IsEmpty; tick++)
                simulation.Tick();

            Assert.IsTrue(nearerLargeBox.IsEmpty);
            Assert.IsFalse(nearSmallBox.IsEmpty);
        }

        [Test]
        public void SetFocus_BotCarryingTextBack_FinishesTheTripBeforeSwitching()
        {
            for (int tick = 0; tick < MaxTicks && FirstBot.State != BotState.GoingToDrop; tick++)
                simulation.Tick();
            GridPosition dropCell = FirstBot.Destination;

            simulation.SetFocus(FirstBot, BotFocus.BiggestBox);

            Assert.AreEqual(BotState.GoingToDrop, FirstBot.State);
            Assert.AreSame(nearSmallBox, FirstBot.JobBox);
            Assert.AreEqual(dropCell, FirstBot.Destination);
            Assert.AreSame(nearerLargeBox, NextJobBox());
        }

        [Test]
        public void SetFocus_BotTakingTextOutOfABox_KeepsItsJob()
        {
            for (int tick = 0; tick < MaxTicks && FirstBot.State != BotState.Extracting; tick++)
                simulation.Tick();

            simulation.SetFocus(FirstBot, BotFocus.BiggestBox);

            Assert.AreEqual(BotState.Extracting, FirstBot.State);
            Assert.AreSame(nearSmallBox, FirstBot.JobBox);
        }

        [Test]
        public void SetFocus_SameFocusAgain_KeepsTheJobAndTheRoute()
        {
            simulation.Tick();
            int routeLength = FirstBot.RouteLength;
            GridPosition accessCell = FirstBot.AccessCell;

            simulation.SetFocus(FirstBot, BotFocus.ClosestBox);

            Assert.AreSame(nearSmallBox, FirstBot.JobBox);
            Assert.AreEqual(accessCell, FirstBot.AccessCell);
            Assert.AreEqual(routeLength, FirstBot.RouteLength);
        }

        [Test]
        public void Tick_BotFocusedOnTheMostImportantBox_WorksDownFromKeyToNormal()
        {
            WarehouseLayout layout = new WarehouseLayoutBuilder(GridSide, GridSide)
                .AddDropCell(new GridPosition(0, 9))
                .AddParkingCell(new GridPosition(9, 9))
                .AddBox(new GridPosition(1, 7), TestBoxKinds.Of(Importance.Normal), CvTestBlocks.WithLetters(1))
                .AddBox(new GridPosition(5, 5), TestBoxKinds.Of(Importance.Important), CvTestBlocks.WithLetters(1))
                .AddBox(new GridPosition(7, 1), TestBoxKinds.Of(Importance.Key), CvTestBlocks.WithLetters(1))
                .AddBox(new GridPosition(2, 4), TestBoxKinds.Of(Importance.Key), CvTestBlocks.WithLetters(1))
                .Build();
            simulation = new WarehouseSimulation(layout, new SimulationSettings(), new System.Random(Seed));
            simulation.TryAddBot();
            simulation.SetFocus(FirstBot, BotFocus.MostImportant);

            Assert.Less(layout.Boxes[3].TruckDistance, layout.Boxes[2].TruckDistance);
            Assert.AreSame(layout.Boxes[3], NextJobBox());
            Assert.AreSame(layout.Boxes[2], NextJobBox());
            Assert.AreSame(layout.Boxes[1], NextJobBox());
            Assert.AreSame(layout.Boxes[0], NextJobBox());
        }

        [Test]
        public void Next_LastFocus_WrapsAroundToTheFirst()
        {
            Assert.AreEqual(BotFocus.MostImportant, BotFocusCycle.Next(BotFocus.ClosestBox));
            Assert.AreEqual(BotFocus.BiggestBox, BotFocusCycle.Next(BotFocus.MostImportant));
            Assert.AreEqual(BotFocus.SmallestBox, BotFocusCycle.Next(BotFocus.BiggestBox));
            Assert.AreEqual(BotFocus.ClosestBox, BotFocusCycle.Next(BotFocus.SmallestBox));
        }

        [Test]
        public void Previous_FirstFocus_WrapsAroundToTheLast()
        {
            Assert.AreEqual(BotFocus.SmallestBox, BotFocusCycle.Previous(BotFocus.ClosestBox));
            Assert.AreEqual(BotFocus.BiggestBox, BotFocusCycle.Previous(BotFocus.SmallestBox));
            Assert.AreEqual(BotFocus.MostImportant, BotFocusCycle.Previous(BotFocus.BiggestBox));
            Assert.AreEqual(BotFocus.ClosestBox, BotFocusCycle.Previous(BotFocus.MostImportant));
        }

        [Test]
        public void Next_EveryFocus_IsUndoneByPrevious()
        {
            foreach (BotFocus focus in Enum.GetValues(typeof(BotFocus)))
                Assert.AreEqual(focus, BotFocusCycle.Previous(BotFocusCycle.Next(focus)));
        }

        [Test]
        public void Tick_BotFocusedOnTheBiggestBox_StillEmptiesTheWholeWarehouse()
        {
            simulation.SetFocus(FirstBot, BotFocus.BiggestBox);

            for (int tick = 0; tick < MaxTicks && !simulation.IsComplete; tick++)
                simulation.Tick();

            Assert.IsTrue(simulation.IsComplete);
        }

        private Box NextJobBox()
        {
            Box previousBox = FirstBot.JobBox;
            for (int tick = 0; tick < MaxTicks && (FirstBot.JobBox == null || FirstBot.JobBox == previousBox); tick++)
                simulation.Tick();

            return FirstBot.JobBox;
        }
    }
}
