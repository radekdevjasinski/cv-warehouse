using CvWarehouse.Core.Cv;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class GeneratorSettings
    {
        public int FillPercent { get; set; } = 6;

        public int WidthToHeightPercent { get; set; } = 150;

        public int MinWidth { get; set; } = 30;

        public int MinStorageHeight { get; set; } = 12;

        public int TruckWidth { get; set; } = 10;

        public int DropCells { get; set; } = 6;

        public int ParkingPerSide { get; set; } = 6;

        public int RampDepth { get; set; } = 4;

        public WeightClass NameWeight { get; set; } = WeightClass.Heavy;

        public WeightClass JobTitleWeight { get; set; } = WeightClass.Medium;

        public WeightClass ContactWeight { get; set; } = WeightClass.Light;

        public int OpeningRadius { get; set; } = 4;

        public int BigClusterRadius { get; set; } = 6;

        public int MediumClusterRadius { get; set; } = 3;

        public int MediumNearHeavyPercent { get; set; } = 60;

        public int LightNearHeavyPercent { get; set; } = 35;

        public int LightNearMediumPercent { get; set; } = 45;

        public WeightClass WeightOf(CvBlock block)
        {
            switch (block.Role)
            {
                case CvBlockRole.Name:
                    return NameWeight;
                case CvBlockRole.JobTitle:
                    return JobTitleWeight;
                case CvBlockRole.Contact:
                    return ContactWeight;
                default:
                    return block.Weight;
            }
        }
    }
}
