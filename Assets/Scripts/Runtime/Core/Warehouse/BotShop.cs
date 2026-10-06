using System;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class BotShop
    {
        private const int Permille = 1000;
        private const int Percent = 100;
        private const int LowestPrice = 1;

        private readonly WarehouseSimulation simulation;
        private readonly ShopSettings settings;
        private int spentPoints;

        public BotShop(WarehouseSimulation simulation, ShopSettings settings)
        {
            this.simulation = simulation;
            this.settings = settings;
        }

        public int Balance => simulation.Score.Points - spentPoints;

        public bool HasBotSlot => simulation.Bots.Count < simulation.MaxBots;

        public bool CanBuyBot => HasBotSlot && Balance >= BotPrice;

        public bool HasCarryUpgrade => simulation.CarrySize != CarrySize.Box;

        public bool CanUpgradeCarry => HasCarryUpgrade && Balance >= CarryUpgradePrice;

        public int BotPrice
        {
            get
            {
                int price = ShareOfAllPoints(settings.FirstBotPricePermille);
                int paidBots = Math.Max(0, simulation.Bots.Count - settings.FreeBots);
                for (int bot = 0; bot < paidBots; bot++)
                    price = Math.Max(price + 1, price * settings.BotPriceGrowthPercent / Percent);
                return price;
            }
        }

        public int CarryUpgradePrice => ShareOfAllPoints(simulation.CarrySize == CarrySize.Letter
            ? settings.WordCarryPricePermille
            : settings.BoxCarryPricePermille);

        public bool TryBuyBot()
        {
            int price = BotPrice;
            if (!CanBuyBot || !simulation.TryAddBot())
                return false;

            spentPoints += price;
            return true;
        }

        public bool TryUpgradeCarry()
        {
            if (!CanUpgradeCarry)
                return false;

            spentPoints += CarryUpgradePrice;
            simulation.SetCarrySize(simulation.CarrySize == CarrySize.Letter ? CarrySize.Word : CarrySize.Box);
            return true;
        }

        private int ShareOfAllPoints(int permille)
        {
            return Math.Max(LowestPrice, simulation.Score.MaxPoints * permille / Permille);
        }
    }
}
