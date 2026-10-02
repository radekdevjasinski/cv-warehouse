using CvWarehouse.Core.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    [CreateAssetMenu(menuName = "CV Warehouse/Generator Config", fileName = "GeneratorConfig")]
    public sealed class GeneratorConfig : ScriptableObject
    {
        [Header("Map")]
        [SerializeField] private int width = 80;
        [SerializeField] private int height = 56;
        [SerializeField] private int truckWidth = 10;
        [SerializeField] private int dropCells = 6;
        [SerializeField] private int parkingPerSide = 6;
        [SerializeField] private int rampDepth = 4;

        [Header("Boxes")]
        [SerializeField] private int heavyBoxes = 10;
        [SerializeField] private int mediumBoxes = 30;
        [SerializeField] private int lightBoxes = 90;
        [SerializeField] private int heavyPieces = 6;
        [SerializeField] private int mediumPieces = 4;
        [SerializeField] private int lightPieces = 2;

        [Header("Clusters")]
        [SerializeField] private int bigClusterRadius = 6;
        [SerializeField] private int mediumClusterRadius = 3;
        [SerializeField, Range(0, 100)] private int mediumNearHeavyPercent = 60;
        [SerializeField, Range(0, 100)] private int lightNearHeavyPercent = 35;
        [SerializeField, Range(0, 100)] private int lightNearMediumPercent = 45;

        public GeneratorSettings CreateGeneratorSettings()
        {
            return new GeneratorSettings
            {
                Width = width,
                Height = height,
                TruckWidth = truckWidth,
                DropCells = dropCells,
                ParkingPerSide = parkingPerSide,
                RampDepth = rampDepth,
                HeavyBoxes = heavyBoxes,
                MediumBoxes = mediumBoxes,
                LightBoxes = lightBoxes,
                HeavyPieces = heavyPieces,
                MediumPieces = mediumPieces,
                LightPieces = lightPieces,
                BigClusterRadius = bigClusterRadius,
                MediumClusterRadius = mediumClusterRadius,
                MediumNearHeavyPercent = mediumNearHeavyPercent,
                LightNearHeavyPercent = lightNearHeavyPercent,
                LightNearMediumPercent = lightNearMediumPercent
            };
        }
    }
}
