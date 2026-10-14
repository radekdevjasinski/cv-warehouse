using CvWarehouse.Core.Cv;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class GeneratorSettings
    {
        public int FillPercent { get; set; } = 18;

        public int WidthToHeightPercent { get; set; } = 150;

        public int MinWidth { get; set; } = 22;

        public int MinStorageHeight { get; set; } = 12;

        public int TruckWidth { get; set; } = 10;

        public int DropCells { get; set; } = 6;

        public int ParkingPerSide { get; set; } = 4;

        public int RampDepth { get; set; } = 3;

        public Importance NameImportance { get; set; } = Importance.Key;

        public Importance JobTitleImportance { get; set; } = Importance.Important;

        public Importance ContactImportance { get; set; } = Importance.Normal;

        public int SmallBoxMaxWords { get; set; } = 8;

        public int MediumBoxMaxWords { get; set; } = 20;

        public int OpeningRadius { get; set; } = 4;

        public int BigClusterRadius { get; set; } = 6;

        public int MediumClusterRadius { get; set; } = 3;

        public int MediumNearLargePercent { get; set; } = 60;

        public int SmallNearLargePercent { get; set; } = 35;

        public int SmallNearMediumPercent { get; set; } = 45;

        public BoxKind KindOf(CvBlock block)
        {
            return new BoxKind(SizeClassOf(block), ImportanceOf(block));
        }

        public BoxSizeClass SizeClassOf(CvBlock block)
        {
            int words = block.Words.Count;
            if (words <= SmallBoxMaxWords)
                return BoxSizeClass.Small;

            return words <= MediumBoxMaxWords ? BoxSizeClass.Medium : BoxSizeClass.Large;
        }

        public Importance ImportanceOf(CvBlock block)
        {
            switch (block.Role)
            {
                case CvBlockRole.Name:
                    return NameImportance;
                case CvBlockRole.JobTitle:
                    return JobTitleImportance;
                case CvBlockRole.Contact:
                    return ContactImportance;
                default:
                    return block.Importance;
            }
        }
    }
}
