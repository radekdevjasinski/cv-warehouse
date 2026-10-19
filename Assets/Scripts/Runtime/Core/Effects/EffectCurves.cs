using System;

namespace CvWarehouse.Core.Effects
{
    public static class EffectCurves
    {
        public static float PopIn(float progress, float overshoot)
        {
            float remaining = Clamp01(progress) - 1f;
            return 1f + (overshoot + 1f) * remaining * remaining * remaining + overshoot * remaining * remaining;
        }

        public static float Pulse(float progress, float peakScale)
        {
            return 1f + (peakScale - 1f) * (float)Math.Sin(Math.PI * Clamp01(progress));
        }

        public static float Flash(float progress)
        {
            float remaining = 1f - Clamp01(progress);
            return remaining * remaining;
        }

        private static float Clamp01(float value)
        {
            return Math.Max(0f, Math.Min(1f, value));
        }
    }
}
