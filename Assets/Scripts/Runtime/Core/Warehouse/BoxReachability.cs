using System.Collections.Generic;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class BoxReachability
    {
        private readonly WarehouseGrid grid;
        private readonly IReadOnlyList<Box> boxes;
        private readonly IReadOnlyList<GridPosition> dropCells;

        public BoxReachability(WarehouseGrid grid, IReadOnlyList<Box> boxes, IReadOnlyList<GridPosition> dropCells)
        {
            this.grid = grid;
            this.boxes = boxes;
            this.dropCells = dropCells;
        }

        public bool CanEmptyEveryBox()
        {
            WarehouseGrid openedGrid = grid.Clone();
            var truckDistances = new DistanceField(openedGrid, dropCells);
            var isOpened = new bool[boxes.Count];
            int openedBoxes = 0;
            int openedThisPass;
            do
            {
                openedThisPass = OpenReachableBoxes(openedGrid, truckDistances, isOpened);
                openedBoxes += openedThisPass;
                if (openedThisPass > 0)
                    truckDistances.Recompute();
            }
            while (openedThisPass > 0);

            return openedBoxes == boxes.Count;
        }

        private int OpenReachableBoxes(WarehouseGrid openedGrid, DistanceField truckDistances, bool[] isOpened)
        {
            int opened = 0;
            for (int index = 0; index < boxes.Count; index++)
            {
                if (isOpened[index] || !HasAccessCell(openedGrid, truckDistances, boxes[index]))
                    continue;

                Open(openedGrid, boxes[index]);
                isOpened[index] = true;
                opened++;
            }

            return opened;
        }

        private static bool HasAccessCell(WarehouseGrid openedGrid, DistanceField truckDistances, Box box)
        {
            for (int y = box.Origin.Y; y < box.Origin.Y + box.Size.Height; y++)
                for (int x = box.Origin.X; x < box.Origin.X + box.Size.Width; x++)
                    if (HasAccessCellAround(openedGrid, truckDistances, new GridPosition(x, y)))
                        return true;
            return false;
        }

        private static bool HasAccessCellAround(WarehouseGrid openedGrid, DistanceField truckDistances, GridPosition boxCell)
        {
            for (int direction = 0; direction < GridDirections.StraightCount; direction++)
            {
                GridPosition neighbour = GridDirections.Step(boxCell, direction);
                if (openedGrid.Contains(neighbour)
                    && openedGrid.GetCellType(neighbour) == CellType.Floor
                    && truckDistances.IsReachable(neighbour))
                    return true;
            }

            return false;
        }

        private static void Open(WarehouseGrid openedGrid, Box box)
        {
            for (int y = box.Origin.Y; y < box.Origin.Y + box.Size.Height; y++)
                for (int x = box.Origin.X; x < box.Origin.X + box.Size.Width; x++)
                    openedGrid.SetCellType(new GridPosition(x, y), CellType.Floor);
        }
    }
}
