using UnityEngine;
using UnityEngine.EventSystems;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class SelectionClearArea : MonoBehaviour, IPointerClickHandler
    {
        private BotSelection botSelection;
        private BoxSelection boxSelection;

        public void Connect(BotSelection bots, BoxSelection box)
        {
            botSelection = bots;
            boxSelection = box;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (botSelection == null)
                return;

            botSelection.Clear();
            boxSelection.Clear();
        }
    }
}
