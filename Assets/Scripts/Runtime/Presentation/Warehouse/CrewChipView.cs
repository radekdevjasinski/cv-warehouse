using System;
using CvWarehouse.Core.Warehouse;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class CrewChipView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image background;
        [SerializeField] private Image swatch;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite selectedSprite;

        public event Action<Bot> Picked;

        public Bot Bot { get; private set; }

        public bool IsSelected { get; private set; }

        public string ShownName => nameText.text;

        public void Show(Bot bot, Color color, bool isSelected)
        {
            if (Bot != bot)
            {
                Bot = bot;
                nameText.SetText(bot.Name);
                swatch.color = color;
            }

            IsSelected = isSelected;
            Sprite sprite = isSelected ? selectedSprite : normalSprite;
            if (background.sprite != sprite)
                background.sprite = sprite;
        }

        public void Pick()
        {
            if (Bot != null)
                Picked?.Invoke(Bot);
        }

        private void OnEnable()
        {
            button.onClick.AddListener(Pick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(Pick);
        }
    }
}
