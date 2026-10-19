using System;
using System.Collections.Generic;

namespace CvWarehouse.Core.Effects
{
    public sealed class EffectTimers
    {
        private readonly List<int> ids;
        private readonly List<float> ages;

        public EffectTimers(int expectedCount)
        {
            ids = new List<int>(expectedCount);
            ages = new List<float>(expectedCount);
        }

        public float DurationSeconds { get; set; } = 1f;

        public int Count => ids.Count;

        public void Start(int id)
        {
            int index = ids.IndexOf(id);
            if (index >= 0)
            {
                ages[index] = 0f;
                return;
            }

            ids.Add(id);
            ages.Add(0f);
        }

        public void Advance(float deltaSeconds)
        {
            for (int index = ids.Count - 1; index >= 0; index--)
            {
                ages[index] += deltaSeconds;
                if (ages[index] >= DurationSeconds)
                    RemoveAt(index);
            }
        }

        public bool TryGetProgress(int id, out float progress)
        {
            int index = ids.IndexOf(id);
            if (index < 0)
            {
                progress = 1f;
                return false;
            }

            progress = DurationSeconds <= 0f ? 1f : Math.Min(1f, ages[index] / DurationSeconds);
            return true;
        }

        private void RemoveAt(int index)
        {
            int last = ids.Count - 1;
            ids[index] = ids[last];
            ages[index] = ages[last];
            ids.RemoveAt(last);
            ages.RemoveAt(last);
        }
    }
}
