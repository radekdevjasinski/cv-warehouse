using System;

namespace CvWarehouse.Core.Audio
{
    public static class RandomPitch
    {
        private const float LowestPitch = 0.01f;

        public static float Pick(Random random, float pitch, float variation)
        {
            float spread = Math.Abs(variation);
            float offset = ((float)random.NextDouble() * 2f - 1f) * spread;
            return Math.Max(LowestPitch, pitch + offset);
        }
    }
}
