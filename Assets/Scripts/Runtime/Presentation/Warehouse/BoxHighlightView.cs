using CvWarehouse.Core.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class BoxHighlightView : MonoBehaviour
    {
        [SerializeField] private BoxSkin skin;

        private BoxSelection selection;
        private SpriteRenderer frame;
        private GameObject highlight;
        private Box shownBox;

        public bool IsVisible => highlight != null && highlight.activeSelf;

        public Vector3 Position => highlight.transform.localPosition;

        public Sprite FrameSprite => frame.sprite;

        public void Show(BoxSelection boxSelection, SquareShapeFactory shapes)
        {
            selection = boxSelection;
            frame = shapes.CreateSquare("Box Highlight", transform, DepthSorting.OverEverythingOrder);
            highlight = frame.gameObject;
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
            frame.sprite = skin.LookOf(box.WeightClass).SelectionFrame;
            highlight.transform.localPosition = new Vector3(
                box.Origin.X + box.Size.Width * 0.5f,
                box.Origin.Y + box.Size.Height * 0.5f);
        }
    }
}
