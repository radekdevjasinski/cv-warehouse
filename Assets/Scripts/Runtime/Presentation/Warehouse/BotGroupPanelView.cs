using System.Collections.Generic;
using CvWarehouse.Core.Warehouse;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class BotGroupPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject window;
        [SerializeField] private UpgradeButtonLook upgradeLook;
        [SerializeField] private BotGroupUpgradeRow carryRow;
        [SerializeField] private BotGroupUpgradeRow speedRow;
        [SerializeField] private TMP_Text focusText;
        [SerializeField] private Button previousFocusButton;
        [SerializeField] private Button nextFocusButton;
        [SerializeField] private Button deleteButton;
        [SerializeField] private Button closeButton;

        private WarehouseSimulation simulation;
        private BotShop shop;
        private BotSelection selection;
        private BotFocus shownFocus;
        private bool isFocusShown;
        private bool isDifferentShown;

        public void Show(WarehouseSimulation shownSimulation, BotShop shownShop, BotSelection botSelection)
        {
            simulation = shownSimulation;
            shop = shownShop;
            selection = botSelection;
            Refresh();
        }

        private void OnEnable()
        {
            carryRow.Button.onClick.AddListener(UpgradeCarry);
            speedRow.Button.onClick.AddListener(UpgradeSpeed);
            previousFocusButton.onClick.AddListener(ShowPreviousFocus);
            nextFocusButton.onClick.AddListener(ShowNextFocus);
            deleteButton.onClick.AddListener(DeleteBots);
            closeButton.onClick.AddListener(Close);
        }

        private void OnDisable()
        {
            carryRow.Button.onClick.RemoveListener(UpgradeCarry);
            speedRow.Button.onClick.RemoveListener(UpgradeSpeed);
            previousFocusButton.onClick.RemoveListener(ShowPreviousFocus);
            nextFocusButton.onClick.RemoveListener(ShowNextFocus);
            deleteButton.onClick.RemoveListener(DeleteBots);
            closeButton.onClick.RemoveListener(Close);
        }

        private void LateUpdate()
        {
            if (selection != null)
                Refresh();
        }

        private void UpgradeCarry()
        {
            shop.TryUpgrade(selection.Bots, carryRow.Upgrade);
        }

        private void UpgradeSpeed()
        {
            shop.TryUpgrade(selection.Bots, speedRow.Upgrade);
        }

        private void ShowPreviousFocus()
        {
            SetFocusOfAll(BotGroup.PreviousFocus(selection.Bots));
        }

        private void ShowNextFocus()
        {
            SetFocusOfAll(BotGroup.NextFocus(selection.Bots));
        }

        private void SetFocusOfAll(BotFocus focus)
        {
            IReadOnlyList<Bot> selectedBots = selection.Bots;
            for (int index = 0; index < selectedBots.Count; index++)
                simulation.SetFocus(selectedBots[index], focus);
        }

        private void DeleteBots()
        {
            IReadOnlyList<Bot> selectedBots = selection.Bots;
            for (int index = 0; index < selectedBots.Count; index++)
                simulation.TryRemoveBot(selectedBots[index]);
            selection.Clear();
        }

        private void Close()
        {
            selection.Clear();
        }

        private void Refresh()
        {
            if (window.activeSelf != selection.HasManyBots)
                window.SetActive(selection.HasManyBots);
            if (!selection.HasManyBots)
                return;

            carryRow.Refresh(shop, selection.Bots, upgradeLook);
            speedRow.Refresh(shop, selection.Bots, upgradeLook);
            RefreshFocus();
            if (deleteButton.interactable != simulation.CanRemoveBot)
                deleteButton.interactable = simulation.CanRemoveBot;
        }

        private void RefreshFocus()
        {
            if (!BotGroup.HasSharedFocus(selection.Bots))
            {
                ShowDifferentFocus();
                return;
            }

            BotFocus focus = selection.Bots[0].Focus;
            if (isFocusShown && !isDifferentShown && shownFocus == focus)
                return;

            isFocusShown = true;
            isDifferentShown = false;
            shownFocus = focus;
            focusText.SetText(BotFocusNames.NameOf(focus));
        }

        private void ShowDifferentFocus()
        {
            if (isFocusShown && isDifferentShown)
                return;

            isFocusShown = true;
            isDifferentShown = true;
            focusText.SetText(BotFocusNames.Different);
        }
    }
}
