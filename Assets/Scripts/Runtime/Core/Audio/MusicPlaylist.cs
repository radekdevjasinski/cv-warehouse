using System;

namespace CvWarehouse.Core.Audio
{
    public sealed class MusicPlaylist
    {
        private readonly int trackCount;
        private bool hasCurrentTrackStarted;

        public MusicPlaylist(int trackCount)
        {
            if (trackCount < 0)
                throw new ArgumentOutOfRangeException(nameof(trackCount), trackCount, "A playlist cannot have a negative number of tracks.");

            this.trackCount = trackCount;
        }

        public int CurrentTrack { get; private set; }

        public bool HasTracks => trackCount > 0;

        public bool TryAdvance(bool isCurrentTrackPlaying)
        {
            if (!HasTracks)
                return false;

            if (isCurrentTrackPlaying)
            {
                hasCurrentTrackStarted = true;
                return false;
            }

            if (!hasCurrentTrackStarted)
                return false;

            CurrentTrack = (CurrentTrack + 1) % trackCount;
            hasCurrentTrackStarted = false;
            return true;
        }
    }
}
