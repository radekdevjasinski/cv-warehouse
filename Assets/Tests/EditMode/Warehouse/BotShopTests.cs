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

        [TestCase(2, 4, 8)]
        [TestCase(20, 40, 80)]
        [TestCase(60, 120, 240)]
        public void CarryUpgradePrice_AnyCvSize_WordCarryCostsNoMoreThanTheNameIsWorth(int heavyEntries, int mediumEntries, int lightEntries)
        {
            WarehouseSimulation cvSimulation = NewCvSimulation(heavyEntries, mediumEntries, lightEntries);
            var cvShop = new BotShop(cvSimulation, new ShopSettings());
            Box nameBox = null;
            foreach (Box box in cvSimulation.Layout.Boxes)
                if (box.Block.Role == CvBlockRole.Name)
                    nameBox = box;

            Assert.LessOrEqual(cvShop.CarryUpgradePrice(cvSimulation.Bots[0]), cvSimulation.Score.PointsIn(nameBox));
        }

        [Test]
        public void CarryUpgradePrice_LongCv_WordCarryIsCheaperThanItsShareOfAllPoints()
        {
            WarehouseSimulation cvSimulation = NewCvSimulation(60, 120, 240);
            var settings = new ShopSettings();

            int price = new BotShop(cvSimulation, settings).CarryUpgradePrice(cvSimulation.Bots[0]);

            Assert.Less(price, cvSimulation.Score.MaxPoints * settings.WordCarryPricePermille / 1000);
        }

        private static WarehouseSimulation NewCvSimulation(int heavyEntries, int mediumEntries, int lightEntries)
        {
            var random = new System.Random(Seed);
            WarehouseLayout layout = new WarehouseGenerator(new GeneratorSettings(), random)
                .Generate(CvTestBlocks.BuildCv(heavyEntries, mediumEntries, lightEntries));
            var cvSimulation = new WarehouseSimulation(layout, new SimulationSettings(), random);
            cvSimulation.TryAddBot();
            return cvSimulation;
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
