using System.Collections;
using CvWarehouse.Core.Audio;
using CvWarehouse.Presentation.Audio;
using CvWarehouse.Presentation.Warehouse;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace CvWarehouse.Tests.PlayMode.Audio
{
    public sealed class AudioSessionSceneTests
    {
        private const string MenuScenePath = "Assets/Scenes/MainMenu.unity";
        private const string ChannelPath = "Assets/Settings/AudioSessionChannel.asset";
        private const string MixerPath = "Assets/Audio/GameMixer.mixer";
        private const string StartShiftButtonName = "StartShiftButton";
        private const string SoundButtonName = "SoundButton";
        private const string SettingsButtonName = "SettingsButton";
        private const string MenuButtonName = "MenuButton";
        private const string RestartButtonName = "RestartButton";
        private const string MusicSliderName = "MusicSlider";
        private const string SfxSliderName = "SfxSlider";
        private const string MusicValueName = "MusicValue";
        private const string SoundOnIconName = "SoundIcon";
        private const string SoundOffIconName = "SoundOffIcon";
        private const string MusicParameter = "MusicVolume";
        private const string SfxParameter = "SfxVolume";
        private const int MaxFramesToWait = 300;
        private const float DecibelTolerance = 0.01f;

        private AudioSessionChannel channel;
        private AudioMixer mixer;

        [UnitySetUp]
        public IEnumerator LoadMenuScene()
        {
            ClearSavedSound();
#if UNITY_EDITOR
            channel = AssetDatabase.LoadAssetAtPath<AudioSessionChannel>(ChannelPath);
            mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(MixerPath);
            DestroySession();
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(MenuScenePath, new LoadSceneParameters(LoadSceneMode.Single));
#endif
            yield return null;
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator DestroySessionAndSavedSound()
        {
            DestroySession();
            ClearSavedSound();
            yield return null;
        }

        [Test]
        public void MainMenu_Loaded_PlaysMusicThroughOneListener()
        {
            Assert.IsTrue(channel.HasSession);
            Assert.IsTrue(channel.Session.GetComponent<AudioSource>().isPlaying);
            Assert.AreEqual(1, Object.FindObjectsByType<AudioListener>().Length);
        }

        [UnityTest]
        public IEnumerator StartShift_GameLoaded_KeepsTheSameMusicPlaying()
        {
            AudioSession menuSession = channel.Session;

            Click(StartShiftButtonName);
            yield return WaitForScene<WarehouseDemo>();

            Assert.AreSame(menuSession, channel.Session);
            Assert.IsTrue(menuSession.GetComponent<AudioSource>().isPlaying);
            Assert.AreEqual(1, Object.FindObjectsByType<AudioSession>().Length);
            Assert.AreEqual(1, Object.FindObjectsByType<AudioListener>().Length);
        }

        [UnityTest]
        public IEnumerator Restart_InTheGame_KeepsTheSameMusicPlaying()
        {
            AudioSession menuSession = channel.Session;
            Click(StartShiftButtonName);
            WarehouseDemo firstDemo = null;
            for (int frame = 0; frame < MaxFramesToWait && firstDemo == null; frame++)
            {
                yield return null;
                firstDemo = Object.FindAnyObjectByType<WarehouseDemo>();
            }

            Click(MenuButtonName);
            Click(RestartButtonName);
            for (int frame = 0; frame < MaxFramesToWait && Object.FindAnyObjectByType<WarehouseDemo>() == firstDemo; frame++)
                yield return null;

            Assert.AreNotSame(firstDemo, Object.FindAnyObjectByType<WarehouseDemo>());
            Assert.AreSame(menuSession, channel.Session);
            Assert.AreEqual(1, Object.FindObjectsByType<AudioSession>().Length);
        }

        [UnityTest]
        public IEnumerator SoundButton_Clicked_MutesEverythingAndSwapsTheIcon()
        {
            Click(SoundButtonName);
            yield return null;

            Assert.IsTrue(channel.Session.Settings.IsMuted);
            Assert.AreEqual(SoundOffIconName, IconOf(SoundButtonName));
            Assert.AreEqual(MixerVolume.SilentDecibels, Decibels(MusicParameter), DecibelTolerance);
            Assert.AreEqual(MixerVolume.SilentDecibels, Decibels(SfxParameter), DecibelTolerance);
            Assert.IsTrue(new PlayerPrefsSoundSettingsStore().Load().IsMuted);
        }

        [UnityTest]
        public IEnumerator SoundButton_ClickedTwice_RestoresTheSoundAndTheIcon()
        {
            Click(SoundButtonName);
            Click(SoundButtonName);
            yield return null;

            Assert.IsFalse(channel.Session.Settings.IsMuted);
            Assert.AreEqual(SoundOnIconName, IconOf(SoundButtonName));
            Assert.AreEqual(channel.Session.MusicDecibels, Decibels(MusicParameter), DecibelTolerance);
            Assert.Greater(Decibels(MusicParameter), MixerVolume.SilentDecibels);
        }

        [UnityTest]
        public IEnumerator SettingsPanel_Opened_StartsAtTheCurrentFullVolumes()
        {
            Click(SettingsButtonName);
            yield return null;

            Assert.AreEqual(1f, SliderNamed(MusicSliderName).value);
            Assert.AreEqual(1f, SliderNamed(SfxSliderName).value);
            Assert.AreEqual("100%", GameObject.Find(MusicValueName).GetComponent<TMPro.TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator SettingsPanel_SlidersMoved_ChangeTheMixerAndAreSavedOnClose()
        {
            SoundSettingsPanel panel = Object.FindAnyObjectByType<SoundSettingsPanel>();
            Click(SettingsButtonName);
            yield return null;

            SliderNamed(MusicSliderName).value = 0.5f;
            SliderNamed(SfxSliderName).value = 0.25f;
            Assert.AreEqual("50%", GameObject.Find(MusicValueName).GetComponent<TMPro.TMP_Text>().text);
            panel.Close();
            yield return null;

            Assert.AreEqual(channel.Session.MusicDecibels, Decibels(MusicParameter), DecibelTolerance);
            Assert.AreEqual(channel.Session.SfxDecibels, Decibels(SfxParameter), DecibelTolerance);
            Assert.Less(Decibels(SfxParameter), Decibels(MusicParameter));
            SoundLevels saved = new PlayerPrefsSoundSettingsStore().Load();
            Assert.AreEqual(0.5f, saved.MusicVolume, 0.0001f);
            Assert.AreEqual(0.25f, saved.SfxVolume, 0.0001f);
        }

        private static void Click(string buttonName)
        {
            GameObject.Find(buttonName).GetComponent<Button>().onClick.Invoke();
        }

        private static Slider SliderNamed(string sliderName)
        {
            return GameObject.Find(sliderName).GetComponent<Slider>();
        }

        private static string IconOf(string buttonName)
        {
            return GameObject.Find(buttonName).transform.Find("Icon").GetComponent<Image>().sprite.name;
        }

        private static IEnumerator WaitForScene<T>() where T : Object
        {
            for (int frame = 0; frame < MaxFramesToWait && Object.FindAnyObjectByType<T>() == null; frame++)
                yield return null;

            Assert.IsNotNull(Object.FindAnyObjectByType<T>(), "The scene did not load in time.");
        }

        private static void ClearSavedSound()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsSoundSettingsStore.MusicVolumeKey);
            PlayerPrefs.DeleteKey(PlayerPrefsSoundSettingsStore.SfxVolumeKey);
            PlayerPrefs.DeleteKey(PlayerPrefsSoundSettingsStore.IsMutedKey);
        }

        private float Decibels(string parameter)
        {
            Assert.IsTrue(mixer.GetFloat(parameter, out float decibels), parameter);
            return decibels;
        }

        private void DestroySession()
        {
            if (channel != null && channel.HasSession)
                Object.DestroyImmediate(channel.Session.gameObject);
        }
    }
}
