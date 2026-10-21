using CvWarehouse.Core.Warehouse;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class ShiftStatusView : MonoBehaviour
    {
        private const string OnShiftLabel = "ON SHIFT";
        private const string PausedLabel = "PAUSED";
        private const string ShippedLabel = "SHIPPED";

        [SerializeField] private TMP_Text label;
        [SerializeField] private Image lamp;
        [SerializeField] private Color onShiftColor = new Color32(0x7b, 0xd3, 0x89, 0xff);
        [SerializeField] private Color pausedColor = new Color32(0xe0, 0x70, 0x70, 0xff);
        [SerializeField] private Color shippedColor = new Color32(0xe8, 0xc5, 0x47, 0xff);

        private WarehouseSimulation simulation;
        private string shownLabel;

        public void Show(WarehouseSimulation shownSimulation)
        {
            simulation = shownSimulation;
            Refresh();
        }

        private void LateUpdate()
        {
            if (simulation != null)
                Refresh();
        }

        private void Refresh()
        {
            if (simulation.IsComplete)
                Show(ShippedLabel, shippedColor);
            else if (simulation.IsPaused || simulation.IsHeld)
                Show(PausedLabel, pausedColor);
            else
                Show(OnShiftLabel, onShiftColor);
        }

        private void Show(string status, Color color)
        {
            if (ReferenceEquals(shownLabel, status))
                return;

            shownLabel = status;
            label.SetText(status);
            lamp.color = color;
        }
    }
}
