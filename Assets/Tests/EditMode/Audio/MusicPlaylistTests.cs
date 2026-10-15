using System;
using CvWarehouse.Core.Audio;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Audio
{
    public sealed class MusicPlaylistTests
    {
        private const int ThreeTracks = 3;

        [Test]
        public void NewPlaylist_WithTracks_StartsAtTheFirstTrack()
        {
            var playlist = new MusicPlaylist(ThreeTracks);

            Assert.IsTrue(playlist.HasTracks);
            Assert.AreEqual(0, playlist.CurrentTrack);
        }

        [Test]
        public void NewPlaylist_NegativeTrackCount_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MusicPlaylist(-1));
        }

        [Test]
        public void TryAdvance_NoTracks_NeverAdvances()
        {
            var playlist = new MusicPlaylist(0);

            Assert.IsFalse(playlist.HasTracks);
            Assert.IsFalse(playlist.TryAdvance(true));
            Assert.IsFalse(playlist.TryAdvance(false));
        }

        [Test]
        public void TryAdvance_TrackHasNotStartedYet_WaitsOnTheSameTrack()
        {
            var playlist = new MusicPlaylist(ThreeTracks);

            Assert.IsFalse(playlist.TryAdvance(false));
            Assert.IsFalse(playlist.TryAdvance(false));
            Assert.AreEqual(0, playlist.CurrentTrack);
        }

        [Test]
        public void TryAdvance_TrackStillPlaying_StaysOnTheSameTrack()
        {
            var playlist = new MusicPlaylist(ThreeTracks);

            Assert.IsFalse(playlist.TryAdvance(true));
            Assert.IsFalse(playlist.TryAdvance(true));
            Assert.AreEqual(0, playlist.CurrentTrack);
        }

        [Test]
        public void TryAdvance_TrackFinished_MovesToTheNextTrack()
        {
            var playlist = new MusicPlaylist(ThreeTracks);
            playlist.TryAdvance(true);

            Assert.IsTrue(playlist.TryAdvance(false));
            Assert.AreEqual(1, playlist.CurrentTrack);
        }

        [Test]
        public void TryAdvance_NextTrackHasNotStartedYet_DoesNotSkipIt()
        {
            var playlist = new MusicPlaylist(ThreeTracks);
            playlist.TryAdvance(true);
            playlist.TryAdvance(false);

            Assert.IsFalse(playlist.TryAdvance(false));
            Assert.AreEqual(1, playlist.CurrentTrack);
        }

        [Test]
        public void TryAdvance_LastTrackFinished_WrapsToTheFirstTrack()
        {
            var playlist = new MusicPlaylist(ThreeTracks);

            for (int track = 0; track < ThreeTracks; track++)
            {
                playlist.TryAdvance(true);
                playlist.TryAdvance(false);
            }

            Assert.AreEqual(0, playlist.CurrentTrack);
        }

        [Test]
        public void TryAdvance_SingleTrackFinished_RepeatsIt()
        {
            var playlist = new MusicPlaylist(1);
            playlist.TryAdvance(true);

            Assert.IsTrue(playlist.TryAdvance(false));
            Assert.AreEqual(0, playlist.CurrentTrack);
        }
    }
}
