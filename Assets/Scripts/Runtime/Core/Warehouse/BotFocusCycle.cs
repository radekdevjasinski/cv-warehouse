namespace CvWarehouse.Core.Warehouse
{
    public static class BotFocusCycle
    {
        public const BotFocus First = BotFocus.ClosestBox;
        public const BotFocus Last = BotFocus.SmallestBox;

        private const int FocusCount = 4;

        public static BotFocus Next(BotFocus focus)
        {
            return (BotFocus)(((int)focus + 1) % FocusCount);
        }

        public static BotFocus Previous(BotFocus focus)
        {
            return (BotFocus)(((int)focus + FocusCount - 1) % FocusCount);
        }
    }
}
