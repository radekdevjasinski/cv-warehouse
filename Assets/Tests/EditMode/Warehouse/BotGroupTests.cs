using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Tests.EditMode.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class BotGroupTests
    {
        private const int GridSide = 8;
        private const int Seed = 12345;

        private WarehouseSimulation simulation;

        [SetUp]
        public void SetUp()
        {
            WarehouseLayout layout = new WarehouseLayoutBuilder(GridSide, GridSide)
                .AddDropCell(new GridPosition(0, 7))
                .AddParkingCell(new GridPosition(5, 7))
                .AddParkingCell(new GridPosition(6, 7))
                .AddParkingCell(new GridPosition(7, 7))
                .AddBox(new GridPosition(3, 2), TestBoxKinds.Large, CvTestBlocks.WithLetters(30))
                .Build();
            simulation = new WarehouseSimulation(layout, new SimulationSettings(), new System.Random(Seed));
            while (simulation.TryAddBot())
            {
            }
        }

        [Test]
        public void Levels_BotsAtTheSameLevel_HaveTheSameLowestAndHighest()
        {
            Assert.AreEqual(Bot.FirstLevel, BotGroup.LowestLevel(simulation.Bots, BotUpgrade.Carry));
            Assert.AreEqual(Bot.FirstLevel, BotGroup.HighestLevel(simulation.Bots, BotUpgrade.Carry));
        }

        [Test]
        public void Levels_OneBotUpgradedTwice_SpanFromTheLowestToTheHighest()
        {
            simulation.Upgrade(simulation.Bots[1], BotUpgrade.Speed);
            simulation.Upgrade(simulation.Bots[1], BotUpgrade.Speed);

            Assert.AreEqual(1, BotGroup.LowestLevel(simulation.Bots, BotUpgrade.Speed));
            Assert.AreEqual(3, BotGroup.HighestLevel(simulation.Bots, BotUpgrade.Speed));
            Assert.AreEqual(1, BotGroup.HighestLevel(simulation.Bots, BotUpgrade.Carry));
        }

        [Test]
        public void HasSharedFocus_EveryBotOnTheSameFocus_IsTrue()
        {
            Assert.IsTrue(BotGroup.HasSharedFocus(simulation.Bots));
        }

        [Test]
        public void HasSharedFocus_OneBotOnAnotherFocus_IsFalse()
        {
            simulation.SetFocus(simulation.Bots[2], BotFocus.BiggestBox);

            Assert.IsFalse(BotGroup.HasSharedFocus(simulation.Bots));
        }

        [Test]
        public void NextAndPreviousFocus_SharedFocus_StepFromIt()
        {
            foreach (Bot bot in simulation.Bots)
                simulation.SetFocus(bot, BotFocus.BiggestBox);

            Assert.AreEqual(BotFocus.SmallestBox, BotGroup.NextFocus(simulation.Bots));
            Assert.AreEqual(BotFocus.MostImportant, BotGroup.PreviousFocus(simulation.Bots));
        }

        [Test]
        public void NextAndPreviousFocus_DifferentFocuses_StartFromTheEndsOfTheCycle()
        {
            simulation.SetFocus(simulation.Bots[0], BotFocus.BiggestBox);
            simulation.SetFocus(simulation.Bots[1], BotFocus.SmallestBox);

            Assert.AreEqual(BotFocus.ClosestBox, BotGroup.NextFocus(simulation.Bots));
            Assert.AreEqual(BotFocus.SmallestBox, BotGroup.PreviousFocus(simulation.Bots));
        }
    }
}
