using System;
using System.Collections.Generic;

namespace CvWarehouse.Core.Warehouse
{
    public static class BotGroup
    {
        public static int LowestLevel(IReadOnlyList<Bot> bots, BotUpgrade upgrade)
        {
            int lowest = int.MaxValue;
            for (int index = 0; index < bots.Count; index++)
                lowest = Math.Min(lowest, bots[index].LevelOf(upgrade));
            return bots.Count == 0 ? Bot.FirstLevel : lowest;
        }

        public static int HighestLevel(IReadOnlyList<Bot> bots, BotUpgrade upgrade)
        {
            int highest = Bot.FirstLevel;
            for (int index = 0; index < bots.Count; index++)
                highest = Math.Max(highest, bots[index].LevelOf(upgrade));
            return highest;
        }

        public static bool HasSharedFocus(IReadOnlyList<Bot> bots)
        {
            for (int index = 1; index < bots.Count; index++)
                if (bots[index].Focus != bots[0].Focus)
                    return false;
            return bots.Count > 0;
        }

        public static BotFocus NextFocus(IReadOnlyList<Bot> bots)
        {
            return HasSharedFocus(bots) ? BotFocusCycle.Next(bots[0].Focus) : BotFocusCycle.First;
        }

        public static BotFocus PreviousFocus(IReadOnlyList<Bot> bots)
        {
            return HasSharedFocus(bots) ? BotFocusCycle.Previous(bots[0].Focus) : BotFocusCycle.Last;
        }
    }
}
