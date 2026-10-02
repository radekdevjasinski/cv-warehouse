using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class WarehouseBoxesView : MonoBehaviour
    {
        private const int SortingOrder = 1;
        private const int NothingShown = -1;

        [SerializeField] private Color lightColor = new Color(0.95f, 0.80f, 0.35f);
        [SerializeField] private Color mediumColor = new Color(0.90f, 0.55f, 0.25f);
        [SerializeField] private Color heavyColor = new Color(0.80f, 0.30f, 0.25f);
        [SerializeField] private float gap = 0.12f;
        [SerializeField] private float emptiestAlpha = 0.35f;

        private IReadOnlyList<Box> boxes;
        private SpriteRenderer[] boxRenderers;
        private int[] shownPieces;

        public int VisibleBoxCount { get; private set; }

        public void Show(WarehouseLayout layout, SquareShapeFactory shapes)
        {
            boxes = layout.Boxes;
            boxRenderers = new SpriteRenderer[boxes.Count];
            shownPieces = new int[boxes.Count];
            for (int index = 0; index < boxes.Count; index++)
            {
                boxRenderers[index] = CreateBox(boxes[index], shapes);
                shownPieces[index] = NothingShown;
            }

            Refresh();
        }

        private void LateUpdate()
        {
            if (boxes != null)
                Refresh();
        }

        private SpriteRenderer CreateBox(Box box, SquareShapeFactory shapes)
        {
            SpriteRenderer boxRenderer = shapes.CreateSquare("Box " + box.Id, transform, SortingOrder);
            Transform boxTransform = boxRenderer.transform;
            boxTransform.localPosition = new Vector3(box.Origin.X + box.Size.Width * 0.5f, box.Origin.Y + box.Size.Height * 0.5f);
            boxTransform.localScale = new Vector3(box.Size.Width - gap, box.Size.Height - gap, 1f);
            boxRenderer.enabled = false;
            return boxRenderer;
        }

        private void Refresh()
        {
            for (int index = 0; index < boxes.Count; index++)
            {
                Box box = boxes[index];
                if (shownPieces[index] == box.RemainingPieces)
                    continue;

                shownPieces[index] = box.RemainingPieces;
                RefreshBox(boxRenderers[index], box);
            }
        }

        private void RefreshBox(SpriteRenderer boxRenderer, Box box)
        {
            if (boxRenderer.enabled == box.IsEmpty)
                VisibleBoxCount += box.IsEmpty ? -1 : 1;

            boxRenderer.enabled = !box.IsEmpty;
            Color color = ColorOf(box.WeightClass);
            color.a = Mathf.Lerp(emptiestAlpha, 1f, (float)box.RemainingPieces / box.TotalPieces);
            boxRenderer.color = color;
        }

        private Color ColorOf(WeightClass weightClass)
        {
            switch (weightClass)
            {
                case WeightClass.Heavy:
                    return heavyColor;
                case WeightClass.Medium:
                    return mediumColor;
                default:
                    return lightColor;
            }
        }
    }
}
