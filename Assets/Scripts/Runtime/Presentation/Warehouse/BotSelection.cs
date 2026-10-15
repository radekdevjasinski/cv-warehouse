using System;
using System.Collections.Generic;
using CvWarehouse.Core.Warehouse;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class BotSelection
    {
        private readonly List<Bot> bots = new List<Bot>();
        private readonly List<Bot> addedBots = new List<Bot>();

        public event Action<IReadOnlyList<Bot>> BotsSelected;

        public IReadOnlyList<Bot> Bots => bots;

        public int Count => bots.Count;

        public bool HasSingleBot => bots.Count == 1;

        public bool HasManyBots => bots.Count > 1;

        public Bot SingleBot => HasSingleBot ? bots[0] : null;

        public bool Contains(Bot bot)
        {
            return bots.Contains(bot);
        }

        public void Select(Bot bot)
        {
            bots.Clear();
            bots.Add(bot);
            AnnounceSelected(bot);
        }

        public void Toggle(Bot bot)
        {
            if (bots.Remove(bot))
                return;

            bots.Add(bot);
            AnnounceSelected(bot);
        }

        public void SelectAll(IReadOnlyList<Bot> selectedBots)
        {
            bots.Clear();
            for (int index = 0; index < selectedBots.Count; index++)
                bots.Add(selectedBots[index]);
            if (bots.Count > 0)
                BotsSelected?.Invoke(bots);
        }

        public void Clear()
        {
            bots.Clear();
        }

        private void AnnounceSelected(Bot bot)
        {
            addedBots.Clear();
            addedBots.Add(bot);
            BotsSelected?.Invoke(addedBots);
        }
    }
}
