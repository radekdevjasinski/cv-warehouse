using System;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Warehouse
{
    [Serializable]
    public sealed class UpgradeButtonLook
    {
        public const string MaxLabel = "Max";

        [SerializeField] private Sprite affordableSprite;
        [SerializeField] private Sprite tooExpensiveSprite;
        [SerializeField] private Sprite maxedSprite;

        public static UpgradeButtonState StateOf(bool hasUpgrade, bool canUpgrade)
        {
            if (!hasUpgrade)
                return UpgradeButtonState.Maxed;

            return canUpgrade ? UpgradeButtonState.Affordable : UpgradeButtonState.TooExpensive;
        }

        public void Apply(Button button, UpgradeButtonState state)
        {
            bool isInteractable = state == UpgradeButtonState.Affordable;
            if (button.interactable != isInteractable)
                button.interactable = isInteractable;

            Sprite sprite = SpriteOf(state);
            if (button.image.sprite != sprite)
                button.image.sprite = sprite;
        }

        private Sprite SpriteOf(UpgradeButtonState state)
        {
            switch (state)
            {
                case UpgradeButtonState.Affordable:
                    return affordableSprite;
                case UpgradeButtonState.Maxed:
                    return maxedSprite;
                default:
                    return tooExpensiveSprite;
            }
        }
    }
}
