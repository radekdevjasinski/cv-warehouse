namespace CvWarehouse.Core.Warehouse
{
    public static class BotFocusCycle
    {
        private const int FocusCount = 2;

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
