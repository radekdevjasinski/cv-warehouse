using System;

namespace CvWarehouse.Core.Audio
{
    public sealed class SoundSettings
    {
        private readonly ISoundSettingsStore store;

        public SoundSettings(ISoundSettingsStore store)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
            SoundLevels saved = store.Load();
            MusicVolume = ClampVolume(saved.MusicVolume);
            SfxVolume = ClampVolume(saved.SfxVolume);
            IsMuted = saved.IsMuted;
        }

        public event Action Changed;

        public float MusicVolume { get; private set; }

        public float SfxVolume { get; private set; }

        public bool IsMuted { get; private set; }

        public void SetMusicVolume(float volume)
        {
            MusicVolume = ClampVolume(volume);
            Changed?.Invoke();
        }

        public void SetSfxVolume(float volume)
        {
            SfxVolume = ClampVolume(volume);
            Changed?.Invoke();
        }

        public void ToggleMute()
        {
            IsMuted = !IsMuted;
            Changed?.Invoke();
        }

        public void Save()
        {
            store.Save(new SoundLevels(MusicVolume, SfxVolume, IsMuted));
        }

        private static float ClampVolume(float volume)
        {
            return float.IsNaN(volume) ? 1f : Math.Max(0f, Math.Min(1f, volume));
        }
    }
}
