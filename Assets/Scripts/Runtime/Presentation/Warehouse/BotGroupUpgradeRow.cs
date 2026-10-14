using System;
using System.Collections.Generic;
using CvWarehouse.Core.Warehouse;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Warehouse
{
    [Serializable]
    public sealed class BotGroupUpgradeRow
    {
        private const int NothingShown = -1;

        [SerializeField] private BotUpgrade upgrade;
        [SerializeField] private TMP_Text levelsText;
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text buttonLabel;

        private int shownLowest = NothingShown;
        private int shownHighest = NothingShown;
        private int shownCount = NothingShown;
        private int shownPrice = NothingShown;

        public Button Button => button;

        public BotUpgrade Upgrade => upgrade;

        public void Refresh(BotShop shop, IReadOnlyList<Bot> bots)
        {
            RefreshLevels(BotGroup.LowestLevel(bots, upgrade), BotGroup.HighestLevel(bots, upgrade));
            RefreshButton(shop, bots);
        }

        private void RefreshLevels(int lowest, int highest)
        {
            if (shownLowest == lowest && shownHighest == highest)
                return;

            shownLowest = lowest;
            shownHighest = highest;
            if (lowest == highest)
                levelsText.SetText(upgrade == BotUpgrade.Speed ? "Speed: {0}" : "Words per haul: {0}", lowest);
            else
                levelsText.SetText(upgrade == BotUpgrade.Speed ? "Speed: {0}-{1}" : "Words per haul: {0}-{1}", lowest, highest);
        }

        private void RefreshButton(BotShop shop, IReadOnlyList<Bot> bots)
        {
            int count = shop.CountUpgradeable(bots, upgrade);
            if (button.gameObject.activeSelf != count > 0)
                button.gameObject.SetActive(count > 0);
            if (count == 0)
                return;

            bool canUpgrade = shop.CanUpgrade(bots, upgrade);
            if (button.interactable != canUpgrade)
                button.interactable = canUpgrade;

            int price = shop.UpgradePrice(bots, upgrade);
            if (shownCount == count && shownPrice == price)
                return;

            shownCount = count;
            shownPrice = price;
            buttonLabel.SetText(upgrade == BotUpgrade.Speed ? "Speed x{0} ({1})" : "Words x{0} ({1})", count, price);
        }
    }
}
