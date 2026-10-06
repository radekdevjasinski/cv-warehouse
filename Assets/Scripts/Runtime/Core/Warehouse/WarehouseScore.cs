using System.Collections.Generic;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class WarehouseScore
    {
        private readonly SimulationSettings settings;

        public WarehouseScore(SimulationSettings settings, IReadOnlyList<Box> boxes)
        {
            this.settings = settings;
            foreach (Box box in boxes)
                MaxPoints += box.TotalPieces * settings.PointsPerPiece(box.WeightClass);
        }

        public int MaxPoints { get; }

        public int DeliveredPieces { get; private set; }

        public int Points { get; private set; }

        public void Deliver(Box box, int pieces)
        {
            DeliveredPieces += pieces;
            Points += pieces * settings.PointsPerPiece(box.WeightClass);
        }
    }
}
