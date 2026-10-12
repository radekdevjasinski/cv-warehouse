using System;

namespace CvWarehouse.Core.Cv
{
    public static class CvScrollMaths
    {
        public static float FurthestOffset(float viewportHeight, float contentHeight)
        {
            return Math.Max(0f, contentHeight - viewportHeight);
        }

        public static float OffsetCentring(float targetFromTop, float viewportHeight, float contentHeight)
        {
            float centred = targetFromTop - viewportHeight * 0.5f;
            return Math.Min(Math.Max(centred, 0f), FurthestOffset(viewportHeight, contentHeight));
        }

        public static bool IsInView(float targetFromTop, float offset, float viewportHeight)
        {
            return targetFromTop >= offset && targetFromTop <= offset + viewportHeight;
        }
    }
}
