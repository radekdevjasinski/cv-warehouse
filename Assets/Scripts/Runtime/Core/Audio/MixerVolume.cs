using System;

namespace CvWarehouse.Core.Audio
{
    public static class MixerVolume
    {
        public const float SilentDecibels = -80f;

        public static float DecibelsFor(float baseDecibels, float volume, bool isMuted)
        {
            if (isMuted || volume <= 0f)
                return SilentDecibels;

            float decibels = baseDecibels + 20f * (float)Math.Log10(Math.Min(1f, volume));
            return Math.Max(SilentDecibels, decibels);
        }
    }
}
