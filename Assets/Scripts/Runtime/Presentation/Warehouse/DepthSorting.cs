using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public static class DepthSorting
    {
        public const int LayersPerPixel = 4;
        public const float PixelsPerUnit = 16f;
        public const int OnTheFloorOrder = 2;
        public const int OverEverythingOrder = 29000;

        private const int FloorLevelOrder = 20000;

        public static int OrderAt(float feetY, int layer)
        {
            return FloorLevelOrder - Mathf.RoundToInt(feetY * PixelsPerUnit) * LayersPerPixel + layer;
        }
    }
}
