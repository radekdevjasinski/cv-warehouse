using CvWarehouse.Core.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class SelectionBoxView : MonoBehaviour
    {
        private const int BoxSortingOrder = 4;

        [SerializeField] private Color boxColor = new Color(1f, 0.85f, 0.2f, 0.25f);

        private GameObject box;

        public bool IsVisible => box != null && box.activeSelf;

        public Vector2 Size => box.transform.localScale;

        public void Show(SquareShapeFactory shapes)
        {
            SpriteRenderer boxRenderer = shapes.CreateSquare("Selection Box", transform, BoxSortingOrder);
            boxRenderer.color = boxColor;
            box = boxRenderer.gameObject;
            box.SetActive(false);
        }

        public void Draw(SelectionArea area)
        {
            box.transform.localPosition = new Vector3(area.MinX + area.Width * 0.5f, area.MinY + area.Height * 0.5f);
            box.transform.localScale = new Vector3(area.Width, area.Height, 1f);
            if (!box.activeSelf)
                box.SetActive(true);
        }

        public void Hide()
        {
            if (box.activeSelf)
                box.SetActive(false);
        }
    }
}
