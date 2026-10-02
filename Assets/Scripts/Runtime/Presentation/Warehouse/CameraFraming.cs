using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class CameraFraming
    {
        private readonly Vector2 mapSize;
        private readonly float closestSize;
        private readonly float margin;
        private float aspect = 1f;

        public CameraFraming(Vector2 mapSize, float closestSize, float margin)
        {
            this.mapSize = mapSize;
            this.closestSize = closestSize;
            this.margin = margin;
            Size = closestSize;
            Centre = mapSize * 0.5f;
        }

        public Vector2 Centre { get; private set; }

        public float Size { get; private set; }

        private float WholeMapSize => Mathf.Max(mapSize.y * 0.5f, mapSize.x * 0.5f / aspect) + margin;

        public void SetAspect(float newAspect)
        {
            if (Mathf.Approximately(aspect, newAspect))
                return;

            aspect = newAspect;
            Clamp();
        }

        public void LookAt(Vector2 centre, float size)
        {
            Centre = centre;
            Size = size;
            Clamp();
        }

        public void Pan(Vector2 worldDelta)
        {
            Centre += worldDelta;
            Clamp();
        }

        public void Zoom(float sizeFactor)
        {
            Size *= sizeFactor;
            Clamp();
        }

        private void Clamp()
        {
            Size = Mathf.Clamp(Size, Mathf.Min(closestSize, WholeMapSize), WholeMapSize);
            Centre = new Vector2(
                ClampAxis(Centre.x, Size * aspect, mapSize.x),
                ClampAxis(Centre.y, Size, mapSize.y));
        }

        private float ClampAxis(float centre, float halfView, float mapLength)
        {
            float lowest = halfView - margin;
            float highest = mapLength - halfView + margin;
            return lowest > highest ? mapLength * 0.5f : Mathf.Clamp(centre, lowest, highest);
        }
    }
}
