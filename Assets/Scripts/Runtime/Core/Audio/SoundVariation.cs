using System;

namespace CvWarehouse.Core.Audio
{
    public sealed class SoundVariation
    {
        private const int NoClipYet = -1;

        private readonly SoundVariationSettings settings;
        private readonly Random random;
        private int lastClip = NoClipYet;
        private float secondsSinceLastPlay;

        public SoundVariation(SoundVariationSettings settings, Random random)
        {
            this.settings = settings;
            this.random = random;
            secondsSinceLastPlay = float.MaxValue;
        }

        public void Advance(float deltaSeconds)
        {
            secondsSinceLastPlay += deltaSeconds;
        }

        public bool TryPick(out int clip, out float pitch)
        {
            clip = NoClipYet;
            pitch = 1f;
            if (settings.ClipCount <= 0 || secondsSinceLastPlay < settings.MinSecondsBetweenPlays)
                return false;

            clip = PickClipOtherThanLast();
            pitch = RandomPitch.Pick(random, settings.Pitch, settings.PitchVariation);
            lastClip = clip;
            secondsSinceLastPlay = 0f;
            return true;
        }

        private int PickClipOtherThanLast()
        {
            if (settings.ClipCount == 1 || lastClip == NoClipYet || lastClip >= settings.ClipCount)
                return random.Next(settings.ClipCount);

            int clip = random.Next(settings.ClipCount - 1);
            return clip >= lastClip ? clip + 1 : clip;
        }
    }
}
