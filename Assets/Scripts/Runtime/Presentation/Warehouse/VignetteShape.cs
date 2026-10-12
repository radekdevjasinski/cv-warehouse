using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public static class VignetteShape
    {
        private const float CornerDistance = 1f;

        public static float OpacityAt(float distanceFromCentre, float clearRadius, float cornerOpacity)
        {
            return Mathf.SmoothStep(0f, cornerOpacity, Mathf.InverseLerp(clearRadius, CornerDistance, distanceFromCentre));
        }
    }
}
