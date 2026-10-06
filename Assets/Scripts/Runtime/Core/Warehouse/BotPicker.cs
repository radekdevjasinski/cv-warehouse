using System.Collections.Generic;

namespace CvWarehouse.Core.Warehouse
{
    public static class BotPicker
    {
        public static bool TryPick(IReadOnlyList<Bot> bots, float pointX, float pointY, float radius, out Bot picked)
        {
            picked = null;
            float nearestSquared = radius * radius;
            for (int index = 0; index < bots.Count; index++)
            {
                Bot bot = bots[index];
                float deltaX = bot.CentreX - pointX;
                float deltaY = bot.CentreY - pointY;
                float distanceSquared = deltaX * deltaX + deltaY * deltaY;
                if (distanceSquared > nearestSquared || (picked != null && distanceSquared == nearestSquared))
                    continue;

                nearestSquared = distanceSquared;
                picked = bot;
            }

            return picked != null;
        }

        public static void PickInside(IReadOnlyList<Bot> bots, SelectionArea area, List<Bot> picked)
        {
            picked.Clear();
            for (int index = 0; index < bots.Count; index++)
                if (area.Contains(bots[index].CentreX, bots[index].CentreY))
                    picked.Add(bots[index]);
        }
    }
}
