using System.Collections.Generic;
using CvWarehouse.Core.Warehouse;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class BotSelection
    {
        private readonly List<Bot> bots = new List<Bot>();

        public IReadOnlyList<Bot> Bots => bots;

        public int Count => bots.Count;

        public bool HasSingleBot => bots.Count == 1;

        public bool HasManyBots => bots.Count > 1;

        public Bot SingleBot => HasSingleBot ? bots[0] : null;

        public void Select(Bot bot)
        {
            bots.Clear();
            bots.Add(bot);
        }

        public void SelectAll(IReadOnlyList<Bot> selectedBots)
        {
            bots.Clear();
            for (int index = 0; index < selectedBots.Count; index++)
                bots.Add(selectedBots[index]);
        }

        public void Clear()
        {
            bots.Clear();
        }
    }
}
