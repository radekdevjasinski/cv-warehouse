using CvWarehouse.Core.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Audio
{
    public sealed class SoundSettingsPanel : MonoBehaviour
    {
        private const int WholePercent = 100;

        [SerializeField] private AudioSessionChannel channel;
        [SerializeField] private Button openButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject panel;
        [SerializeField] private GameObject hiddenWhileOpen;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private TMP_Text musicValueText;
        [SerializeField] private TMP_Text sfxValueText;
        [SerializeField] private SliderReleaseRelay sfxSliderRelease;
        [SerializeField] private BotVoicePlayer sfxPreview;

        private SoundSettings settings;

        public bool IsOpen => panel.activeSelf;

        public void Open()
        {
            panel.SetActive(true);
            hiddenWhileOpen.SetActive(false);
            musicSlider.SetValueWithoutNotify(settings.MusicVolume);
            sfxSlider.SetValueWithoutNotify(settings.SfxVolume);
            ShowPercent(musicValueText, settings.MusicVolume);
            ShowPercent(sfxValueText, settings.SfxVolume);
        }

        public void Close()
        {
            panel.SetActive(false);
            hiddenWhileOpen.SetActive(true);
            settings.Save();
        }

        private void Awake()
        {
            panel.SetActive(false);
        }

        private void OnEnable()
        {
            settings = channel.Session.Settings;
            openButton.onClick.AddListener(Open);
            closeButton.onClick.AddListener(Close);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);
            sfxSliderRelease.Released += sfxPreview.PlayRandomVoice;
        }

        private void OnDisable()
        {
            sfxSliderRelease.Released -= sfxPreview.PlayRandomVoice;
            sfxSlider.onValueChanged.RemoveListener(SetSfxVolume);
            musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
            closeButton.onClick.RemoveListener(Close);
            openButton.onClick.RemoveListener(Open);
        }

        private void SetMusicVolume(float volume)
        {
            settings.SetMusicVolume(volume);
            ShowPercent(musicValueText, settings.MusicVolume);
        }

        private void SetSfxVolume(float volume)
        {
            settings.SetSfxVolume(volume);
            ShowPercent(sfxValueText, settings.SfxVolume);
        }

        private static void ShowPercent(TMP_Text text, float volume)
        {
            text.SetText("{0}%", Mathf.RoundToInt(volume * WholePercent));
        }
    }
}
