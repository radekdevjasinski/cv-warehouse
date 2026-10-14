using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class CameraFraming
    {
        private const float ScaleEpsilon = 0.0001f;

        private readonly Vector2 mapSize;
        private readonly CameraZoomLimits limits;
        private Vector2Int viewPixels = Vector2Int.one;
        private Vector2 centre;
        private int closestScale = 1;
        private int farthestScale = 1;

        public CameraFraming(Vector2 mapSize, CameraZoomLimits limits)
        {
            this.mapSize = mapSize;
            this.limits = limits;
            centre = mapSize * 0.5f;
            Scale = 1;
        }

        public int Scale { get; private set; }

        public float UnitsPerScreenPixel => 1f / (limits.SpritePixelsPerUnit * Scale);

        public float Size => viewPixels.y * 0.5f * UnitsPerScreenPixel;

        public Vector2 Centre => new Vector2(SnapToScreenPixel(centre.x, viewPixels.x), SnapToScreenPixel(centre.y, viewPixels.y));

        public void SetViewPixels(Vector2Int newViewPixels)
        {
            if (newViewPixels == viewPixels || newViewPixels.x <= 0 || newViewPixels.y <= 0)
                return;

            viewPixels = newViewPixels;
            UpdateScaleRange();
            Clamp();
        }

        public void LookAt(Vector2 newCentre)
        {
            centre = newCentre;
            Clamp();
        }

        public void Pan(Vector2 worldDelta)
        {
            centre += worldDelta;
            Clamp();
        }

        public void ZoomIn()
        {
            Scale++;
            Clamp();
        }

        public void ZoomOut()
        {
            Scale--;
            Clamp();
        }

        public void ZoomOutFully()
        {
            Scale = farthestScale;
            Clamp();
        }

        private void UpdateScaleRange()
        {
            int shortSide = Mathf.Min(viewPixels.x, viewPixels.y);
            int fitsTheMap = Mathf.Max(ScaleShowingAtMost(viewPixels.x, mapSize.x), ScaleShowingAtMost(viewPixels.y, mapSize.y));
            farthestScale = Mathf.Max(fitsTheMap, ScaleShowingAtMost(shortSide, limits.FarthestViewCells));
            int closest = Mathf.FloorToInt(shortSide / (limits.SpritePixelsPerUnit * limits.ClosestViewCells) + ScaleEpsilon);
            closestScale = Mathf.Max(farthestScale, closest);
        }

        private int ScaleShowingAtMost(int pixels, float cells)
        {
            return Mathf.Max(1, Mathf.CeilToInt(pixels / (limits.SpritePixelsPerUnit * cells) - ScaleEpsilon));
        }

        private void Clamp()
        {
            Scale = Mathf.Clamp(Scale, farthestScale, closestScale);
            Vector2 halfView = (Vector2)viewPixels * (0.5f * UnitsPerScreenPixel);
            centre = new Vector2(ClampAxis(centre.x, halfView.x, mapSize.x), ClampAxis(centre.y, halfView.y, mapSize.y));
        }

        private static float ClampAxis(float axisCentre, float halfView, float mapLength)
        {
            float highest = mapLength - halfView;
            return halfView > highest ? mapLength * 0.5f : Mathf.Clamp(axisCentre, halfView, highest);
        }

        private float SnapToScreenPixel(float axisCentre, int axisPixels)
        {
            float halfViewPixels = axisPixels * 0.5f;
            float viewStartPixel = Mathf.Round(axisCentre / UnitsPerScreenPixel - halfViewPixels);
            return (viewStartPixel + halfViewPixels) * UnitsPerScreenPixel;
        }
    }
}
