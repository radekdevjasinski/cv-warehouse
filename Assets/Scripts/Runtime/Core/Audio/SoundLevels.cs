namespace CvWarehouse.Core.Audio
{
    public readonly struct SoundLevels
    {
        public static readonly SoundLevels Default = new SoundLevels(1f, 1f, false);

        public SoundLevels(float musicVolume, float sfxVolume, bool isMuted)
        {
            MusicVolume = musicVolume;
            SfxVolume = sfxVolume;
            IsMuted = isMuted;
        }

        public float MusicVolume { get; }

        public float SfxVolume { get; }

        public bool IsMuted { get; }
    }
}
