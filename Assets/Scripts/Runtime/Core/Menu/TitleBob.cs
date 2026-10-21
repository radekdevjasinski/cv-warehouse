using System;

namespace CvWarehouse.Core.Menu
{
    public static class TitleBob
    {
        public static float OffsetAt(float seconds, float periodSeconds, float amplitude, float step)
        {
            if (periodSeconds <= 0f)
                return 0f;

            float wave = (float)Math.Sin(seconds / periodSeconds * 2.0 * Math.PI) * amplitude;
            return step > 0f ? (float)Math.Round(wave / step) * step : wave;
        }
    }
}
