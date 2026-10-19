using System;

namespace CvWarehouse.Core.Effects
{
    public sealed class EffectTimer
    {
        private float ageSeconds;

        public float DurationSeconds { get; set; } = 1f;

        public bool IsRunning { get; private set; }

        public float Progress => DurationSeconds <= 0f ? 1f : Math.Min(1f, ageSeconds / DurationSeconds);

        public void Start()
        {
            ageSeconds = 0f;
            IsRunning = true;
        }

        public void Advance(float deltaSeconds)
        {
            if (!IsRunning)
                return;

            ageSeconds += deltaSeconds;
            if (ageSeconds >= DurationSeconds)
                IsRunning = false;
        }
    }
}
