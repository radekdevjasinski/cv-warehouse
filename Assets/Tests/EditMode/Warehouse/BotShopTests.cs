using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class BotShopTests
    {
        private const int GridSide = 8;
        private const int HeavyPieces = 100;
        private const int MaxTicks = 400000;
        private const int FirstBotPrice = 6;
        private const int SecondBotPrice = 9;
        private const int WordCarryPrice = 15;
        private const int BoxCarryPrice = 30;
        private const int PlentyOfPoints = 150;

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
        public void TryUpgradeCarry_NotEnoughPoints_KeepsCarryingLetters()
        {
            Assert.IsFalse(shop.TryUpgradeCarry(FirstBot));
            Assert.AreEqual(CarrySize.Letter, FirstBot.CarrySize);
        }

        [Test]
        public void TryUpgradeCarry_EnoughPoints_GoesFromLettersToWordsToBoxes()
        {
            Earn(PlentyOfPoints);
            int pointsBefore = shop.Balance;
            Assert.AreEqual(WordCarryPrice, shop.CarryUpgradePrice(FirstBot));

            Assert.IsTrue(shop.TryUpgradeCarry(FirstBot));
            Assert.AreEqual(CarrySize.Word, FirstBot.CarrySize);
            Assert.AreEqual(BoxCarryPrice, shop.CarryUpgradePrice(FirstBot));

            Assert.IsTrue(shop.TryUpgradeCarry(FirstBot));
            Assert.AreEqual(CarrySize.Box, FirstBot.CarrySize);
            Assert.AreEqual(pointsBefore - WordCarryPrice - BoxCarryPrice, shop.Balance);
        }

        [Test]
        public void TryUpgradeCarry_AlreadyCarryingBoxes_ReturnsFalse()
        {
            Earn(PlentyOfPoints);
            shop.TryUpgradeCarry(FirstBot);
            shop.TryUpgradeCarry(FirstBot);

            Assert.IsFalse(BotShop.HasCarryUpgrade(FirstBot));
            Assert.IsFalse(shop.TryUpgradeCarry(FirstBot));
        }

        [Test]
        public void TryUpgradeCarry_OneBotUpgraded_LeavesTheOtherBotsCarryingLetters()
        {
            Earn(PlentyOfPoints);
            shop.TryBuyBot();
            Bot secondBot = simulation.Bots[1];

            Assert.IsTrue(shop.TryUpgradeCarry(secondBot));

            Assert.AreEqual(CarrySize.Word, secondBot.CarrySize);
            Assert.AreEqual(CarrySize.Letter, FirstBot.CarrySize);
            Assert.AreEqual(WordCarryPrice, shop.CarryUpgradePrice(FirstBot));
            Assert.AreEqual(BoxCarryPrice, shop.CarryUpgradePrice(secondBot));
        }

        [Test]
        public void TryBuyBot_AfterUpgradingEveryBot_AddsABotCarryingLetters()
        {
            Earn(PlentyOfPoints);
            shop.TryUpgradeCarry(FirstBot);

            shop.TryBuyBot();

            Assert.AreEqual(CarrySize.Letter, simulation.Bots[1].CarrySize);
        }

        [Test]
        public void BotPrice_WarehouseWithTwiceThePoints_IsTwiceAsHigh()
        {
            WarehouseSimulation biggerSimulation = NewSimulation(HeavyPieces * 2);
            BotShop biggerShop = NewShop(biggerSimulation);

            Assert.AreEqual(FirstBotPrice * 2, biggerShop.BotPrice);
            Assert.AreEqual(WordCarryPrice * 2, biggerShop.CarryUpgradePrice(biggerSimulation.Bots[0]));
        }

        [Test]
        public void BotPrice_TinyWarehouse_IsAtLeastOnePoint()
        {
            WarehouseSimulation tinySimulation = NewSimulation(1);
            BotShop tinyShop = NewShop(tinySimulation);

            Assert.AreEqual(1, tinyShop.BotPrice);
            Assert.AreEqual(1, tinyShop.CarryUpgradePrice(tinySimulation.Bots[0]));
        }

        private static WarehouseSimulation NewSimulation(int heavyPieces)
        {
            WarehouseLayout layout = new WarehouseLayoutBuilder(GridSide, GridSide)
                .AddDropCell(new GridPosition(0, 7))
                .AddParkingCell(new GridPosition(5, 7))
                .AddParkingCell(new GridPosition(6, 7))
                .AddParkingCell(new GridPosition(7, 7))
                .AddBox(new GridPosition(3, 2), WeightClass.Heavy, heavyPieces)
                .Build();
            var newSimulation = new WarehouseSimulation(layout, new SimulationSettings());
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
                WordCarryPricePermille = 50,
                BoxCarryPricePermille = 100
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
