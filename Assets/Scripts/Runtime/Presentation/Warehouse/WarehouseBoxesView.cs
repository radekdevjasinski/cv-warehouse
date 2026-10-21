using System.Collections.Generic;
using CvWarehouse.Core.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class WarehouseBoxesView : MonoBehaviour
    {
        private const int BoxLayer = 0;
        private const int NothingShown = -1;
        private const int DiscardedShown = -2;
        private const float BottomEdgeAboveOrigin = 1f / DepthSorting.PixelsPerUnit;

        [SerializeField] private BoxSkin skin;

        private IReadOnlyList<Box> boxes;
        private SpriteRenderer[] boxRenderers;
        private int[] shownPieces;

        public Sprite ShownSprite(int index)
        {
            return boxRenderers[index].sprite;
        }

        public int SortingOrder(int index)
        {
            return boxRenderers[index].sortingOrder;
        }

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
            SpriteRenderer boxRenderer = shapes.CreateSquare("Box " + box.Id, transform, DepthSorting.OnTheFloorOrder);
            boxRenderer.transform.localPosition = new Vector3(box.Origin.X + box.Size.Width * 0.5f, box.Origin.Y + box.Size.Height * 0.5f);
            return boxRenderer;
        }

        private void Refresh()
        {
            for (int index = 0; index < boxes.Count; index++)
            {
                Box box = boxes[index];
                int shownState = box.IsDiscarded ? DiscardedShown : box.RemainingPieces;
                if (shownPieces[index] == shownState)
                    continue;

                shownPieces[index] = shownState;
                RefreshBox(boxRenderers[index], box);
            }
        }

        private void RefreshBox(SpriteRenderer boxRenderer, Box box)
        {
            boxRenderer.enabled = !box.IsDiscarded;
            boxRenderer.sprite = skin.LookOf(box).SpriteOf(box);
            boxRenderer.sortingOrder = box.IsEmpty
                ? DepthSorting.OnTheFloorOrder
                : DepthSorting.OrderAt(box.Origin.Y + BottomEdgeAboveOrigin, BoxLayer);
        }
    }
}
