using CvWarehouse.Core.Warehouse;
using TMPro;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class PointsView : MonoBehaviour
    {
        private const int NothingShown = -1;

        [SerializeField] private TMP_Text pointsText;

        private BotShop shop;
        private int shownPoints = NothingShown;

        public void Show(BotShop shownShop)
        {
            shop = shownShop;
            Refresh();
        }

        private void LateUpdate()
        {
            if (shop != null)
                Refresh();
        }

        private void Refresh()
        {
            if (shownPoints == shop.Balance)
                return;

            shownPoints = shop.Balance;
            pointsText.SetText("{0}", shownPoints);
        }
    }
}
