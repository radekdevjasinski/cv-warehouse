using System;
using System.Collections.Generic;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class WarehouseLayout
    {
        public WarehouseGrid Grid { get; internal set; }

        public IReadOnlyList<Box> Boxes { get; internal set; }

        public IReadOnlyList<GridPosition> DropCells { get; internal set; }

        public IReadOnlyList<GridPosition> ParkingCells { get; internal set; }

        public DistanceField TruckDistances { get; internal set; }

        public GridPosition TruckOrigin { get; internal set; }

        public GridSize TruckSize { get; internal set; }

        public int TotalPieces { get; internal set; }

        public int Version { get; private set; }

        public void RemoveEmptyBox(Box box)
        {
            if (!box.IsEmpty)
                throw new InvalidOperationException("Box " + box.Id + " still holds pieces.");

            for (int y = box.Origin.Y; y < box.Origin.Y + box.Size.Height; y++)
                for (int x = box.Origin.X; x < box.Origin.X + box.Size.Width; x++)
                    Grid.SetCellType(new GridPosition(x, y), CellType.Floor);

            RefreshAccess();
        }

        internal void RefreshAccess()
        {
            TruckDistances.Recompute();
            foreach (Box box in Boxes)
            {
                box.ClearAccessCells();
                if (!box.IsEmpty)
                    FindAccessCells(box);
            }

            Version++;
        }

        private void FindAccessCells(Box box)
        {
            for (int y = box.Origin.Y; y < box.Origin.Y + box.Size.Height; y++)
                for (int x = box.Origin.X; x < box.Origin.X + box.Size.Width; x++)
                    AddAccessCellsAround(box, new GridPosition(x, y));
        }

        private void AddAccessCellsAround(Box box, GridPosition boxCell)
        {
            for (int direction = 0; direction < GridDirections.StraightCount; direction++)
            {
                GridPosition neighbour = GridDirections.Step(boxCell, direction);
                if (!Grid.Contains(neighbour) || Grid.GetCellType(neighbour) != CellType.Floor)
                    continue;
                if (TruckDistances.IsReachable(neighbour))
                    box.AddAccessCell(neighbour, TruckDistances.GetDistance(neighbour));
            }
        }
    }
}
