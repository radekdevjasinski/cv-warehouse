using System;
using System.Collections.Generic;

namespace CvWarehouse.Core.Audio
{
    public sealed class BotVoiceCascade
    {
        private readonly BotVoiceSettings settings;
        private readonly Random random;
        private readonly List<int> pendingSlots = new List<int>();
        private int nextCue;
        private float secondsSinceStart;
        private float volume;

        public BotVoiceCascade(BotVoiceSettings settings, Random random)
        {
            this.settings = settings;
            this.random = random;
        }

        public bool IsPlaying => nextCue < pendingSlots.Count;

        public void Start(IReadOnlyList<int> crewSlots)
        {
            pendingSlots.Clear();
            nextCue = 0;
            secondsSinceStart = 0f;
            if (settings.VoiceCount <= 0)
                return;

            for (int index = 0; index < crewSlots.Count; index++)
                pendingSlots.Add(crewSlots[index]);
            pendingSlots.Sort();
            int voicesInCascade = Math.Max(0, settings.MaxVoicesInCascade);
            if (pendingSlots.Count > voicesInCascade)
                pendingSlots.RemoveRange(voicesInCascade, pendingSlots.Count - voicesInCascade);

            volume = pendingSlots.Count > 1 ? settings.GroupVolume : 1f;
        }

        public void Advance(float deltaSeconds, List<BotVoiceCue> dueCues)
        {
            dueCues.Clear();
            if (settings.VoiceCount <= 0)
                pendingSlots.Clear();
            if (!IsPlaying)
                return;

            secondsSinceStart += deltaSeconds;
            while (IsPlaying && nextCue * settings.SecondsBetweenVoices <= secondsSinceStart)
            {
                float pitch = RandomPitch.Pick(random, settings.Pitch, settings.PitchVariation);
                dueCues.Add(new BotVoiceCue(pendingSlots[nextCue] % settings.VoiceCount, volume, pitch));
                nextCue++;
            }
        }
    }
}
