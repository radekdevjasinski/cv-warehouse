using CvWarehouse.Core.Menu;
using CvWarehouse.Presentation.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Menu
{
    [CreateAssetMenu(menuName = "CV Warehouse/Menu Backdrop Config", fileName = "MenuBackdropConfig")]
    public sealed class MenuBackdropConfig : ScriptableObject
    {
        [Header("Bot")]
        [SerializeField] private BotSkin skin;
        [SerializeField] private BotAnimation botAnimation;
        [SerializeField] private Color botColor = new Color32(0x7b, 0xd3, 0x89, 0xff);
        [SerializeField] private int animationTicksPerSecond = 60;

        [Header("Loop, in seconds")]
        [SerializeField] private float takeOutSeconds = 0.5f;
        [SerializeField] private float walkSeconds = 1.6f;
        [SerializeField] private float putInSeconds = 0.35f;
        [SerializeField] private int[] cargoWordsPerTrip = { 1, 2, 3, 2 };

        public BotSkin Skin => skin;

        public BotAnimation BotAnimation => botAnimation;

        public Color BotColor => botColor;

        public int AnimationTicksPerSecond => animationTicksPerSecond;

        public MenuBotLoopSettings CreateLoopSettings()
        {
            return new MenuBotLoopSettings
            {
                TakeOutSeconds = takeOutSeconds,
                WalkSeconds = walkSeconds,
                PutInSeconds = putInSeconds,
                CargoWordsPerTrip = (int[])cargoWordsPerTrip.Clone()
            };
        }
    }
}
