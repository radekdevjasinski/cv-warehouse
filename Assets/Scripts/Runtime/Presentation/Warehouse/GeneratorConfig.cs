using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using UnityEngine;
using UnityEngine.Serialization;

namespace CvWarehouse.Presentation.Warehouse
{
    [CreateAssetMenu(menuName = "CV Warehouse/Generator Config", fileName = "GeneratorConfig")]
    public sealed class GeneratorConfig : ScriptableObject
    {
        [Header("Map, sized to the boxes of the CV")]
        [SerializeField, Range(1, 50)] private int fillPercent = 18;
        [SerializeField] private int widthToHeightPercent = 150;
        [SerializeField] private int minWidth = 22;
        [SerializeField] private int minStorageHeight = 12;
        [SerializeField] private int truckWidth = 10;
        [SerializeField] private int dropCells = 6;
        [SerializeField] private int parkingPerSide = 6;
        [SerializeField] private int rampDepth = 4;

        [Header("Boxes that open the CV")]
        [SerializeField, FormerlySerializedAs("nameWeight")] private Importance nameImportance = Importance.Key;
        [SerializeField, FormerlySerializedAs("jobTitleWeight")] private Importance jobTitleImportance = Importance.Important;
        [SerializeField, FormerlySerializedAs("contactWeight")] private Importance contactImportance = Importance.Normal;
        [SerializeField] private int openingRadius = 4;

        [Header("Box size, by the words of its block")]
        [SerializeField] private int smallBoxMaxWords = 8;
        [SerializeField] private int mediumBoxMaxWords = 20;

        [Header("Clusters")]
        [SerializeField] private int bigClusterRadius = 6;
        [SerializeField] private int mediumClusterRadius = 3;
        [SerializeField, Range(0, 100), FormerlySerializedAs("mediumNearHeavyPercent")] private int mediumNearLargePercent = 60;
        [SerializeField, Range(0, 100), FormerlySerializedAs("lightNearHeavyPercent")] private int smallNearLargePercent = 35;
        [SerializeField, Range(0, 100), FormerlySerializedAs("lightNearMediumPercent")] private int smallNearMediumPercent = 45;

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
                NameImportance = nameImportance,
                JobTitleImportance = jobTitleImportance,
                ContactImportance = contactImportance,
                SmallBoxMaxWords = smallBoxMaxWords,
                MediumBoxMaxWords = mediumBoxMaxWords,
                OpeningRadius = openingRadius,
                BigClusterRadius = bigClusterRadius,
                MediumClusterRadius = mediumClusterRadius,
                MediumNearLargePercent = mediumNearLargePercent,
                SmallNearLargePercent = smallNearLargePercent,
                SmallNearMediumPercent = smallNearMediumPercent
            };
        }
    }
}
