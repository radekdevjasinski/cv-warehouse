using System.Collections.Generic;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class Pathfinder
    {
        private readonly WarehouseGrid grid;
        private readonly CellHeap openCells;
        private readonly int[] costFromStart;
        private readonly int[] cameFrom;
        private readonly int[] reachedInSearch;
        private readonly int[] closedInSearch;
        private int searchId;

        public Pathfinder(WarehouseGrid grid)
        {
            this.grid = grid;
            openCells = new CellHeap(grid.CellCount * GridDirections.Count);
            costFromStart = new int[grid.CellCount];
            cameFrom = new int[grid.CellCount];
            reachedInSearch = new int[grid.CellCount];
            closedInSearch = new int[grid.CellCount];
        }

        public bool TryFindPath(PathRequest request, List<GridPosition> path)
        {
            path.Clear();
            if (!grid.IsWalkable(request.Start) || !grid.IsWalkable(request.Goal))
                return false;

            int startCell = grid.IndexOf(request.Start);
            int goalCell = grid.IndexOf(request.Goal);
            BeginSearch(startCell);
            while (openCells.Count > 0)
            {
                int cell = openCells.Pop(out _);
                if (cell == goalCell)
                {
                    BuildPath(startCell, goalCell, path);
                    return true;
                }

                if (closedInSearch[cell] == searchId)
                    continue;

                closedInSearch[cell] = searchId;
                ExpandNeighbours(cell, request.Goal);
            }

            return false;
        }

        private void BeginSearch(int startCell)
        {
            searchId++;
            openCells.Clear();
            costFromStart[startCell] = 0;
            reachedInSearch[startCell] = searchId;
            openCells.Push(startCell, 0);
        }

        private void ExpandNeighbours(int cell, GridPosition goal)
        {
            GridPosition position = grid.PositionOf(cell);
            for (int direction = 0; direction < GridDirections.Count; direction++)
            {
                if (!grid.CanStep(position, direction))
                    continue;

                GridPosition neighbour = GridDirections.Step(position, direction);
                int neighbourCell = grid.IndexOf(neighbour);
                int cost = costFromStart[cell] + GridDirections.CostOf(direction);
                if (reachedInSearch[neighbourCell] == searchId && cost >= costFromStart[neighbourCell])
                    continue;

                reachedInSearch[neighbourCell] = searchId;
                costFromStart[neighbourCell] = cost;
                cameFrom[neighbourCell] = cell;
                openCells.Push(neighbourCell, cost + GridDirections.OctileDistance(neighbour, goal));
            }
        }

        private void BuildPath(int startCell, int goalCell, List<GridPosition> path)
        {
            for (int cell = goalCell; cell != startCell; cell = cameFrom[cell])
                path.Add(grid.PositionOf(cell));
            path.Reverse();
        }
    }
}
