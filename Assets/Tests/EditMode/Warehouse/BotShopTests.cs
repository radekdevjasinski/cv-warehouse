using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Tests.EditMode.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class BotShopTests
    {
        private const int GridSide = 8;
        private const int Seed = 12345;
        private const int HeavyPieces = 100;
        private const int MaxTicks = 400000;
        private const int FirstBotPrice = 6;
        private const int SecondBotPrice = 9;
        private const int LastLevel = 5;
        private const int PlentyOfPoints = 150;

        private static readonly int[] CarryPrices = { 15, 22, 33, 49 };
        private static readonly int[] SpeedPrices = { 6, 9, 13, 19 };

        private WarehouseSimulation simulation;
        private BotShop shop;

        private Bot FirstBot => simulation.Bots[0];

        [SetUp]
        public void SetUp()
        {
            simulation = NewSimulation(HeavyPieces);
            shop = NewShop(simulation);
        }

        [Test]
        public void TryBuyBot_NotEnoughPoints_AddsNoBot()
        {
            Assert.AreEqual(0, shop.Balance);
            Assert.IsFalse(shop.CanBuyBot);
            Assert.IsFalse(shop.TryBuyBot());
            Assert.AreEqual(1, simulation.Bots.Count);
        }

        [Test]
        public void TryBuyBot_EnoughPoints_AddsABotAndTakesItsPrice()
        {
            Earn(FirstBotPrice);
            int pointsBefore = shop.Balance;

            Assert.IsTrue(shop.TryBuyBot());

            Assert.AreEqual(2, simulation.Bots.Count);
            Assert.AreEqual(pointsBefore - FirstBotPrice, shop.Balance);
        }

        [Test]
        public void BotPrice_AfterBuyingABot_Grows()
        {
            Assert.AreEqual(FirstBotPrice, shop.BotPrice);
            Earn(FirstBotPrice);

            shop.TryBuyBot();

            Assert.AreEqual(SecondBotPrice, shop.BotPrice);
        }

        [Test]
        public void BotPrice_AfterRemovingTheBoughtBot_DropsBack()
        {
            Earn(FirstBotPrice);
            shop.TryBuyBot();

            Assert.IsTrue(simulation.TryRemoveBot(simulation.Bots[1]));

            Assert.AreEqual(FirstBotPrice, shop.BotPrice);
        }

        [Test]
        public void TryBuyBot_EveryBotSlotTaken_ReturnsFalse()
        {
            Earn(PlentyOfPoints);
            while (shop.HasBotSlot)
                Assert.IsTrue(shop.TryBuyBot());

            Assert.AreEqual(simulation.MaxBots, simulation.Bots.Count);
            Assert.IsFalse(shop.CanBuyBot);
            Assert.IsFalse(shop.TryBuyBot());
        }

        [Test]
        public void TryAddBot_NewBot_StartsAtTheFirstLevelOfEveryUpgrade()
        {
            Assert.AreEqual(1, FirstBot.CarryLevel);
            Assert.AreEqual(1, FirstBot.SpeedLevel);
        }

        [TestCase(BotUpgrade.Carry)]
        [TestCase(BotUpgrade.Speed)]
        public void TryUpgrade_NotEnoughPoints_KeepsTheLevel(BotUpgrade upgrade)
        {
            Assert.IsTrue(shop.HasUpgrade(FirstBot, upgrade));
            Assert.IsFalse(shop.CanUpgrade(FirstBot, upgrade));
            Assert.IsFalse(shop.TryUpgrade(FirstBot, upgrade));
            Assert.AreEqual(1, FirstBot.LevelOf(upgrade));
        }

        [Test]
        public void TryUpgrade_Carry_GoesUpALevelAtATimeAndEveryLevelCostsMore()
        {
            Earn(PlentyOfPoints);
            int pointsBefore = shop.Balance;

            for (int level = 0; level < CarryPrices.Length; level++)
            {
                Assert.AreEqual(CarryPrices[level], shop.UpgradePrice(FirstBot, BotUpgrade.Carry));
                Assert.IsTrue(shop.TryUpgrade(FirstBot, BotUpgrade.Carry));
                Assert.AreEqual(level + 2, FirstBot.CarryLevel);
            }

            Assert.AreEqual(pointsBefore - 15 - 22 - 33 - 49, shop.Balance);
            Assert.AreEqual(1, FirstBot.SpeedLevel);
        }

        [Test]
        public void TryUpgrade_Speed_GoesUpALevelAtATimeAndEveryLevelCostsMore()
        {
            Earn(PlentyOfPoints);
            int pointsBefore = shop.Balance;

            for (int level = 0; level < SpeedPrices.Length; level++)
            {
                Assert.AreEqual(SpeedPrices[level], shop.UpgradePrice(FirstBot, BotUpgrade.Speed));
                Assert.IsTrue(shop.TryUpgrade(FirstBot, BotUpgrade.Speed));
                Assert.AreEqual(level + 2, FirstBot.SpeedLevel);
            }

            Assert.AreEqual(pointsBefore - 6 - 9 - 13 - 19, shop.Balance);
            Assert.AreEqual(1, FirstBot.CarryLevel);
        }

        [TestCase(BotUpgrade.Carry)]
        [TestCase(BotUpgrade.Speed)]
        public void TryUpgrade_AlreadyAtTheLastLevel_ReturnsFalse(BotUpgrade upgrade)
        {
            Earn(PlentyOfPoints);
            for (int level = 1; level < LastLevel; level++)
                Assert.IsTrue(shop.TryUpgrade(FirstBot, upgrade));
            int pointsBefore = shop.Balance;

            Assert.AreEqual(LastLevel, FirstBot.LevelOf(upgrade));
            Assert.IsFalse(shop.HasUpgrade(FirstBot, upgrade));
            Assert.IsFalse(shop.TryUpgrade(FirstBot, upgrade));
            Assert.AreEqual(LastLevel, FirstBot.LevelOf(upgrade));
            Assert.AreEqual(pointsBefore, shop.Balance);
        }

        [Test]
        public void TryUpgrade_OneBotUpgraded_LeavesTheOtherBotsAlone()
        {
            Earn(PlentyOfPoints);
            shop.TryBuyBot();
            Bot secondBot = simulation.Bots[1];

            Assert.IsTrue(shop.TryUpgrade(secondBot, BotUpgrade.Carry));

            Assert.AreEqual(2, secondBot.CarryLevel);
            Assert.AreEqual(1, FirstBot.CarryLevel);
            Assert.AreEqual(CarryPrices[0], shop.UpgradePrice(FirstBot, BotUpgrade.Carry));
            Assert.AreEqual(CarryPrices[1], shop.UpgradePrice(secondBot, BotUpgrade.Carry));
        }

        [Test]
        public void TryBuyBot_AfterUpgradingEveryBot_AddsABotWithoutUpgrades()
        {
            Earn(PlentyOfPoints);
            shop.TryUpgrade(FirstBot, BotUpgrade.Carry);
            shop.TryUpgrade(FirstBot, BotUpgrade.Speed);

            shop.TryBuyBot();

            Assert.AreEqual(1, simulation.Bots[1].CarryLevel);
            Assert.AreEqual(1, simulation.Bots[1].SpeedLevel);
        }

        [Test]
        public void BotPrice_WarehouseWithTwiceThePoints_IsTwiceAsHigh()
        {
            WarehouseSimulation biggerSimulation = NewSimulation(HeavyPieces * 2);
            BotShop biggerShop = NewShop(biggerSimulation);

            Assert.AreEqual(FirstBotPrice * 2, biggerShop.BotPrice);
            Assert.AreEqual(CarryPrices[0] * 2, biggerShop.UpgradePrice(biggerSimulation.Bots[0], BotUpgrade.Carry));
            Assert.AreEqual(SpeedPrices[0] * 2, biggerShop.UpgradePrice(biggerSimulation.Bots[0], BotUpgrade.Speed));
        }

        [Test]
        public void BotPrice_TinyWarehouse_IsAtLeastOnePoint()
        {
            WarehouseSimulation tinySimulation = NewSimulation(1);
            BotShop tinyShop = NewShop(tinySimulation);

            Assert.AreEqual(1, tinyShop.BotPrice);
            Assert.AreEqual(1, tinyShop.UpgradePrice(tinySimulation.Bots[0], BotUpgrade.Carry));
            Assert.AreEqual(1, tinyShop.UpgradePrice(tinySimulation.Bots[0], BotUpgrade.Speed));
        }

        [Test]
        public void UpgradePrice_TinyWarehouse_StillGrowsWithEveryLevel()
        {
            WarehouseSimulation tinySimulation = NewSimulation(1);
            BotShop tinyShop = NewShop(tinySimulation);
            Bot bot = tinySimulation.Bots[0];

            tinySimulation.Upgrade(bot, BotUpgrade.Carry);

            Assert.AreEqual(2, tinyShop.UpgradePrice(bot, BotUpgrade.Carry));
        }

        private static WarehouseSimulation NewSimulation(int heavyPieces)
        {
            WarehouseLayout layout = new WarehouseLayoutBuilder(GridSide, GridSide)
                .AddDropCell(new GridPosition(0, 7))
                .AddParkingCell(new GridPosition(5, 7))
                .AddParkingCell(new GridPosition(6, 7))
                .AddParkingCell(new GridPosition(7, 7))
                .AddBox(new GridPosition(3, 2), WeightClass.Heavy, CvTestBlocks.WithLetters(heavyPieces))
                .Build();
            var newSimulation = new WarehouseSimulation(layout, new SimulationSettings(), new System.Random(Seed));
            newSimulation.TryAddBot();
            return newSimulation;
        }

        private static BotShop NewShop(WarehouseSimulation shopSimulation)
        {
            return new BotShop(shopSimulation, new ShopSettings
            {
                FreeBots = 1,
                FirstBotPricePermille = 20,
                BotPriceGrowthPercent = 150,
                MaxUpgradeLevel = LastLevel,
                CarryUpgradePricePermille = 50,
                SpeedUpgradePricePermille = 20,
                UpgradePriceGrowthPercent = 150
            });
        }

        private void Earn(int points)
        {
            for (int tick = 0; tick < MaxTicks && simulation.Score.Points < points; tick++)
                simulation.Tick();

            Assert.GreaterOrEqual(simulation.Score.Points, points);
        }
    }
}
