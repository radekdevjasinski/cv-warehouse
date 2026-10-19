namespace CvWarehouse.Core.Audio
{
    public sealed class SoundVariationSettings
    {
        public int ClipCount { get; set; }

        public float Pitch { get; set; } = 1f;

        public float PitchVariation { get; set; } = 0.08f;

        public float MinSecondsBetweenPlays { get; set; } = 0.05f;
    }
}
