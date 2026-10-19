using System;
using System.Collections.Generic;

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

        public event Action<BotUpgrade> Upgraded;

        public int Balance => simulation.Score.Points - spentPoints;

        public bool HasBotSlot => simulation.Bots.Count < simulation.MaxBots;

        public bool CanBuyBot => HasBotSlot && Balance >= BotPrice;

        public int BotPrice
        {
            get
            {
                int paidBots = Math.Max(0, simulation.Bots.Count - settings.FreeBots);
                return Grow(ShareOfAllPoints(settings.FirstBotPricePermille), paidBots, settings.BotPriceGrowthPercent);
            }
        }

        public bool HasUpgrade(Bot bot, BotUpgrade upgrade)
        {
            return bot.LevelOf(upgrade) < settings.MaxUpgradeLevel;
        }

        public bool CanUpgrade(Bot bot, BotUpgrade upgrade)
        {
            return HasUpgrade(bot, upgrade) && Balance >= UpgradePrice(bot, upgrade);
        }

        public int UpgradePrice(Bot bot, BotUpgrade upgrade)
        {
            int firstPricePermille = upgrade == BotUpgrade.Speed
                ? settings.SpeedUpgradePricePermille
                : settings.CarryUpgradePricePermille;
            int boughtLevels = bot.LevelOf(upgrade) - Bot.FirstLevel;
            return Grow(ShareOfAllPoints(firstPricePermille), boughtLevels, settings.UpgradePriceGrowthPercent);
        }

        public int CountUpgradeable(IReadOnlyList<Bot> bots, BotUpgrade upgrade)
        {
            int upgradeable = 0;
            for (int index = 0; index < bots.Count; index++)
                if (HasUpgrade(bots[index], upgrade))
                    upgradeable++;
            return upgradeable;
        }

        public int UpgradePrice(IReadOnlyList<Bot> bots, BotUpgrade upgrade)
        {
            int price = 0;
            for (int index = 0; index < bots.Count; index++)
                if (HasUpgrade(bots[index], upgrade))
                    price += UpgradePrice(bots[index], upgrade);
            return price;
        }

        public bool CanUpgrade(IReadOnlyList<Bot> bots, BotUpgrade upgrade)
        {
            return CountUpgradeable(bots, upgrade) > 0 && Balance >= UpgradePrice(bots, upgrade);
        }

        public bool TryBuyBot()
        {
            int price = BotPrice;
            if (!CanBuyBot || !simulation.TryAddBot())
                return false;

            spentPoints += price;
            return true;
        }

        public bool TryUpgrade(Bot bot, BotUpgrade upgrade)
        {
            if (!TryBuyUpgrade(bot, upgrade))
                return false;

            Upgraded?.Invoke(upgrade);
            return true;
        }

        public bool TryUpgrade(IReadOnlyList<Bot> bots, BotUpgrade upgrade)
        {
            if (!CanUpgrade(bots, upgrade))
                return false;

            for (int index = 0; index < bots.Count; index++)
                TryBuyUpgrade(bots[index], upgrade);
            Upgraded?.Invoke(upgrade);
            return true;
        }

        private bool TryBuyUpgrade(Bot bot, BotUpgrade upgrade)
        {
            if (!CanUpgrade(bot, upgrade))
                return false;

            spentPoints += UpgradePrice(bot, upgrade);
            simulation.Upgrade(bot, upgrade);
            return true;
        }

        private static int Grow(int price, int steps, int growthPercent)
        {
            for (int step = 0; step < steps; step++)
                price = Math.Max(price + 1, price * growthPercent / Percent);
            return price;
        }

        private int ShareOfAllPoints(int permille)
        {
            return Math.Max(LowestPrice, simulation.Score.MaxPoints * permille / Permille);
        }
    }
}
