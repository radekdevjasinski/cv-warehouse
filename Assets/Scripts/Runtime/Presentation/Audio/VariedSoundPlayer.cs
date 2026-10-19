using CvWarehouse.Core.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace CvWarehouse.Presentation.Audio
{
    public sealed class VariedSoundPlayer : MonoBehaviour
    {
        [SerializeField] private SoundVariationConfig config;
        [SerializeField] private AudioMixerGroup output;

        private readonly SoundVariationSettings settings = new SoundVariationSettings();
        private SoundVariation variation;
        private AudioSource[] voices;
        private int nextVoice;

        public void Play()
        {
            config.CopyTo(settings);
            if (!variation.TryPick(out int clip, out float pitch))
                return;

            AudioSource voice = voices[nextVoice];
            nextVoice = (nextVoice + 1) % voices.Length;
            voice.clip = config.Clip(clip);
            voice.pitch = pitch;
            voice.volume = config.Volume;
            voice.Play();
        }

        private void Awake()
        {
            variation = new SoundVariation(settings, new System.Random());
            voices = new AudioSource[config.Voices];
            for (int voice = 0; voice < voices.Length; voice++)
                voices[voice] = CreateVoice();
        }

        private void Update()
        {
            variation.Advance(Time.deltaTime);
        }

        private AudioSource CreateVoice()
        {
            AudioSource voice = gameObject.AddComponent<AudioSource>();
            voice.outputAudioMixerGroup = output;
            voice.playOnAwake = false;
            voice.loop = false;
            voice.spatialBlend = 0f;
            return voice;
        }
    }
}
