using CvWarehouse.Core.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Audio
{
    public sealed class MuteButton : MonoBehaviour
    {
        [SerializeField] private AudioSessionChannel channel;
        [SerializeField] private Button button;
        [SerializeField] private Image icon;
        [SerializeField] private Sprite soundOnIcon;
        [SerializeField] private Sprite soundOffIcon;

        private SoundSettings settings;

        private void OnEnable()
        {
            settings = channel.Session.Settings;
            settings.Changed += Refresh;
            button.onClick.AddListener(ToggleMute);
            Refresh();
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(ToggleMute);
            settings.Changed -= Refresh;
        }

        private void ToggleMute()
        {
            settings.ToggleMute();
            settings.Save();
        }

        private void Refresh()
        {
            icon.sprite = settings.IsMuted ? soundOffIcon : soundOnIcon;
        }
    }
}
