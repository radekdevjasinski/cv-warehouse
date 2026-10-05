using System;
using System.Collections.Generic;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class ReservationTable
    {
        private const int NoOwner = -1;

        private readonly WarehouseGrid grid;
        private readonly int[] ownerAtCell;

        public ReservationTable(WarehouseGrid grid)
        {
            this.grid = grid;
            ownerAtCell = new int[grid.CellCount];
            Array.Fill(ownerAtCell, NoOwner);
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
            int nearest = -1;
            int nearestDistance = int.MaxValue;
            for (int candidate = 0; candidate < candidates.Count; candidate++)
            {
                int distance = GridDirections.OctileDistance(bot.Cell, candidates[candidate]);
                if (distance >= nearestDistance || IsReserved(candidates[candidate]))
                    continue;

                nearest = candidate;
                nearestDistance = distance;
            }

            reserved = nearest < 0 ? default : candidates[nearest];
            return nearest >= 0 && TryReserve(reserved, bot.Id);
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
    }
}
