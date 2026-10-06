using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class BotPickerTests
    {
        private const int GridSide = 8;
        private const float PickRadius = 0.6f;
        private const float FirstBotCentreX = 5.5f;
        private const float SecondBotCentreX = 6.5f;
        private const float ParkingRowCentreY = 7.5f;

        private WarehouseSimulation simulation;

        [SetUp]
        public void SetUp()
        {
            WarehouseLayout layout = new WarehouseLayoutBuilder(GridSide, GridSide)
                .AddDropCell(new GridPosition(0, 7))
                .AddParkingCell(new GridPosition(5, 7))
                .AddParkingCell(new GridPosition(6, 7))
                .AddBox(new GridPosition(3, 2), WeightClass.Heavy, 10)
                .Build();
            simulation = new WarehouseSimulation(layout, new SimulationSettings());
            simulation.TryAddBot();
            simulation.TryAddBot();
        }

        [Test]
        public void TryPick_PointOnABot_PicksThatBot()
        {
            Assert.IsTrue(BotPicker.TryPick(simulation.Bots, SecondBotCentreX, ParkingRowCentreY, PickRadius, out Bot picked));
            Assert.AreSame(simulation.Bots[1], picked);
        }

        [Test]
        public void TryPick_PointBetweenTwoBots_PicksTheNearerOne()
        {
            Assert.IsTrue(BotPicker.TryPick(simulation.Bots, FirstBotCentreX + 0.4f, ParkingRowCentreY, PickRadius, out Bot picked));
            Assert.AreSame(simulation.Bots[0], picked);
        }

        [Test]
        public void TryPick_PointFarFromEveryBot_PicksNothing()
        {
            Assert.IsFalse(BotPicker.TryPick(simulation.Bots, 1.5f, 1.5f, PickRadius, out Bot picked));
            Assert.IsNull(picked);
        }

        [Test]
        public void TryPick_PointJustOutsideTheRadius_PicksNothing()
        {
            Assert.IsFalse(BotPicker.TryPick(simulation.Bots, FirstBotCentreX - PickRadius - 0.1f, ParkingRowCentreY, PickRadius, out _));
        }

        [Test]
        public void PickInside_AreaAroundBothBots_PicksBoth()
        {
            var picked = new List<Bot>();

            BotPicker.PickInside(simulation.Bots, new SelectionArea(5f, 7f, 7f, 8f), picked);

            CollectionAssert.AreEqual(simulation.Bots, picked);
        }

        [Test]
        public void PickInside_AreaAroundOneBot_PicksOnlyThatBot()
        {
            var picked = new List<Bot>();

            BotPicker.PickInside(simulation.Bots, new SelectionArea(6.1f, 7.1f, 6.9f, 7.9f), picked);

            Assert.AreEqual(1, picked.Count);
            Assert.AreSame(simulation.Bots[1], picked[0]);
        }

        [Test]
        public void PickInside_AreaThatOnlyTouchesTheEdgeOfABot_PicksNothing()
        {
            var picked = new List<Bot>();

            BotPicker.PickInside(simulation.Bots, new SelectionArea(4f, 7f, 5.3f, 8f), picked);

            Assert.AreEqual(0, picked.Count);
        }

        [Test]
        public void PickInside_ListWithOldPicks_ReplacesThem()
        {
            var picked = new List<Bot> { simulation.Bots[0], simulation.Bots[1] };

            BotPicker.PickInside(simulation.Bots, new SelectionArea(0f, 0f, 2f, 2f), picked);

            Assert.AreEqual(0, picked.Count);
        }

        [Test]
        public void SelectionArea_DraggedFromTopRightToBottomLeft_CoversTheSameCells()
        {
            var area = new SelectionArea(7f, 8f, 5f, 7f);

            Assert.AreEqual(5f, area.MinX);
            Assert.AreEqual(7f, area.MinY);
            Assert.AreEqual(2f, area.Width);
            Assert.AreEqual(1f, area.Height);
            Assert.IsTrue(area.Contains(FirstBotCentreX, ParkingRowCentreY));
            Assert.IsFalse(area.Contains(FirstBotCentreX, 6.9f));
        }

        [Test]
        public void TryPick_BotOnTheMove_IsPickedWhereItIsDrawn()
        {
            Bot bot = simulation.Bots[0];
            simulation.Tick();
            simulation.Tick();
            Assert.IsTrue(bot.IsMoving);

            Assert.IsTrue(BotPicker.TryPick(simulation.Bots, bot.CentreX, bot.CentreY, 0.05f, out Bot picked));
            Assert.AreSame(bot, picked);
        }
    }
}
