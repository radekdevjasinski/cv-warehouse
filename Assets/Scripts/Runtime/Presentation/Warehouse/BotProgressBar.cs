using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class BotProgressBar
    {
        private const float Pixel = 1f / DepthSorting.PixelsPerUnit;
        private const int FillPixels = 10;
        private const int FillHeightPixels = 2;
        private const int BorderPixels = 1;
        private const float CentreBelowBot = 8f * Pixel;
        private const int FrameOrder = DepthSorting.OverEverythingOrder + 1;
        private const int FillOrder = FrameOrder + 1;

        private readonly SpriteRenderer frame;
        private readonly SpriteRenderer fill;

        public BotProgressBar(Transform parent, BotSkin skin, SquareShapeFactory shapes)
        {
            frame = shapes.CreateSquare("Progress Bar", parent, FrameOrder);
            frame.color = skin.ProgressFrameColor;
            frame.transform.localPosition = new Vector3(0f, -CentreBelowBot);
            frame.transform.localScale = new Vector3((FillPixels + 2 * BorderPixels) * Pixel, (FillHeightPixels + 2 * BorderPixels) * Pixel, 1f);
            fill = shapes.CreateSquare("Progress Fill", parent, FillOrder);
            fill.color = skin.ProgressFillColor;
            frame.gameObject.SetActive(false);
            fill.gameObject.SetActive(false);
        }

        public bool IsVisible => frame.gameObject.activeSelf;

        public int FilledPixels { get; private set; }

        public void Show(float progress)
        {
            bool isVisible = progress > 0f;
            SetActive(frame.gameObject, isVisible);
            if (!isVisible)
            {
                SetActive(fill.gameObject, false);
                FilledPixels = 0;
                return;
            }

            int filledPixels = Mathf.Clamp(Mathf.FloorToInt(progress * FillPixels), 0, FillPixels);
            SetActive(fill.gameObject, filledPixels > 0);
            if (filledPixels == FilledPixels)
                return;

            FilledPixels = filledPixels;
            float left = -FillPixels * Pixel / 2f;
            fill.transform.localPosition = new Vector3(left + filledPixels * Pixel / 2f, -CentreBelowBot);
            fill.transform.localScale = new Vector3(filledPixels * Pixel, FillHeightPixels * Pixel, 1f);
        }

        private static void SetActive(GameObject target, bool isActive)
        {
            if (target.activeSelf != isActive)
                target.SetActive(isActive);
        }
    }
}
