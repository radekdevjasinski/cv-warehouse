using System;
using System.Collections.Generic;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class DistanceField
    {
        public const int Unreachable = int.MaxValue;

        private readonly WarehouseGrid grid;
        private readonly IReadOnlyList<GridPosition> sources;
        private readonly CellHeap openCells;
        private readonly int[] distances;

        public DistanceField(WarehouseGrid grid, IReadOnlyList<GridPosition> sources)
        {
            this.grid = grid;
            this.sources = sources;
            openCells = new CellHeap(grid.CellCount);
            distances = new int[grid.CellCount];
            Recompute();
        }

        public int GetDistance(GridPosition position)
        {
            return grid.Contains(position) ? distances[grid.IndexOf(position)] : Unreachable;
        }

        public bool IsReachable(GridPosition position)
        {
            return GetDistance(position) != Unreachable;
        }

        public void Recompute()
        {
            Array.Fill(distances, Unreachable);
            openCells.Clear();
            for (int index = 0; index < sources.Count; index++)
            {
                if (!grid.IsWalkable(sources[index]))
                    continue;

                distances[grid.IndexOf(sources[index])] = 0;
                openCells.Push(grid.IndexOf(sources[index]), 0);
            }

            while (openCells.Count > 0)
            {
                int cell = openCells.Pop(out int distance);
                if (distance == distances[cell])
                    SpreadFrom(cell);
            }
        }

        private void SpreadFrom(int cell)
        {
            GridPosition position = grid.PositionOf(cell);
            for (int direction = 0; direction < GridDirections.Count; direction++)
            {
                if (!grid.CanStep(position, direction))
                    continue;

                int neighbourCell = grid.IndexOf(GridDirections.Step(position, direction));
                int distance = distances[cell] + GridDirections.CostOf(direction);
                if (distance >= distances[neighbourCell])
                    continue;

                distances[neighbourCell] = distance;
                openCells.Push(neighbourCell, distance);
            }
        }
    }
}
