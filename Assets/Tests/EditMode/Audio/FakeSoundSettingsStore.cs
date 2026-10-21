using CvWarehouse.Core.Audio;

namespace CvWarehouse.Tests.EditMode.Audio
{
    public sealed class FakeSoundSettingsStore : ISoundSettingsStore
    {
        private readonly SoundLevels loaded;

        public FakeSoundSettingsStore(SoundLevels loaded)
        {
            this.loaded = loaded;
        }

        public SoundLevels Saved { get; private set; }

        public int SaveCount { get; private set; }

        public SoundLevels Load()
        {
            return loaded;
        }

        public void Save(SoundLevels levels)
        {
            Saved = levels;
            SaveCount++;
        }
    }
}
