using CvWarehouse.Core.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class BoxHighlightView : MonoBehaviour
    {
        private const int HighlightSortingOrder = 1;
        private const float BehindBoxes = 0.1f;

        [SerializeField] private Color highlightColor = new Color(1f, 0.85f, 0.2f);

        private BoxSelection selection;
        private GameObject highlight;
        private Box shownBox;

        public bool IsVisible => highlight != null && highlight.activeSelf;

        public Vector3 Position => highlight.transform.localPosition;

        public void Show(BoxSelection boxSelection, SquareShapeFactory shapes)
        {
            selection = boxSelection;
            SpriteRenderer highlightRenderer = shapes.CreateSquare("Box Highlight", transform, HighlightSortingOrder);
            highlightRenderer.color = highlightColor;
            highlight = highlightRenderer.gameObject;
            highlight.SetActive(false);
        }

        private void LateUpdate()
        {
            if (selection == null)
                return;

            if (highlight.activeSelf != selection.HasBox)
                highlight.SetActive(selection.HasBox);
            if (!selection.HasBox || shownBox == selection.SelectedBox)
                return;

            shownBox = selection.SelectedBox;
            Frame(shownBox);
        }

        private void Frame(Box box)
        {
            Transform highlightTransform = highlight.transform;
            highlightTransform.localPosition = new Vector3(
                box.Origin.X + box.Size.Width * 0.5f,
                box.Origin.Y + box.Size.Height * 0.5f,
                BehindBoxes);
            highlightTransform.localScale = new Vector3(box.Size.Width, box.Size.Height, 1f);
        }
    }
}
