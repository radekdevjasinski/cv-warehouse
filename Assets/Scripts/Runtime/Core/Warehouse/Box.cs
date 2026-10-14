using System;
using System.Collections.Generic;
using CvWarehouse.Core.Cv;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class Box
    {
        private readonly List<GridPosition> accessCells = new List<GridPosition>();
        private readonly BoxLetters letters;

        public Box(int id, GridPosition origin, BoxKind kind, CvBlock block)
        {
            Id = id;
            Origin = origin;
            SizeClass = kind.SizeClass;
            Importance = kind.Importance;
            Block = block;
            Size = BoxFootprints.For(kind.SizeClass);
            letters = new BoxLetters(block);
            TotalPieces = block.LetterCount;
            RemainingPieces = block.LetterCount;
            TruckDistance = DistanceField.Unreachable;
        }

        public int Id { get; }

        public GridPosition Origin { get; }

        public GridSize Size { get; }

        public BoxSizeClass SizeClass { get; }

        public Importance Importance { get; }

        public CvBlock Block { get; }

        public int TotalPieces { get; }

        public int RemainingPieces { get; private set; }

        public int TruckDistance { get; private set; }

        public IReadOnlyList<GridPosition> AccessCells => accessCells;

        public int UnclaimedPieces => letters.UnclaimedCount;

        public int OpenWords => letters.CountOpenWords();

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
