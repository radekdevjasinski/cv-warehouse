using System;
using System.Collections.Generic;
using CvWarehouse.Core.Cv;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class WarehouseScore
    {
        private readonly SimulationSettings settings;

        public WarehouseScore(SimulationSettings settings, IReadOnlyList<Box> boxes)
        {
            this.settings = settings;
            foreach (Box box in boxes)
                MaxPoints += PointsIn(box);
        }

        public event Action<CvBlock, IReadOnlyList<int>> Delivered;

        public int MaxPoints { get; }

        public int DeliveredPieces { get; private set; }

        public int Points { get; private set; }

        public int PointsIn(Box box)
        {
            return box.TotalPieces * settings.PointsPerPiece(box.Importance);
        }

        public int Deliver(Box box, IReadOnlyList<int> letters)
        {
            if (letters.Count == 0)
                return 0;

            int points = letters.Count * settings.PointsPerPiece(box.Importance);
            DeliveredPieces += letters.Count;
            Points += points;
            Delivered?.Invoke(box.Block, letters);
            return points;
        }
    }
}
