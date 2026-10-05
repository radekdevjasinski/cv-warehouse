using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Cv
{
    public sealed class FlowLayoutGroup : LayoutGroup
    {
        private const int HorizontalAxis = 0;
        private const int VerticalAxis = 1;

        [SerializeField] private Vector2 spacing = new Vector2(20f, 4f);

        private float InnerWidth => rectTransform.rect.width - padding.horizontal;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            SetLayoutInputForAxis(padding.horizontal, padding.horizontal, -1f, HorizontalAxis);
        }

        public override void CalculateLayoutInputVertical()
        {
            float height = ArrangeRows(false) + padding.vertical;
            SetLayoutInputForAxis(height, height, -1f, VerticalAxis);
        }

        public override void SetLayoutHorizontal()
        {
            ArrangeRows(true);
        }

        public override void SetLayoutVertical()
        {
            ArrangeRows(true);
        }

        private float ArrangeRows(bool placesChildren)
        {
            float rowTop = 0f;
            int rowStart = 0;
            while (rowStart < rectChildren.Count)
            {
                int rowEnd = FindRowEnd(rowStart);
                float rowHeight = MeasureRowHeight(rowStart, rowEnd);
                if (placesChildren)
                    PlaceRow(rowStart, rowEnd, padding.top + rowTop);

                rowTop += rowHeight + spacing.y;
                rowStart = rowEnd;
            }

            return Mathf.Max(0f, rowTop - spacing.y);
        }

        private int FindRowEnd(int rowStart)
        {
            float rowWidth = WidthOf(rowStart);
            int rowEnd = rowStart + 1;
            while (rowEnd < rectChildren.Count && rowWidth + spacing.x + WidthOf(rowEnd) <= InnerWidth)
            {
                rowWidth += spacing.x + WidthOf(rowEnd);
                rowEnd++;
            }

            return rowEnd;
        }

        private float MeasureRowWidth(int rowStart, int rowEnd)
        {
            float rowWidth = spacing.x * (rowEnd - rowStart - 1);
            for (int index = rowStart; index < rowEnd; index++)
                rowWidth += WidthOf(index);
            return rowWidth;
        }

        private float MeasureRowHeight(int rowStart, int rowEnd)
        {
            float rowHeight = 0f;
            for (int index = rowStart; index < rowEnd; index++)
                rowHeight = Mathf.Max(rowHeight, LayoutUtility.GetPreferredSize(rectChildren[index], VerticalAxis));
            return rowHeight;
        }

        private void PlaceRow(int rowStart, int rowEnd, float top)
        {
            float freeWidth = InnerWidth - MeasureRowWidth(rowStart, rowEnd);
            float left = padding.left + freeWidth * GetAlignmentOnAxis(HorizontalAxis);
            for (int index = rowStart; index < rowEnd; index++)
            {
                RectTransform child = rectChildren[index];
                float width = WidthOf(index);
                SetChildAlongAxis(child, HorizontalAxis, left, width);
                SetChildAlongAxis(child, VerticalAxis, top, LayoutUtility.GetPreferredSize(child, VerticalAxis));
                left += width + spacing.x;
            }
        }

        private float WidthOf(int childIndex)
        {
            return Mathf.Min(LayoutUtility.GetPreferredSize(rectChildren[childIndex], HorizontalAxis), InnerWidth);
        }
    }
}
