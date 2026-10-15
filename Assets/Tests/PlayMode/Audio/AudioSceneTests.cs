using System.Collections;
using System.Collections.Generic;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Presentation.Audio;
using CvWarehouse.Presentation.Warehouse;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace CvWarehouse.Tests.PlayMode.Audio
{
    public sealed class AudioSceneTests
    {
        private const string ScenePath = "Assets/Scenes/WarehouseDemo.unity";
        private const string FirstTrackName = "track1";
        private const string DeliveryConfigPath = "Assets/Settings/DeliveryPopSound.asset";
        private const string ExtractConfigPath = "Assets/Settings/ExtractSound.asset";
        private const string BotVoicesConfigPath = "Assets/Settings/BotVoices.asset";
        private const string DeliverySoundName = "DeliverySound";
        private const string ExtractSoundName = "ExtractSound";
        private const string BotVoicesName = "BotVoices";
        private const float CascadeSeconds = 0.5f;
        private const string MusicGroupName = "Music";
        private const string SfxGroupName = "SFX";
        private const float MinPitch = 0.5f;
        private const float MaxPitch = 2f;
        private const int MaxFramesToWait = 300;
        private const int MaxTicks = 600000;

        private WarehouseDemo demo;

        [UnitySetUp]
        public IEnumerator LoadWarehouseScene()
        {
#if UNITY_EDITOR
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(ScenePath, new LoadSceneParameters(LoadSceneMode.Single));
#endif
            yield return null;
            yield return null;
            demo = Object.FindAnyObjectByType<WarehouseDemo>();
            demo.enabled = false;
            for (int frame = 0; frame < MaxFramesToWait && demo.Simulation == null; frame++)
                yield return null;

            Assert.IsNotNull(demo.Simulation, "The warehouse did not boot from the CV in time.");
        }

        [Test]
        public void WarehouseScene_Booted_StartsTheFirstMusicTrack()
        {
            MusicPlayer player = Object.FindAnyObjectByType<MusicPlayer>();

            Assert.IsNotNull(player, "The warehouse scene has no music player.");
            Assert.IsNotNull(player.CurrentClip, "The music player did not start a track.");
            Assert.AreEqual(FirstTrackName, player.CurrentClip.name);
        }

        [Test]
        public void Extraction_BotTakesLettersOutOfABox_PlaysACardDrawWithAVariedPitch()
        {
            WarehouseSimulation simulation = demo.Simulation;
            bool hasExtracted = false;
            simulation.Extracted += bot => hasExtracted = true;

            for (int tick = 0; tick < MaxTicks && !hasExtracted; tick++)
                simulation.Tick();

            AssertPlaysOneOf(ExtractSoundName, ConfiguredClips(ExtractConfigPath, "clips"));
        }

        [Test]
        public void Delivery_LettersReachTheTruck_PlaysAPopWithAVariedPitch()
        {
            WarehouseSimulation simulation = demo.Simulation;

            for (int tick = 0; tick < MaxTicks && simulation.Score.DeliveredPieces == 0; tick++)
                simulation.Tick();

            AssertPlaysOneOf(DeliverySoundName, ConfiguredClips(DeliveryConfigPath, "clips"));
        }

        [Test]
        public void BotSelection_OneBotPicked_PlaysThatBotsVoice()
        {
            Bot bot = demo.Simulation.Bots[0];

            demo.Selection.Select(bot);

            List<string> voices = ConfiguredClips(BotVoicesConfigPath, "voices");
            AssertPlaysOneOf(BotVoicesName, new List<string> { voices[bot.CrewSlot % voices.Count] });
        }

        [UnityTest]
        public IEnumerator BotSelection_ManyBotsPicked_PlaysEachVoiceInACascade()
        {
            demo.Selection.SelectAll(demo.Simulation.Bots);
            yield return new WaitForSeconds(CascadeSeconds);

            var playedClips = new List<string>();
            foreach (AudioSource voice in GameObject.Find(BotVoicesName).GetComponents<AudioSource>())
            {
                if (voice.clip != null)
                    playedClips.Add(voice.clip.name);
            }

            playedClips.Sort();
            List<string> voices = ConfiguredClips(BotVoicesConfigPath, "voices");
            var expectedClips = new List<string>();
            foreach (Bot bot in demo.Simulation.Bots)
                expectedClips.Add(voices[bot.CrewSlot % voices.Count]);
            expectedClips.Sort();
            CollectionAssert.AreEqual(expectedClips, playedClips);
        }

        [Test]
        public void WarehouseScene_Booted_RoutesMusicAndEffectsToTheirMixerGroups()
        {
            AudioSource musicSource = Object.FindAnyObjectByType<MusicPlayer>().GetComponent<AudioSource>();
            Assert.AreEqual(MusicGroupName, musicSource.outputAudioMixerGroup.name);

            foreach (string soundName in new[] { ExtractSoundName, DeliverySoundName, BotVoicesName })
            {
                foreach (AudioSource voice in GameObject.Find(soundName).GetComponents<AudioSource>())
                    Assert.AreEqual(SfxGroupName, voice.outputAudioMixerGroup.name, soundName);
            }
        }

        private static List<string> ConfiguredClips(string configPath, string clipsField)
        {
            var clipNames = new List<string>();
#if UNITY_EDITOR
            var config = new UnityEditor.SerializedObject(UnityEditor.AssetDatabase.LoadMainAssetAtPath(configPath));
            UnityEditor.SerializedProperty clips = config.FindProperty(clipsField);
            for (int index = 0; index < clips.arraySize; index++)
                clipNames.Add(clips.GetArrayElementAtIndex(index).objectReferenceValue.name);
#endif
            Assert.IsNotEmpty(clipNames, configPath + " has no clips.");
            return clipNames;
        }

        private static void AssertPlaysOneOf(string soundName, List<string> clipNames)
        {
            GameObject soundObject = GameObject.Find(soundName);
            Assert.IsNotNull(soundObject, "The warehouse scene has no " + soundName + ".");

            AudioSource playingVoice = null;
            foreach (AudioSource voice in soundObject.GetComponents<AudioSource>())
            {
                if (voice.isPlaying)
                    playingVoice = voice;
            }

            Assert.IsNotNull(playingVoice, soundName + " did not play.");
            CollectionAssert.Contains(clipNames, playingVoice.clip.name);
            Assert.That(playingVoice.pitch, Is.InRange(MinPitch, MaxPitch));
        }
    }
}
