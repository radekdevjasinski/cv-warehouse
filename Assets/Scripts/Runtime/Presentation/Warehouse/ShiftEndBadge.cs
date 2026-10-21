using CvWarehouse.Core.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class ShiftEndBadge : MonoBehaviour
    {
        [SerializeField] private RectTransform badge;
        [SerializeField] private WarehouseScreenSplit screenSplit;

        private WarehouseSimulation simulation;

        public bool IsShown => badge.gameObject.activeSelf;

        public void Show(WarehouseSimulation shownSimulation)
        {
            simulation = shownSimulation;
            Refresh();
        }

        private void Awake()
        {
            badge.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (simulation != null)
                Refresh();
        }

        private void Refresh()
        {
            bool isShiftOver = simulation.IsComplete || simulation.IsSkipped;
            bool isShown = isShiftOver && !screenSplit.IsWaybillExpanded;
            if (badge.gameObject.activeSelf != isShown)
                badge.gameObject.SetActive(isShown);
        }
    }
}
