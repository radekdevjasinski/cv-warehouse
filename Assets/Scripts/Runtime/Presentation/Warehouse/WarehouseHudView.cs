using CvWarehouse.Core.Warehouse;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class WarehouseHudView : MonoBehaviour
    {
        private const int NothingShown = -1;

        [SerializeField] private TMP_Text pointsText;
        [SerializeField] private TMP_Text botsText;
        [SerializeField] private TMP_Text deliveredText;
        [SerializeField] private Button buyBotButton;
        [SerializeField] private TMP_Text buyBotLabel;
        [SerializeField] private Button removeBotButton;
        [SerializeField] private Button upgradeCarryButton;
        [SerializeField] private TMP_Text upgradeCarryLabel;

        private WarehouseSimulation simulation;
        private BotShop shop;
        private int shownPoints = NothingShown;
        private int shownBots = NothingShown;
        private int shownDelivered = NothingShown;
        private int shownBotPrice = NothingShown;
        private int shownCarryPrice = NothingShown;

        public void Show(WarehouseSimulation shownSimulation, BotShop shownShop)
        {
            simulation = shownSimulation;
            shop = shownShop;
            Refresh();
        }

        private void OnEnable()
        {
            buyBotButton.onClick.AddListener(BuyBot);
            removeBotButton.onClick.AddListener(RemoveBot);
            upgradeCarryButton.onClick.AddListener(UpgradeCarry);
        }

        private void OnDisable()
        {
            buyBotButton.onClick.RemoveListener(BuyBot);
            removeBotButton.onClick.RemoveListener(RemoveBot);
            upgradeCarryButton.onClick.RemoveListener(UpgradeCarry);
        }

        private void LateUpdate()
        {
            if (simulation != null)
                Refresh();
        }

        private void BuyBot()
        {
            shop.TryBuyBot();
        }

        private void RemoveBot()
        {
            simulation.TryRemoveBot();
        }

        private void UpgradeCarry()
        {
            shop.TryUpgradeCarry();
        }

        private void Refresh()
        {
            RefreshCounters();
            RefreshBuyBotButton();
            RefreshUpgradeCarryButton();
        }

        private void RefreshCounters()
        {
            if (shownPoints != shop.Balance)
            {
                shownPoints = shop.Balance;
                pointsText.SetText("{0} points", shownPoints);
            }

            if (shownBots != simulation.Bots.Count)
            {
                shownBots = simulation.Bots.Count;
                botsText.SetText("Bots {0}/{1}", shownBots, simulation.MaxBots);
            }

            if (shownDelivered != simulation.Score.DeliveredPieces)
            {
                shownDelivered = simulation.Score.DeliveredPieces;
                deliveredText.SetText("Delivered {0}/{1}", shownDelivered, simulation.Layout.TotalPieces);
            }
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

        private void RefreshUpgradeCarryButton()
        {
            SetInteractable(upgradeCarryButton, shop.CanUpgradeCarry);
            int carryPrice = shop.HasCarryUpgrade ? shop.CarryUpgradePrice : 0;
            if (shownCarryPrice == carryPrice)
                return;

            shownCarryPrice = carryPrice;
            if (!shop.HasCarryUpgrade)
                upgradeCarryLabel.SetText("Bots carry whole boxes");
            else if (simulation.CarrySize == CarrySize.Letter)
                upgradeCarryLabel.SetText("Carry words ({0})", carryPrice);
            else
                upgradeCarryLabel.SetText("Carry whole boxes ({0})", carryPrice);
        }

        private static void SetInteractable(Button button, bool isInteractable)
        {
            if (button.interactable != isInteractable)
                button.interactable = isInteractable;
        }
    }
}
