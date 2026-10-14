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

        public int TicksPerCell { get; set; } = 16;

        public int SpeedStepPercent { get; set; } = 25;

        public int NormalExtractTicks { get; set; } = 30;

        public int ImportantExtractTicks { get; set; } = 60;

        public int KeyExtractTicks { get; set; } = 120;

        public int UnloadTicks { get; set; } = 20;

        public int NormalPiecePoints { get; set; } = 1;

        public int ImportantPiecePoints { get; set; } = 2;

        public int KeyPiecePoints { get; set; } = 3;

        public IReadOnlyList<string> CrewNames { get; set; } = Array.Empty<string>();

        public int StepTicks(int speedLevel, int stepCost)
        {
            int speedPercent = Percent + (speedLevel - Bot.FirstLevel) * SpeedStepPercent;
            return Math.Max(ShortestStepTicks, TicksPerCell * stepCost * Percent / (GridDirections.StraightCost * speedPercent));
        }

        public int ExtractTicks(Importance importance)
        {
            switch (importance)
            {
                case Importance.Key:
                    return KeyExtractTicks;
                case Importance.Important:
                    return ImportantExtractTicks;
                default:
                    return NormalExtractTicks;
            }
        }

        public int PointsPerPiece(Importance importance)
        {
            switch (importance)
            {
                case Importance.Key:
                    return KeyPiecePoints;
                case Importance.Important:
                    return ImportantPiecePoints;
                default:
                    return NormalPiecePoints;
            }
        }
    }
}
