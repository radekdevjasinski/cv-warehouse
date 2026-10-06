using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class WarehouseScreenSplit : MonoBehaviour
    {
        private const string ExpandLabel = "Expand";
        private const string CollapseLabel = "Collapse";

        [SerializeField] private Camera warehouseCamera;
        [SerializeField] private RectTransform waybillPanel;
        [SerializeField] private RectTransform warehouseHudArea;
        [SerializeField, Range(0f, 1f)] private float waybillWidthFraction = 0.38f;
        [SerializeField] private Button expandButton;
        [SerializeField] private TMP_Text expandButtonLabel;
        [SerializeField] private GameObject[] warehouseOnlyObjects;

        public bool IsWaybillExpanded { get; private set; }

        private void Awake()
        {
            warehouseCamera.rect = new Rect(0f, 0f, 1f - waybillWidthFraction, 1f);
            warehouseHudArea.anchorMin = Vector2.zero;
            warehouseHudArea.anchorMax = new Vector2(1f - waybillWidthFraction, 1f);
            Apply();
        }

        private void OnEnable()
        {
            expandButton.onClick.AddListener(ToggleWaybill);
        }

        private void OnDisable()
        {
            expandButton.onClick.RemoveListener(ToggleWaybill);
        }

        private void ToggleWaybill()
        {
            IsWaybillExpanded = !IsWaybillExpanded;
            Apply();
        }

        private void Apply()
        {
            float waybillLeftEdge = IsWaybillExpanded ? 0f : 1f - waybillWidthFraction;
            waybillPanel.anchorMin = new Vector2(waybillLeftEdge, 0f);
            waybillPanel.anchorMax = Vector2.one;
            expandButtonLabel.SetText(IsWaybillExpanded ? CollapseLabel : ExpandLabel);
            foreach (GameObject warehouseOnlyObject in warehouseOnlyObjects)
                warehouseOnlyObject.SetActive(!IsWaybillExpanded);
        }
    }
}
