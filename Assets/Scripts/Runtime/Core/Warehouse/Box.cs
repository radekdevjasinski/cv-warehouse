using System;
using System.Collections.Generic;
using CvWarehouse.Core.Cv;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class Box
    {
        private readonly List<GridPosition> accessCells = new List<GridPosition>();
        private readonly BoxLetters letters;

        public Box(int id, GridPosition origin, WeightClass weightClass, CvBlock block)
        {
            Id = id;
            Origin = origin;
            WeightClass = weightClass;
            Block = block;
            Size = BoxFootprints.For(weightClass);
            letters = new BoxLetters(block);
            TotalPieces = block.LetterCount;
            RemainingPieces = block.LetterCount;
            TruckDistance = DistanceField.Unreachable;
        }

        public int Id { get; }

        public GridPosition Origin { get; }

        public GridSize Size { get; }

        public WeightClass WeightClass { get; }

        public CvBlock Block { get; }

        public int TotalPieces { get; }

        public int RemainingPieces { get; private set; }

        public int TruckDistance { get; private set; }

        public IReadOnlyList<GridPosition> AccessCells => accessCells;

        public int UnclaimedPieces => letters.UnclaimedCount;

        public bool IsEmpty => RemainingPieces == 0;

        public int Claim(int words, Random random, List<int> claimed)
        {
            claimed.Clear();
            for (int word = 0; word < words && UnclaimedPieces > 0; word++)
                letters.ClaimWord(random, claimed);
            return claimed.Count;
        }

        public void ReleaseClaim(List<int> claimed)
        {
            letters.Release(claimed);
            claimed.Clear();
        }

        public void TakeClaimed(int pieces)
        {
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
