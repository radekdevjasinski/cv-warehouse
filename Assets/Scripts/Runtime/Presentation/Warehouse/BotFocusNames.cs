using CvWarehouse.Core.Warehouse;

namespace CvWarehouse.Presentation.Warehouse
{
    public static class BotFocusNames
    {
        public const string Different = "Different";

        public static string NameOf(BotFocus focus)
        {
            switch (focus)
            {
                case BotFocus.BiggestBox:
                    return "Biggest box";
                case BotFocus.SmallestBox:
                    return "Smallest box";
                default:
                    return "Closest box";
            }
        }
    }
}
