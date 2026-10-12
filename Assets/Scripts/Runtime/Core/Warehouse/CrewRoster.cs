using System.Collections.Generic;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class CrewRoster
    {
        private const string UnnamedPrefix = "BOT ";

        private readonly IReadOnlyList<string> names;

        public CrewRoster(IReadOnlyList<string> names)
        {
            this.names = names;
        }

        public int FirstFreeSlot(IReadOnlyList<Bot> bots)
        {
            int slot = 0;
            while (IsTaken(bots, slot))
                slot++;
            return slot;
        }

        public string NameOf(int slot)
        {
            return slot < names.Count ? names[slot] : UnnamedPrefix + (slot + 1);
        }

        private static bool IsTaken(IReadOnlyList<Bot> bots, int slot)
        {
            for (int index = 0; index < bots.Count; index++)
                if (bots[index].CrewSlot == slot)
                    return true;
            return false;
        }
    }
}
