using System;
using System.Collections.Generic;

namespace CvWarehouse.Core.Effects
{
    public sealed class PointsPopupQueue
    {
        private readonly PointsPopupSettings settings;
        private readonly List<PointsPopup> popups;
        private readonly int capacity;

        public PointsPopupQueue(PointsPopupSettings settings, int capacity)
        {
            this.settings = settings;
            this.capacity = Math.Max(1, capacity);
            popups = new List<PointsPopup>(this.capacity);
        }

        public int Count => popups.Count;

        public PointsPopup this[int index] => popups[index];

        public void Add(float x, float y, int points)
        {
            if (points <= 0)
                return;

            if (popups.Count == capacity)
                popups.RemoveAt(0);
            popups.Add(new PointsPopup(x, ClearHeightAbove(x, y), points, 0f));
        }

        public void Advance(float deltaSeconds)
        {
            for (int index = 0; index < popups.Count; index++)
                popups[index] = popups[index].Aged(deltaSeconds);

            while (popups.Count > 0 && popups[0].AgeSeconds >= settings.DurationSeconds)
                popups.RemoveAt(0);
        }

        public float RiseOf(int index)
        {
            float progress = Progress(popups[index]);
            float easedOut = 1f - (1f - progress) * (1f - progress);
            return settings.RiseDistance * easedOut;
        }

        public float AlphaOf(int index)
        {
            float progress = Progress(popups[index]);
            float fadeStart = Clamp01(settings.FadeStartFraction);
            if (progress <= fadeStart)
                return 1f;

            return Clamp01((1f - progress) / (1f - fadeStart));
        }

        private float ClearHeightAbove(float x, float y)
        {
            for (int pass = 0; pass < popups.Count; pass++)
            {
                if (!TryFindOverlap(x, y, out float overlappedHeight))
                    return y;

                y = overlappedHeight + settings.LineHeight;
            }

            return y;
        }

        private bool TryFindOverlap(float x, float y, out float overlappedHeight)
        {
            for (int index = 0; index < popups.Count; index++)
            {
                float height = popups[index].Y + RiseOf(index);
                if (Math.Abs(popups[index].X - x) < settings.OverlapWidth && Math.Abs(height - y) < settings.LineHeight)
                {
                    overlappedHeight = height;
                    return true;
                }
            }

            overlappedHeight = 0f;
            return false;
        }

        private float Progress(PointsPopup popup)
        {
            if (settings.DurationSeconds <= 0f)
                return 1f;

            return Clamp01(popup.AgeSeconds / settings.DurationSeconds);
        }

        private static float Clamp01(float value)
        {
            return Math.Max(0f, Math.Min(1f, value));
        }
    }
}
