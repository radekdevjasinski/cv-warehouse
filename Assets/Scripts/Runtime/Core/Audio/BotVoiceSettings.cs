namespace CvWarehouse.Core.Audio
{
    public sealed class BotVoiceSettings
    {
        public int VoiceCount { get; set; }

        public int MaxVoicesInCascade { get; set; } = 4;

        public float SecondsBetweenVoices { get; set; } = 0.05f;

        public float GroupVolume { get; set; } = 0.8f;

        public float Pitch { get; set; } = 1f;

        public float PitchVariation { get; set; } = 0.03f;
    }
}
