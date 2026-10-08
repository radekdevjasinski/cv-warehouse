using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    [CreateAssetMenu(menuName = "CV Warehouse/Generator Config", fileName = "GeneratorConfig")]
    public sealed class GeneratorConfig : ScriptableObject
    {
        [Header("Map, sized to the boxes of the CV")]
        [SerializeField, Range(1, 50)] private int fillPercent = 6;
        [SerializeField] private int widthToHeightPercent = 150;
        [SerializeField] private int minWidth = 30;
        [SerializeField] private int minStorageHeight = 12;
        [SerializeField] private int truckWidth = 10;
        [SerializeField] private int dropCells = 6;
        [SerializeField] private int parkingPerSide = 6;
        [SerializeField] private int rampDepth = 4;

        [Header("Boxes that open the CV")]
        [SerializeField] private WeightClass nameWeight = WeightClass.Heavy;
        [SerializeField] private WeightClass jobTitleWeight = WeightClass.Medium;
        [SerializeField] private WeightClass contactWeight = WeightClass.Light;
        [SerializeField] private int openingRadius = 4;

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
                FillPercent = fillPercent,
                WidthToHeightPercent = widthToHeightPercent,
                MinWidth = minWidth,
                MinStorageHeight = minStorageHeight,
                TruckWidth = truckWidth,
                DropCells = dropCells,
                ParkingPerSide = parkingPerSide,
                RampDepth = rampDepth,
                NameWeight = nameWeight,
                JobTitleWeight = jobTitleWeight,
                ContactWeight = contactWeight,
                OpeningRadius = openingRadius,
                BigClusterRadius = bigClusterRadius,
                MediumClusterRadius = mediumClusterRadius,
                MediumNearHeavyPercent = mediumNearHeavyPercent,
                LightNearHeavyPercent = lightNearHeavyPercent,
                LightNearMediumPercent = lightNearMediumPercent
            };
        }
    }
}
