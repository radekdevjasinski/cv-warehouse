using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public static class EdgePan
    {
        public static Vector2 DirectionAt(Vector2 pointer, Rect view, float edgeWidth)
        {
            if (!view.Contains(pointer))
                return Vector2.zero;

            return new Vector2(
                AxisDirection(pointer.x - view.xMin, view.width, edgeWidth),
                AxisDirection(pointer.y - view.yMin, view.height, edgeWidth));
        }

        private static float AxisDirection(float distanceFromStart, float length, float edgeWidth)
        {
            if (distanceFromStart < edgeWidth)
                return -1f;

            return distanceFromStart > length - edgeWidth ? 1f : 0f;
        }
    }
}
