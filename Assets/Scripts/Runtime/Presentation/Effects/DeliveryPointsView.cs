using CvWarehouse.Core.Effects;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Presentation.Warehouse;
using TMPro;
using UnityEngine;

namespace CvWarehouse.Presentation.Effects
{
    public sealed class DeliveryPointsView : MonoBehaviour
    {
        private const string PointsFormat = "+{0}";
        private const int NothingShown = -1;

        [SerializeField] private PointsPopupConfig config;

        private readonly PointsPopupSettings settings = new PointsPopupSettings();
        private PointsPopupQueue queue;
        private TextMeshPro[] labels;
        private int[] shownPoints;
        private WarehouseSimulation simulation;

        public int VisiblePopupCount => queue.Count;

        public TextMeshPro Label(int index)
        {
            return labels[index];
        }

        public void Listen(WarehouseSimulation listenedSimulation)
        {
            Unsubscribe();
            simulation = listenedSimulation;
            if (isActiveAndEnabled)
                Subscribe();
        }

        private void Awake()
        {
            config.CopyTo(settings);
            queue = new PointsPopupQueue(settings, config.Capacity);
            labels = new TextMeshPro[config.Capacity];
            shownPoints = new int[config.Capacity];
            for (int index = 0; index < labels.Length; index++)
            {
                labels[index] = CreateLabel(index);
                shownPoints[index] = NothingShown;
            }
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void LateUpdate()
        {
            config.CopyTo(settings);
            queue.Advance(Time.deltaTime);
            for (int index = 0; index < labels.Length; index++)
            {
                if (index < queue.Count)
                    ShowPopup(index);
                else
                    HideLabel(index);
            }
        }

        private void Subscribe()
        {
            if (simulation != null)
                simulation.Unloaded += AddPopup;
        }

        private void Unsubscribe()
        {
            if (simulation != null)
                simulation.Unloaded -= AddPopup;
        }

        private void AddPopup(Bot bot, int points)
        {
            queue.Add(bot.CentreX, bot.CentreY + config.StartHeight, points);
        }

        private void ShowPopup(int index)
        {
            PointsPopup popup = queue[index];
            TextMeshPro label = labels[index];
            if (!label.gameObject.activeSelf)
                label.gameObject.SetActive(true);
            if (shownPoints[index] != popup.Points)
            {
                shownPoints[index] = popup.Points;
                label.SetText(PointsFormat, popup.Points);
            }

            label.transform.localPosition = new Vector3(popup.X, popup.Y + queue.RiseOf(index));
            Color color = config.Color;
            color.a *= queue.AlphaOf(index);
            label.color = color;
        }

        private void HideLabel(int index)
        {
            if (labels[index].gameObject.activeSelf)
                labels[index].gameObject.SetActive(false);
        }

        private TextMeshPro CreateLabel(int index)
        {
            var labelObject = new GameObject("Points Popup " + index);
            labelObject.transform.SetParent(transform, false);
            TextMeshPro label = labelObject.AddComponent<TextMeshPro>();
            label.font = config.Font;
            if (config.OutlinedMaterial != null)
                label.fontSharedMaterial = config.OutlinedMaterial;
            label.fontSize = config.FontSize;
            label.alignment = TextAlignmentOptions.Center;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.sortingOrder = DepthSorting.OverEverythingOrder;
            label.raycastTarget = false;
            labelObject.SetActive(false);
            return label;
        }
    }
}
