using System;
using System.Collections.Generic;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class ReservationTable
    {
        private const int NoOwner = -1;
        private const int NotMeasured = -1;

        private readonly WarehouseGrid grid;
        private readonly int[] ownerAtCell;
        private readonly List<GridPosition> walkStart = new List<GridPosition> { default };
        private readonly DistanceField walkDistances;
        private int measuredGridVersion = NotMeasured;

        public ReservationTable(WarehouseGrid grid)
        {
            this.grid = grid;
            ownerAtCell = new int[grid.CellCount];
            Array.Fill(ownerAtCell, NoOwner);
            walkDistances = new DistanceField(grid, walkStart);
        }

        public bool IsReserved(GridPosition position)
        {
            return ownerAtCell[grid.IndexOf(position)] != NoOwner;
        }

        public bool TryReserve(GridPosition position, int botId)
        {
            if (IsReserved(position))
                return false;

            ownerAtCell[grid.IndexOf(position)] = botId;
            return true;
        }

        public bool TryReserveNearest(IReadOnlyList<GridPosition> candidates, Bot bot, out GridPosition reserved)
        {
            reserved = default;
            if (!HasFreeCandidate(candidates))
                return false;

            MeasureWalksFrom(bot.IsMoving ? bot.NextCell : bot.Cell);
            int nearest = -1;
            int nearestDistance = DistanceField.Unreachable;
            for (int candidate = 0; candidate < candidates.Count; candidate++)
            {
                int distance = walkDistances.GetDistance(candidates[candidate]);
                if (distance >= nearestDistance || IsReserved(candidates[candidate]))
                    continue;

                nearest = candidate;
                nearestDistance = distance;
            }

            if (nearest < 0)
                return false;

            reserved = candidates[nearest];
            return TryReserve(reserved, bot.Id);
        }

        public void Release(GridPosition position, int botId)
        {
            int cellIndex = grid.IndexOf(position);
            if (ownerAtCell[cellIndex] == botId)
                ownerAtCell[cellIndex] = NoOwner;
        }

        public void ReleaseAll(int botId)
        {
            for (int cellIndex = 0; cellIndex < ownerAtCell.Length; cellIndex++)
                if (ownerAtCell[cellIndex] == botId)
                    ownerAtCell[cellIndex] = NoOwner;
        }

        private bool HasFreeCandidate(IReadOnlyList<GridPosition> candidates)
        {
            for (int candidate = 0; candidate < candidates.Count; candidate++)
                if (!IsReserved(candidates[candidate]))
                    return true;
            return false;
        }

        private void MeasureWalksFrom(GridPosition start)
        {
            if (walkStart[0] == start && measuredGridVersion == grid.Version)
                return;

            walkStart[0] = start;
            walkDistances.Recompute();
            measuredGridVersion = grid.Version;
        }
    }
}
