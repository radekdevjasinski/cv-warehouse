namespace CvWarehouse.Core.Warehouse
{
    public sealed class ShopSettings
    {
        public int FreeBots { get; set; } = 3;

        public int FirstBotPricePermille { get; set; } = 15;

        public int BotPriceGrowthPercent { get; set; } = 125;

        public int WordCarryPricePermille { get; set; } = 30;

        public int BoxCarryPricePermille { get; set; } = 120;
    }
}
