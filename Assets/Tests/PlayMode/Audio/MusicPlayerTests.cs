using System.Collections;
using CvWarehouse.Presentation.Audio;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace CvWarehouse.Tests.PlayMode.Audio
{
    public sealed class MusicPlayerTests
    {
        private const int SampleRate = 44100;
        private const int ShortClipSamples = SampleRate / 20;
        private const int MaxFramesToWait = 600;

        private GameObject playerObject;
        private MusicPlayer player;
        private AudioClip[] tracks;

        [SetUp]
        public void CreatePlayer()
        {
            playerObject = new GameObject("MusicPlayer", typeof(AudioSource));
            player = playerObject.AddComponent<MusicPlayer>();
            tracks = new[] { CreateShortClip("First"), CreateShortClip("Second"), CreateShortClip("Third") };
        }

        [TearDown]
        public void DestroyPlayer()
        {
            Object.Destroy(playerObject);
            foreach (AudioClip track in tracks)
                Object.Destroy(track);
        }

        [UnityTest]
        public IEnumerator PlayInOrder_Started_PlaysTheFirstTrack()
        {
            yield return null;

            player.PlayInOrder(tracks);

            Assert.AreSame(tracks[0], player.CurrentClip);
        }

        [UnityTest]
        public IEnumerator PlayInOrder_EveryTrackFinishes_PlaysThemInSequenceAndStartsOver()
        {
            yield return null;
            player.PlayInOrder(tracks);

            yield return WaitForClip(tracks[1]);
            yield return WaitForClip(tracks[2]);
            yield return WaitForClip(tracks[0]);
        }

        [UnityTest]
        public IEnumerator PlayInOrder_NoTracks_StaysSilent()
        {
            yield return null;

            player.PlayInOrder(new AudioClip[0]);
            yield return null;

            Assert.IsFalse(playerObject.GetComponent<AudioSource>().isPlaying);
        }

        private IEnumerator WaitForClip(AudioClip expectedClip)
        {
            for (int frame = 0; frame < MaxFramesToWait && player.CurrentClip != expectedClip; frame++)
                yield return null;

            Assert.AreSame(expectedClip, player.CurrentClip, "The playlist did not move on to " + expectedClip.name + " in time.");
        }

        private static AudioClip CreateShortClip(string clipName)
        {
            AudioClip clip = AudioClip.Create(clipName, ShortClipSamples, 1, SampleRate, false);
            clip.SetData(new float[ShortClipSamples], 0);
            return clip;
        }
    }
}
