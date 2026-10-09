using CvWarehouse.Core.Warehouse;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class BotPanelView : MonoBehaviour
    {
        private const int NothingShown = -1;
        private const int UpgradeCount = 2;

        [SerializeField] private GameObject window;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text carryText;
        [SerializeField] private Button upgradeCarryButton;
        [SerializeField] private TMP_Text upgradeCarryLabel;
        [SerializeField] private TMP_Text speedText;
        [SerializeField] private Button upgradeSpeedButton;
        [SerializeField] private TMP_Text upgradeSpeedLabel;
        [SerializeField] private TMP_Text focusText;
        [SerializeField] private Button previousFocusButton;
        [SerializeField] private Button nextFocusButton;
        [SerializeField] private Button deleteButton;
        [SerializeField] private Button closeButton;

        private readonly int[] shownLevels = new int[UpgradeCount];
        private readonly int[] shownPrices = new int[UpgradeCount];
        private WarehouseSimulation simulation;
        private BotShop shop;
        private BotSelection selection;
        private Bot shownBot;
        private BotFocus shownFocus;
        private bool isFocusShown;

        public void Show(WarehouseSimulation shownSimulation, BotShop shownShop, BotSelection botSelection)
        {
            simulation = shownSimulation;
            shop = shownShop;
            selection = botSelection;
            ForgetShownUpgrades();
            Refresh();
        }

        private static void SetActive(GameObject target, bool isActive)
        {
            if (target.activeSelf != isActive)
                target.SetActive(isActive);
        }

        private static void SetInteractable(Button button, bool isInteractable)
        {
            if (button.interactable != isInteractable)
                button.interactable = isInteractable;
        }

        private static string NameOf(BotFocus focus)
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

        private void OnEnable()
        {
            upgradeCarryButton.onClick.AddListener(UpgradeCarry);
            upgradeSpeedButton.onClick.AddListener(UpgradeSpeed);
            previousFocusButton.onClick.AddListener(ShowPreviousFocus);
            nextFocusButton.onClick.AddListener(ShowNextFocus);
            deleteButton.onClick.AddListener(DeleteBot);
            closeButton.onClick.AddListener(Close);
        }

        private void OnDisable()
        {
            upgradeCarryButton.onClick.RemoveListener(UpgradeCarry);
            upgradeSpeedButton.onClick.RemoveListener(UpgradeSpeed);
            previousFocusButton.onClick.RemoveListener(ShowPreviousFocus);
            nextFocusButton.onClick.RemoveListener(ShowNextFocus);
            deleteButton.onClick.RemoveListener(DeleteBot);
            closeButton.onClick.RemoveListener(Close);
        }

        private void LateUpdate()
        {
            if (selection != null)
                Refresh();
        }

        private void UpgradeCarry()
        {
            if (selection.HasSingleBot)
                shop.TryUpgrade(selection.SingleBot, BotUpgrade.Carry);
        }

        private void UpgradeSpeed()
        {
            if (selection.HasSingleBot)
                shop.TryUpgrade(selection.SingleBot, BotUpgrade.Speed);
        }

        private void ShowPreviousFocus()
        {
            if (selection.HasSingleBot)
                simulation.SetFocus(selection.SingleBot, BotFocusCycle.Previous(selection.SingleBot.Focus));
        }

        private void ShowNextFocus()
        {
            if (selection.HasSingleBot)
                simulation.SetFocus(selection.SingleBot, BotFocusCycle.Next(selection.SingleBot.Focus));
        }

        private void DeleteBot()
        {
            if (selection.HasSingleBot && simulation.TryRemoveBot(selection.SingleBot))
                selection.Clear();
        }

        private void Close()
        {
            selection.Clear();
        }

        private void ForgetShownUpgrades()
        {
            for (int upgrade = 0; upgrade < UpgradeCount; upgrade++)
            {
                shownLevels[upgrade] = NothingShown;
                shownPrices[upgrade] = NothingShown;
            }
        }

        private void Refresh()
        {
            SetActive(window, selection.HasSingleBot);
            if (!selection.HasSingleBot)
                return;

            Bot bot = selection.SingleBot;
            RefreshName(bot);
            RefreshCarry(bot);
            RefreshSpeed(bot);
            RefreshFocus(bot);
            SetInteractable(deleteButton, simulation.CanRemoveBot);
        }

        private void RefreshName(Bot bot)
        {
            if (shownBot == bot)
                return;

            shownBot = bot;
            isFocusShown = false;
            ForgetShownUpgrades();
            nameText.SetText("Bot {0}", bot.Id + 1);
        }

        private void RefreshCarry(Bot bot)
        {
            if (HasNewLevel(bot, BotUpgrade.Carry))
                carryText.SetText("Words per haul: {0}", bot.CarryLevel);
            if (RefreshUpgradeButton(bot, BotUpgrade.Carry, upgradeCarryButton))
                upgradeCarryLabel.SetText("More words ({0})", shop.UpgradePrice(bot, BotUpgrade.Carry));
        }

        private void RefreshSpeed(Bot bot)
        {
            if (HasNewLevel(bot, BotUpgrade.Speed))
                speedText.SetText("Speed: {0}", bot.SpeedLevel);
            if (RefreshUpgradeButton(bot, BotUpgrade.Speed, upgradeSpeedButton))
                upgradeSpeedLabel.SetText("More speed ({0})", shop.UpgradePrice(bot, BotUpgrade.Speed));
        }

        private bool HasNewLevel(Bot bot, BotUpgrade upgrade)
        {
            int level = bot.LevelOf(upgrade);
            if (shownLevels[(int)upgrade] == level)
                return false;

            shownLevels[(int)upgrade] = level;
            return true;
        }

        private bool RefreshUpgradeButton(Bot bot, BotUpgrade upgrade, Button button)
        {
            bool hasUpgrade = shop.HasUpgrade(bot, upgrade);
            SetActive(button.gameObject, hasUpgrade);
            SetInteractable(button, shop.CanUpgrade(bot, upgrade));
            int price = hasUpgrade ? shop.UpgradePrice(bot, upgrade) : NothingShown;
            if (!hasUpgrade || shownPrices[(int)upgrade] == price)
                return false;

            shownPrices[(int)upgrade] = price;
            return true;
        }

        private void RefreshFocus(Bot bot)
        {
            if (isFocusShown && shownFocus == bot.Focus)
                return;

            isFocusShown = true;
            shownFocus = bot.Focus;
            focusText.SetText(NameOf(bot.Focus));
        }
    }
}
