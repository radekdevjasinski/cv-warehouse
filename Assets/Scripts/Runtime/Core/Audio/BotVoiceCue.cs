namespace CvWarehouse.Core.Audio
{
    public readonly struct BotVoiceCue
    {
        public BotVoiceCue(int voice, float volume, float pitch)
        {
            Voice = voice;
            Volume = volume;
            Pitch = pitch;
        }

        public int Voice { get; }

        public float Volume { get; }

        public float Pitch { get; }
    }
}
