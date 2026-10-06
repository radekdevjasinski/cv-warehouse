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

        public static bool HasCarryUpgrade(Bot bot)
        {
            return bot.CarrySize != CarrySize.Box;
        }

        public bool CanUpgradeCarry(Bot bot)
        {
            return HasCarryUpgrade(bot) && Balance >= CarryUpgradePrice(bot);
        }

        public int CarryUpgradePrice(Bot bot)
        {
            return ShareOfAllPoints(bot.CarrySize == CarrySize.Letter
                ? settings.WordCarryPricePermille
                : settings.BoxCarryPricePermille);
        }

        public bool TryBuyBot()
        {
            int price = BotPrice;
            if (!CanBuyBot || !simulation.TryAddBot())
                return false;

            spentPoints += price;
            return true;
        }

        public bool TryUpgradeCarry(Bot bot)
        {
            if (!CanUpgradeCarry(bot))
                return false;

            spentPoints += CarryUpgradePrice(bot);
            simulation.SetCarrySize(bot, bot.CarrySize == CarrySize.Letter ? CarrySize.Word : CarrySize.Box);
            return true;
        }

        private int ShareOfAllPoints(int permille)
        {
            return Math.Max(LowestPrice, simulation.Score.MaxPoints * permille / Permille);
        }
    }
}
