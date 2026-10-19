using System;
using System.Collections.Generic;

namespace CvWarehouse.Core.Audio
{
    public sealed class BotVoiceCascade
    {
        private readonly BotVoiceSettings settings;
        private readonly Random random;
        private readonly List<int> pendingVoices = new List<int>();
        private int nextCue;
        private float secondsSinceStart;
        private float volume;

        public BotVoiceCascade(BotVoiceSettings settings, Random random)
        {
            this.settings = settings;
            this.random = random;
        }

        public bool IsPlaying => nextCue < pendingVoices.Count;

        public void Start(IReadOnlyList<int> crewSlots)
        {
            pendingVoices.Clear();
            nextCue = 0;
            secondsSinceStart = 0f;
            if (settings.VoiceCount <= 0)
                return;

            CollectDistinctVoices(crewSlots);
            KeepRandomVoices(Math.Max(0, settings.MaxVoicesInCascade));
            pendingVoices.Sort();
            volume = pendingVoices.Count > 1 ? settings.GroupVolume : 1f;
        }

        public void Advance(float deltaSeconds, List<BotVoiceCue> dueCues)
        {
            dueCues.Clear();
            if (settings.VoiceCount <= 0)
                pendingVoices.Clear();
            if (!IsPlaying)
                return;

            secondsSinceStart += deltaSeconds;
            while (IsPlaying && nextCue * settings.SecondsBetweenVoices <= secondsSinceStart)
            {
                float pitch = RandomPitch.Pick(random, settings.Pitch, settings.PitchVariation);
                dueCues.Add(new BotVoiceCue(pendingVoices[nextCue], volume, pitch));
                nextCue++;
            }
        }

        private void CollectDistinctVoices(IReadOnlyList<int> crewSlots)
        {
            for (int index = 0; index < crewSlots.Count; index++)
            {
                int voice = crewSlots[index] % settings.VoiceCount;
                if (!pendingVoices.Contains(voice))
                    pendingVoices.Add(voice);
            }
        }

        private void KeepRandomVoices(int voicesToKeep)
        {
            if (pendingVoices.Count <= voicesToKeep)
                return;

            for (int kept = 0; kept < voicesToKeep; kept++)
            {
                int picked = random.Next(kept, pendingVoices.Count);
                (pendingVoices[kept], pendingVoices[picked]) = (pendingVoices[picked], pendingVoices[kept]);
            }

            pendingVoices.RemoveRange(voicesToKeep, pendingVoices.Count - voicesToKeep);
        }
    }
}
