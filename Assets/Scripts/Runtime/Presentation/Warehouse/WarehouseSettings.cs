using CvWarehouse.Core.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    [CreateAssetMenu(menuName = "CV Warehouse/Warehouse Settings", fileName = "WarehouseSettings")]
    public sealed class WarehouseSettings : ScriptableObject
    {
        [SerializeField] private int ticksPerSecond = 60;
        [SerializeField] private int maxTicksPerFrame = 5;
        [SerializeField] private int ticksPerCell = 8;
        [SerializeField] private int extractTicks = 30;
        [SerializeField] private int unloadTicks = 20;
        [SerializeField] private int letterPieces = 1;
        [SerializeField] private int wordPieces = 2;
        [SerializeField] private int lightPiecePoints = 1;
        [SerializeField] private int mediumPiecePoints = 2;
        [SerializeField] private int heavyPiecePoints = 3;
        [SerializeField] private int startingBots = 3;

        [Header("Shop prices, in thousandths of all the points in the warehouse")]
        [SerializeField] private int firstBotPricePermille = 15;
        [SerializeField] private int botPriceGrowthPercent = 125;
        [SerializeField] private int wordCarryPricePermille = 30;
        [SerializeField] private int boxCarryPricePermille = 120;

        public int StartingBots => startingBots;

        public ShopSettings CreateShopSettings()
        {
            return new ShopSettings
            {
                FreeBots = startingBots,
                FirstBotPricePermille = firstBotPricePermille,
                BotPriceGrowthPercent = botPriceGrowthPercent,
                WordCarryPricePermille = wordCarryPricePermille,
                BoxCarryPricePermille = boxCarryPricePermille
            };
        }

        public SimulationSettings CreateSimulationSettings()
        {
            return new SimulationSettings
            {
                TickSeconds = 1f / ticksPerSecond,
                MaxTicksPerAdvance = maxTicksPerFrame,
                TicksPerCell = ticksPerCell,
                ExtractTicks = extractTicks,
                UnloadTicks = unloadTicks,
                LetterPieces = letterPieces,
                WordPieces = wordPieces,
                LightPiecePoints = lightPiecePoints,
                MediumPiecePoints = mediumPiecePoints,
                HeavyPiecePoints = heavyPiecePoints
            };
        }
    }
}
