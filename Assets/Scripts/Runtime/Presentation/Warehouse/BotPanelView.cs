using CvWarehouse.Core.Warehouse;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class BotPanelView : MonoBehaviour
    {
        private const int NothingShown = -1;

        [SerializeField] private GameObject window;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text carryText;
        [SerializeField] private Button upgradeCarryButton;
        [SerializeField] private TMP_Text upgradeCarryLabel;
        [SerializeField] private TMP_Text focusText;
        [SerializeField] private Button previousFocusButton;
        [SerializeField] private Button nextFocusButton;
        [SerializeField] private Button deleteButton;
        [SerializeField] private Button closeButton;

        private WarehouseSimulation simulation;
        private BotShop shop;
        private BotSelection selection;
        private Bot shownBot;
        private CarrySize shownCarrySize;
        private BotFocus shownFocus;
        private bool isFocusShown;
        private int shownCarryPrice = NothingShown;

        public void Show(WarehouseSimulation shownSimulation, BotShop shownShop, BotSelection botSelection)
        {
            simulation = shownSimulation;
            shop = shownShop;
            selection = botSelection;
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

        private void OnEnable()
        {
            upgradeCarryButton.onClick.AddListener(UpgradeCarry);
            previousFocusButton.onClick.AddListener(ShowPreviousFocus);
            nextFocusButton.onClick.AddListener(ShowNextFocus);
            deleteButton.onClick.AddListener(DeleteBot);
            closeButton.onClick.AddListener(Close);
        }

        private void OnDisable()
        {
            upgradeCarryButton.onClick.RemoveListener(UpgradeCarry);
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
                shop.TryUpgradeCarry(selection.SingleBot);
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

        private void Refresh()
        {
            SetActive(window, selection.HasSingleBot);
            if (!selection.HasSingleBot)
                return;

            Bot bot = selection.SingleBot;
            RefreshName(bot);
            RefreshCarry(bot);
            RefreshFocus(bot);
            SetInteractable(deleteButton, simulation.CanRemoveBot);
        }

        private void RefreshFocus(Bot bot)
        {
            if (isFocusShown && shownFocus == bot.Focus)
                return;

            isFocusShown = true;
            shownFocus = bot.Focus;
            focusText.SetText(bot.Focus == BotFocus.BiggestBox ? "Biggest box" : "Closest box");
        }

        private void RefreshName(Bot bot)
        {
            if (shownBot == bot)
                return;

            shownBot = bot;
            shownCarryPrice = NothingShown;
            isFocusShown = false;
            nameText.SetText("Bot {0}", bot.Id + 1);
        }

        private void RefreshCarry(Bot bot)
        {
            bool hasUpgrade = BotShop.HasCarryUpgrade(bot);
            SetActive(upgradeCarryButton.gameObject, hasUpgrade);
            SetInteractable(upgradeCarryButton, shop.CanUpgradeCarry(bot));
            int carryPrice = hasUpgrade ? shop.CarryUpgradePrice(bot) : 0;
            if (shownCarryPrice == carryPrice && shownCarrySize == bot.CarrySize)
                return;

            shownCarryPrice = carryPrice;
            shownCarrySize = bot.CarrySize;
            ShowCarry(bot.CarrySize, carryPrice);
        }

        private void ShowCarry(CarrySize carrySize, int nextPrice)
        {
            switch (carrySize)
            {
                case CarrySize.Box:
                    carryText.SetText("Carries whole boxes");
                    break;
                case CarrySize.Word:
                    carryText.SetText("Carries words");
                    upgradeCarryLabel.SetText("Carry whole boxes ({0})", nextPrice);
                    break;
                default:
                    carryText.SetText("Carries letters");
                    upgradeCarryLabel.SetText("Carry words ({0})", nextPrice);
                    break;
            }
        }
    }
}
