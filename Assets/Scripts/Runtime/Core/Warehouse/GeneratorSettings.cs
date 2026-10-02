namespace CvWarehouse.Core.Warehouse
{
    public sealed class GeneratorSettings
    {
        public int Width { get; set; } = 80;

        public int Height { get; set; } = 56;

        public int TruckWidth { get; set; } = 10;

        public int DropCells { get; set; } = 6;

        public int ParkingPerSide { get; set; } = 6;

        public int RampDepth { get; set; } = 4;

        public int HeavyBoxes { get; set; } = 10;

        public int MediumBoxes { get; set; } = 30;

        public int LightBoxes { get; set; } = 90;

        public int HeavyPieces { get; set; } = 6;

        public int MediumPieces { get; set; } = 4;

        public int LightPieces { get; set; } = 2;

        public int BigClusterRadius { get; set; } = 6;

        public int MediumClusterRadius { get; set; } = 3;

        public int MediumNearHeavyPercent { get; set; } = 60;

        public int LightNearHeavyPercent { get; set; } = 35;

        public int LightNearMediumPercent { get; set; } = 45;
    }
}
