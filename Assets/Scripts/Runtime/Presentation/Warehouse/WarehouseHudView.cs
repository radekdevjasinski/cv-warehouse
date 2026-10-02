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
        [SerializeField] private TMP_Text deliveredText;
        [SerializeField] private TMP_Text pointsText;
        [SerializeField] private Button addBotButton;
        [SerializeField] private Button removeBotButton;

        private WarehouseSimulation simulation;
        private int shownBots = NothingShown;
        private int shownDelivered = NothingShown;
        private int shownPoints = NothingShown;

        public void Show(WarehouseSimulation shownSimulation)
        {
            simulation = shownSimulation;
            Refresh();
        }

        private void OnEnable()
        {
            addBotButton.onClick.AddListener(AddBot);
            removeBotButton.onClick.AddListener(RemoveBot);
        }

        private void OnDisable()
        {
            addBotButton.onClick.RemoveListener(AddBot);
            removeBotButton.onClick.RemoveListener(RemoveBot);
        }

        private void LateUpdate()
        {
            if (simulation != null)
                Refresh();
        }

        private void AddBot()
        {
            simulation.TryAddBot();
        }

        private void RemoveBot()
        {
            simulation.TryRemoveBot();
        }

        private void Refresh()
        {
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

            if (shownPoints != simulation.Score.Points)
            {
                shownPoints = simulation.Score.Points;
                pointsText.SetText("Points {0}", shownPoints);
            }
        }
    }
}
