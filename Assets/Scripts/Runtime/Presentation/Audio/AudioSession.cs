using CvWarehouse.Core.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace CvWarehouse.Presentation.Audio
{
    [RequireComponent(typeof(MusicPlayer))]
    public sealed class AudioSession : MonoBehaviour
    {
        [SerializeField] private AudioSessionChannel channel;
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private string musicVolumeParameter = "MusicVolume";
        [SerializeField] private string sfxVolumeParameter = "SfxVolume";

        [Header("Mixer levels at full volume, in decibels")]
        [SerializeField] private float musicBaseDecibels = -6.0206f;
        [SerializeField] private float sfxBaseDecibels = -9.118639f;

        public SoundSettings Settings { get; private set; }

        public MusicPlayer Music { get; private set; }

        public float MusicDecibels => MixerVolume.DecibelsFor(musicBaseDecibels, Settings.MusicVolume, Settings.IsMuted);

        public float SfxDecibels => MixerVolume.DecibelsFor(sfxBaseDecibels, Settings.SfxVolume, Settings.IsMuted);

        private void Awake()
        {
            Settings = new SoundSettings(new PlayerPrefsSoundSettingsStore());
            Music = GetComponent<MusicPlayer>();
            channel.Register(this);
        }

        private void OnEnable()
        {
            Settings.Changed += ApplyVolumes;
        }

        private void Start()
        {
            ApplyVolumes();
        }

        private void OnDisable()
        {
            Settings.Changed -= ApplyVolumes;
        }

        private void OnDestroy()
        {
            channel.Unregister(this);
        }

        private void ApplyVolumes()
        {
            mixer.SetFloat(musicVolumeParameter, MusicDecibels);
            mixer.SetFloat(sfxVolumeParameter, SfxDecibels);
        }
    }
}
