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

            simulation.TryRemoveBot();

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
            Assert.IsFalse(shop.TryUpgradeCarry());
            Assert.AreEqual(CarrySize.Letter, simulation.CarrySize);
        }

        [Test]
        public void TryUpgradeCarry_EnoughPoints_GoesFromLettersToWordsToBoxes()
        {
            Earn(PlentyOfPoints);
            int pointsBefore = shop.Balance;
            Assert.AreEqual(WordCarryPrice, shop.CarryUpgradePrice);

            Assert.IsTrue(shop.TryUpgradeCarry());
            Assert.AreEqual(CarrySize.Word, simulation.CarrySize);
            Assert.AreEqual(BoxCarryPrice, shop.CarryUpgradePrice);

            Assert.IsTrue(shop.TryUpgradeCarry());
            Assert.AreEqual(CarrySize.Box, simulation.CarrySize);
            Assert.AreEqual(pointsBefore - WordCarryPrice - BoxCarryPrice, shop.Balance);
        }

        [Test]
        public void TryUpgradeCarry_AlreadyCarryingBoxes_ReturnsFalse()
        {
            Earn(PlentyOfPoints);
            shop.TryUpgradeCarry();
            shop.TryUpgradeCarry();

            Assert.IsFalse(shop.HasCarryUpgrade);
            Assert.IsFalse(shop.TryUpgradeCarry());
        }

        [Test]
        public void BotPrice_WarehouseWithTwiceThePoints_IsTwiceAsHigh()
        {
            BotShop biggerShop = NewShop(NewSimulation(HeavyPieces * 2));

            Assert.AreEqual(FirstBotPrice * 2, biggerShop.BotPrice);
            Assert.AreEqual(WordCarryPrice * 2, biggerShop.CarryUpgradePrice);
        }

        [Test]
        public void BotPrice_TinyWarehouse_IsAtLeastOnePoint()
        {
            BotShop tinyShop = NewShop(NewSimulation(1));

            Assert.AreEqual(1, tinyShop.BotPrice);
            Assert.AreEqual(1, tinyShop.CarryUpgradePrice);
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
