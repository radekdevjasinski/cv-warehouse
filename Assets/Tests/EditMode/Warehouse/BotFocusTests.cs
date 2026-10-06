using System;
using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class BotFocusTests
    {
        private const int GridSide = 10;
        private const int MaxTicks = 400000;

        private WarehouseSimulation simulation;
        private Box nearLightBox;
        private Box farMediumBox;
        private Box nearerHeavyBox;
        private Box furtherHeavyBox;

        private Bot FirstBot => simulation.Bots[0];

        [SetUp]
        public void SetUp()
        {
            WarehouseLayout layout = new WarehouseLayoutBuilder(GridSide, GridSide)
                .AddDropCell(new GridPosition(0, 9))
                .AddParkingCell(new GridPosition(8, 9))
                .AddParkingCell(new GridPosition(9, 9))
                .AddBox(new GridPosition(1, 7), WeightClass.Light, 1)
                .AddBox(new GridPosition(5, 5), WeightClass.Medium, 1)
                .AddBox(new GridPosition(1, 3), WeightClass.Heavy, 1)
                .AddBox(new GridPosition(6, 0), WeightClass.Heavy, 1)
                .Build();
            nearLightBox = layout.Boxes[0];
            farMediumBox = layout.Boxes[1];
            nearerHeavyBox = layout.Boxes[2];
            furtherHeavyBox = layout.Boxes[3];
            simulation = new WarehouseSimulation(layout, new SimulationSettings());
            simulation.TryAddBot();
        }

        [Test]
        public void Tick_NewBot_FocusesOnTheClosestBox()
        {
            simulation.Tick();

            Assert.AreEqual(BotFocus.ClosestBox, FirstBot.Focus);
            Assert.AreSame(nearLightBox, FirstBot.JobBox);
        }

        [Test]
        public void Tick_BotFocusedOnTheBiggestBox_GoesToTheHeavyBoxNearestTheTruck()
        {
            simulation.SetFocus(FirstBot, BotFocus.BiggestBox);

            simulation.Tick();

            Assert.Less(nearerHeavyBox.TruckDistance, furtherHeavyBox.TruckDistance);
            Assert.AreSame(nearerHeavyBox, FirstBot.JobBox);
        }

        [Test]
        public void Tick_BotFocusedOnTheBiggestBox_WorksDownFromHeavyToLight()
        {
            simulation.SetFocus(FirstBot, BotFocus.BiggestBox);

            Assert.AreSame(nearerHeavyBox, NextJobBox());
            Assert.AreSame(furtherHeavyBox, NextJobBox());
            Assert.AreSame(farMediumBox, NextJobBox());
            Assert.AreSame(nearLightBox, NextJobBox());
        }

        [Test]
        public void Tick_BotsWithDifferentFocus_EachFollowsItsOwn()
        {
            simulation.TryAddBot();
            simulation.SetFocus(simulation.Bots[1], BotFocus.BiggestBox);

            simulation.Tick();

            Assert.AreSame(nearLightBox, FirstBot.JobBox);
            Assert.AreSame(nearerHeavyBox, simulation.Bots[1].JobBox);
        }

        [Test]
        public void SetFocus_BotOnItsWayToABox_TurnsToTheBoxOfTheNewFocusStraightAway()
        {
            simulation.Tick();
            Assert.AreEqual(BotState.GoingToBox, FirstBot.State);
            Assert.AreSame(nearLightBox, FirstBot.JobBox);

            simulation.SetFocus(FirstBot, BotFocus.BiggestBox);

            Assert.AreEqual(BotState.GoingToBox, FirstBot.State);
            Assert.AreSame(nearerHeavyBox, FirstBot.JobBox);
            Assert.AreEqual(FirstBot.AccessCell, FirstBot.Destination);
            Assert.AreEqual(1, FirstBot.ClaimedPieces);
            Assert.AreEqual(nearLightBox.TotalPieces, nearLightBox.UnclaimedPieces);
        }

        [Test]
        public void SetFocus_BotOnItsWayToABox_FreesTheOldBoxForAnotherBot()
        {
            simulation.Tick();
            simulation.SetFocus(FirstBot, BotFocus.BiggestBox);

            simulation.TryAddBot();
            simulation.Tick();

            Assert.AreSame(nearLightBox, simulation.Bots[1].JobBox);
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

            for (int tick = 0; tick < MaxTicks && !nearerHeavyBox.IsEmpty; tick++)
                simulation.Tick();

            Assert.IsTrue(nearerHeavyBox.IsEmpty);
            Assert.IsFalse(nearLightBox.IsEmpty);
        }

        [Test]
        public void SetFocus_BotCarryingTextBack_FinishesTheTripBeforeSwitching()
        {
            for (int tick = 0; tick < MaxTicks && FirstBot.State != BotState.GoingToDrop; tick++)
                simulation.Tick();
            GridPosition dropCell = FirstBot.Destination;

            simulation.SetFocus(FirstBot, BotFocus.BiggestBox);

            Assert.AreEqual(BotState.GoingToDrop, FirstBot.State);
            Assert.AreSame(nearLightBox, FirstBot.JobBox);
            Assert.AreEqual(dropCell, FirstBot.Destination);
            Assert.AreSame(nearerHeavyBox, NextJobBox());
        }

        [Test]
        public void SetFocus_BotTakingTextOutOfABox_KeepsItsJob()
        {
            for (int tick = 0; tick < MaxTicks && FirstBot.State != BotState.Extracting; tick++)
                simulation.Tick();

            simulation.SetFocus(FirstBot, BotFocus.BiggestBox);

            Assert.AreEqual(BotState.Extracting, FirstBot.State);
            Assert.AreSame(nearLightBox, FirstBot.JobBox);
        }

        [Test]
        public void SetFocus_SameFocusAgain_KeepsTheJobAndTheRoute()
        {
            simulation.Tick();
            int routeLength = FirstBot.RouteLength;
            GridPosition accessCell = FirstBot.AccessCell;

            simulation.SetFocus(FirstBot, BotFocus.ClosestBox);

            Assert.AreSame(nearLightBox, FirstBot.JobBox);
            Assert.AreEqual(accessCell, FirstBot.AccessCell);
            Assert.AreEqual(routeLength, FirstBot.RouteLength);
        }

        [Test]
        public void Next_LastFocus_WrapsAroundToTheFirst()
        {
            Assert.AreEqual(BotFocus.BiggestBox, BotFocusCycle.Next(BotFocus.ClosestBox));
            Assert.AreEqual(BotFocus.ClosestBox, BotFocusCycle.Next(BotFocus.BiggestBox));
        }

        [Test]
        public void Previous_FirstFocus_WrapsAroundToTheLast()
        {
            Assert.AreEqual(BotFocus.BiggestBox, BotFocusCycle.Previous(BotFocus.ClosestBox));
            Assert.AreEqual(BotFocus.ClosestBox, BotFocusCycle.Previous(BotFocus.BiggestBox));
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
