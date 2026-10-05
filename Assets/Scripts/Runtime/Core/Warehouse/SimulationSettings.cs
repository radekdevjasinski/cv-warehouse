using CvWarehouse.Core.Cv;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class SimulationSettings
    {
        public float TickSeconds { get; set; } = 1f / 60f;

        public int MaxTicksPerAdvance { get; set; } = 5;

        public int TicksPerCell { get; set; } = 8;

        public int ExtractTicks { get; set; } = 30;

        public int UnloadTicks { get; set; } = 20;

        public int CarryPieces { get; set; } = 1;

        public int LightPiecePoints { get; set; } = 1;

        public int MediumPiecePoints { get; set; } = 2;

        public int HeavyPiecePoints { get; set; } = 3;

        public int PointsPerPiece(WeightClass weightClass)
        {
            switch (weightClass)
            {
                case WeightClass.Heavy:
                    return HeavyPiecePoints;
                case WeightClass.Medium:
                    return MediumPiecePoints;
                default:
                    return LightPiecePoints;
            }
        }
    }
}
