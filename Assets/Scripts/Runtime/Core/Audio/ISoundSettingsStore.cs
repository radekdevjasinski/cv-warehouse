namespace CvWarehouse.Core.Audio
{
    public interface ISoundSettingsStore
    {
        SoundLevels Load();

        void Save(SoundLevels levels);
    }
}
