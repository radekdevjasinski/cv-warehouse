using CvWarehouse.Core.Warehouse;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class WarehouseHudView : MonoBehaviour
    {
        private const int NothingShown = -1;

        [SerializeField] private TMP_Text botsText;
        [SerializeField] private Button buyBotButton;
        [SerializeField] private TMP_Text buyBotLabel;

        private WarehouseSimulation simulation;
        private BotShop shop;
        private int shownBots = NothingShown;
        private int shownBotPrice = NothingShown;

        public void Show(WarehouseSimulation shownSimulation, BotShop shownShop)
        {
            simulation = shownSimulation;
            shop = shownShop;
            Refresh();
        }

        private static void SetInteractable(Button button, bool isInteractable)
        {
            if (button.interactable != isInteractable)
                button.interactable = isInteractable;
        }

        private void OnEnable()
        {
            buyBotButton.onClick.AddListener(BuyBot);
        }

        private void OnDisable()
        {
            buyBotButton.onClick.RemoveListener(BuyBot);
        }

        private void LateUpdate()
        {
            if (simulation != null)
                Refresh();
        }

        private void BuyBot()
        {
            if (shop != null)
                shop.TryBuyBot();
        }

        private void Refresh()
        {
            RefreshBotCount();
            RefreshBuyBotButton();
        }

        private void RefreshBotCount()
        {
            if (shownBots == simulation.Bots.Count)
                return;

            shownBots = simulation.Bots.Count;
            botsText.SetText("{0}/{1}", shownBots, simulation.MaxBots);
        }

        private void RefreshBuyBotButton()
        {
            SetInteractable(buyBotButton, shop.CanBuyBot);
            int botPrice = shop.HasBotSlot ? shop.BotPrice : 0;
            if (shownBotPrice == botPrice)
                return;

            shownBotPrice = botPrice;
            if (shop.HasBotSlot)
                buyBotLabel.SetText("Buy bot ({0})", botPrice);
            else
                buyBotLabel.SetText("No free bot slots");
        }
    }
}
