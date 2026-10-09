namespace CvWarehouse.Core.Warehouse
{
    public sealed class ShopSettings
    {
        public int FreeBots { get; set; } = 3;

        public int FirstBotPricePermille { get; set; } = 15;

        public int BotPriceGrowthPercent { get; set; } = 125;

        public int MaxUpgradeLevel { get; set; } = 5;

        public int CarryUpgradePricePermille { get; set; } = 10;

        public int SpeedUpgradePricePermille { get; set; } = 10;

        public int UpgradePriceGrowthPercent { get; set; } = 150;
    }
}
