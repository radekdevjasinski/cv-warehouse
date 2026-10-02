using System;
using System.Collections.Generic;
using CvWarehouse.Core.Cv;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class Box
    {
        private readonly List<GridPosition> accessCells = new List<GridPosition>();

        public Box(int id, GridPosition origin, WeightClass weightClass, int pieceCount)
        {
            Id = id;
            Origin = origin;
            WeightClass = weightClass;
            Size = BoxFootprints.For(weightClass);
            TotalPieces = pieceCount;
            RemainingPieces = pieceCount;
            TruckDistance = DistanceField.Unreachable;
        }

        public int Id { get; }

        public GridPosition Origin { get; }

        public GridSize Size { get; }

        public WeightClass WeightClass { get; }

        public int TotalPieces { get; }

        public int RemainingPieces { get; private set; }

        public int ClaimedPieces { get; private set; }

        public int TruckDistance { get; private set; }

        public IReadOnlyList<GridPosition> AccessCells => accessCells;

        public int UnclaimedPieces => RemainingPieces - ClaimedPieces;

        public bool IsEmpty => RemainingPieces == 0;

        public int Claim(int wantedPieces)
        {
            int claimed = Math.Min(wantedPieces, UnclaimedPieces);
            ClaimedPieces += claimed;
            return claimed;
        }

        public void ReleaseClaim(int pieces)
        {
            ClaimedPieces -= pieces;
        }

        public void TakeClaimed(int pieces)
        {
            ClaimedPieces -= pieces;
            RemainingPieces -= pieces;
        }

        internal void ClearAccessCells()
        {
            accessCells.Clear();
            TruckDistance = DistanceField.Unreachable;
        }

        internal void AddAccessCell(GridPosition accessCell, int truckDistance)
        {
            if (accessCells.Contains(accessCell))
                return;

            accessCells.Add(accessCell);
            TruckDistance = Math.Min(TruckDistance, truckDistance);
        }
    }
}
