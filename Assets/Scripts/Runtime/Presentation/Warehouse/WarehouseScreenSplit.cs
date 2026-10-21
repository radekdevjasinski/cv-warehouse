using CvWarehouse.Presentation.Cv;
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
        [SerializeField] private RectTransform topBar;
        [SerializeField] private RectTransform warehouseDock;
        [SerializeField, Range(0f, 1f)] private float waybillWidthFraction = 0.38f;
        [SerializeField, Range(0f, 0.5f)] private float topBarHeightFraction = 0.075f;
        [SerializeField, Range(0f, 0.5f)] private float dockHeightFraction = 0.25f;
        [SerializeField] private Button expandButton;
        [SerializeField] private TMP_Text expandButtonLabel;
        [SerializeField] private GameObject[] warehouseOnlyObjects;
        [SerializeField] private CvAutoScroll waybillAutoScroll;

        public bool IsWaybillExpanded { get; private set; }

        public void ExpandWaybill()
        {
            if (!IsWaybillExpanded)
                ToggleWaybill();
        }

        public void CollapseWaybill()
        {
            if (IsWaybillExpanded)
                ToggleWaybill();
        }

        private void Awake()
        {
            float warehouseWidth = 1f - waybillWidthFraction;
            float contentTop = 1f - topBarHeightFraction;
            warehouseCamera.rect = new Rect(0f, dockHeightFraction, warehouseWidth, contentTop - dockHeightFraction);
            warehouseHudArea.anchorMin = new Vector2(0f, dockHeightFraction);
            warehouseHudArea.anchorMax = new Vector2(warehouseWidth, contentTop);
            warehouseDock.anchorMin = Vector2.zero;
            warehouseDock.anchorMax = new Vector2(warehouseWidth, dockHeightFraction);
            topBar.anchorMin = new Vector2(0f, contentTop);
            topBar.anchorMax = Vector2.one;
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
            waybillPanel.anchorMax = new Vector2(1f, 1f - topBarHeightFraction);
            expandButtonLabel.SetText(IsWaybillExpanded ? CollapseLabel : ExpandLabel);
            waybillAutoScroll.enabled = !IsWaybillExpanded;
            foreach (GameObject warehouseOnlyObject in warehouseOnlyObjects)
                warehouseOnlyObject.SetActive(!IsWaybillExpanded);
        }
    }
}
