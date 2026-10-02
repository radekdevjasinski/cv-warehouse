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
        [SerializeField] private int repathWaitTicks = 15;
        [SerializeField] private int sidestepWaitTicks = 60;
        [SerializeField] private int carryPieces = 1;
        [SerializeField] private int lightPiecePoints = 1;
        [SerializeField] private int mediumPiecePoints = 2;
        [SerializeField] private int heavyPiecePoints = 3;
        [SerializeField] private int startingBots = 3;

        public int StartingBots => startingBots;

        public SimulationSettings CreateSimulationSettings()
        {
            return new SimulationSettings
            {
                TickSeconds = 1f / ticksPerSecond,
                MaxTicksPerAdvance = maxTicksPerFrame,
                TicksPerCell = ticksPerCell,
                ExtractTicks = extractTicks,
                UnloadTicks = unloadTicks,
                RepathWaitTicks = repathWaitTicks,
                SidestepWaitTicks = sidestepWaitTicks,
                CarryPieces = carryPieces,
                LightPiecePoints = lightPiecePoints,
                MediumPiecePoints = mediumPiecePoints,
                HeavyPiecePoints = heavyPiecePoints
            };
        }
    }
}
