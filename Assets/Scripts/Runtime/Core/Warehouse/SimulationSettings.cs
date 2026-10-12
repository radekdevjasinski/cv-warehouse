using System;
using System.Collections.Generic;
using CvWarehouse.Core.Cv;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class SimulationSettings
    {
        private const int Percent = 100;
        private const int ShortestStepTicks = 1;

        public float TickSeconds { get; set; } = 1f / 60f;

        public int MaxTicksPerAdvance { get; set; } = 5;

        public int TicksPerCell { get; set; } = 8;

        public int SpeedStepPercent { get; set; } = 25;

        public int LightExtractTicks { get; set; } = 20;

        public int MediumExtractTicks { get; set; } = 30;

        public int HeavyExtractTicks { get; set; } = 45;

        public int UnloadTicks { get; set; } = 20;

        public int LightPiecePoints { get; set; } = 1;

        public int MediumPiecePoints { get; set; } = 2;

        public int HeavyPiecePoints { get; set; } = 3;

        public IReadOnlyList<string> CrewNames { get; set; } = Array.Empty<string>();

        public int StepTicks(int speedLevel, int stepCost)
        {
            int speedPercent = Percent + (speedLevel - Bot.FirstLevel) * SpeedStepPercent;
            return Math.Max(ShortestStepTicks, TicksPerCell * stepCost * Percent / (GridDirections.StraightCost * speedPercent));
        }

        public int ExtractTicks(WeightClass weightClass)
        {
            switch (weightClass)
            {
                case WeightClass.Heavy:
                    return HeavyExtractTicks;
                case WeightClass.Medium:
                    return MediumExtractTicks;
                default:
                    return LightExtractTicks;
            }
        }

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
