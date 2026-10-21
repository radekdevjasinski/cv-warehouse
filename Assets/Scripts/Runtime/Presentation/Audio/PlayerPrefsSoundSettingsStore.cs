using CvWarehouse.Core.Audio;
using UnityEngine;

namespace CvWarehouse.Presentation.Audio
{
    public sealed class PlayerPrefsSoundSettingsStore : ISoundSettingsStore
    {
        public const string MusicVolumeKey = "sound.musicVolume";
        public const string SfxVolumeKey = "sound.sfxVolume";
        public const string IsMutedKey = "sound.isMuted";

        public SoundLevels Load()
        {
            return new SoundLevels(
                PlayerPrefs.GetFloat(MusicVolumeKey, SoundLevels.Default.MusicVolume),
                PlayerPrefs.GetFloat(SfxVolumeKey, SoundLevels.Default.SfxVolume),
                PlayerPrefs.GetInt(IsMutedKey, 0) == 1);
        }

        public void Save(SoundLevels levels)
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, levels.MusicVolume);
            PlayerPrefs.SetFloat(SfxVolumeKey, levels.SfxVolume);
            PlayerPrefs.SetInt(IsMutedKey, levels.IsMuted ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
