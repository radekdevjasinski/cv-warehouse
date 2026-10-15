using System.Collections.Generic;
using CvWarehouse.Core.Audio;
using CvWarehouse.Core.Warehouse;
using UnityEngine;
using UnityEngine.Audio;

namespace CvWarehouse.Presentation.Audio
{
    public sealed class BotVoicePlayer : MonoBehaviour
    {
        [SerializeField] private BotVoiceConfig config;
        [SerializeField] private AudioMixerGroup output;

        private readonly BotVoiceSettings settings = new BotVoiceSettings();
        private readonly List<int> crewSlots = new List<int>();
        private readonly List<BotVoiceCue> dueCues = new List<BotVoiceCue>();
        private BotVoiceCascade cascade;
        private AudioSource[] sources;
        private int nextSource;

        public void Play(IReadOnlyList<Bot> bots)
        {
            config.CopyTo(settings);
            crewSlots.Clear();
            for (int index = 0; index < bots.Count; index++)
                crewSlots.Add(bots[index].CrewSlot);

            cascade.Start(crewSlots);
            PlayDueCues(0f);
        }

        private void Awake()
        {
            cascade = new BotVoiceCascade(settings, new System.Random());
            sources = new AudioSource[config.MaxVoicesInCascade];
            for (int source = 0; source < sources.Length; source++)
                sources[source] = CreateSource();
        }

        private void Update()
        {
            if (cascade.IsPlaying)
                PlayDueCues(Time.deltaTime);
        }

        private void PlayDueCues(float deltaSeconds)
        {
            cascade.Advance(deltaSeconds, dueCues);
            for (int index = 0; index < dueCues.Count; index++)
                PlayCue(dueCues[index]);
        }

        private void PlayCue(BotVoiceCue cue)
        {
            AudioSource source = sources[nextSource];
            nextSource = (nextSource + 1) % sources.Length;
            source.clip = config.Voice(cue.Voice);
            source.pitch = cue.Pitch;
            source.volume = config.Volume * cue.Volume;
            source.Play();
        }

        private AudioSource CreateSource()
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = output;
            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = 0f;
            return source;
        }
    }
}
