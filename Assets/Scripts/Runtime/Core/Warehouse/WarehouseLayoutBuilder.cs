using System;
using System.Collections.Generic;
using CvWarehouse.Core.Cv;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class WarehouseLayoutBuilder
    {
        private readonly WarehouseGrid grid;
        private readonly List<Box> boxes = new List<Box>();
        private readonly List<GridPosition> dropCells = new List<GridPosition>();
        private readonly List<GridPosition> parkingCells = new List<GridPosition>();
        private GridPosition truckOrigin;
        private GridSize truckSize;

        public WarehouseLayoutBuilder(int width, int height)
        {
            grid = new WarehouseGrid(width, height);
        }

        public bool IsFree(GridPosition origin, GridSize size)
        {
            for (int y = origin.Y; y < origin.Y + size.Height; y++)
                for (int x = origin.X; x < origin.X + size.Width; x++)
                    if (!IsFreeFloor(new GridPosition(x, y)))
                        return false;
            return true;
        }

        public WarehouseLayoutBuilder Block(GridPosition origin, GridSize size)
        {
            for (int y = origin.Y; y < origin.Y + size.Height; y++)
                for (int x = origin.X; x < origin.X + size.Width; x++)
                    Mark(new GridPosition(x, y), CellType.Blocked);
            return this;
        }

        public WarehouseLayoutBuilder PlaceTruck(GridPosition origin, GridSize size)
        {
            truckOrigin = origin;
            truckSize = size;
            return Block(origin, size);
        }

        public WarehouseLayoutBuilder AddBox(GridPosition origin, WeightClass weightClass, int pieceCount)
        {
            if (pieceCount <= 0)
                throw new ArgumentException("A box needs at least one piece.");

            var box = new Box(boxes.Count, origin, weightClass, pieceCount);
            Block(origin, box.Size);
            boxes.Add(box);
            return this;
        }

        public WarehouseLayoutBuilder AddDropCell(GridPosition position)
        {
            Mark(position, CellType.Drop);
            dropCells.Add(position);
            return this;
        }

        public WarehouseLayoutBuilder AddParkingCell(GridPosition position)
        {
            Mark(position, CellType.Parking);
            parkingCells.Add(position);
            return this;
        }

        public WarehouseLayout Build()
        {
            int totalPieces = 0;
            foreach (Box box in boxes)
                totalPieces += box.TotalPieces;

            var layout = new WarehouseLayout
            {
                Grid = grid,
                Boxes = boxes,
                DropCells = dropCells,
                ParkingCells = parkingCells,
                TruckDistances = new DistanceField(grid, dropCells),
                TruckOrigin = truckOrigin,
                TruckSize = truckSize,
                TotalPieces = totalPieces
            };
            layout.RefreshAccess();
            return layout;
        }

        private bool IsFreeFloor(GridPosition position)
        {
            return grid.Contains(position) && grid.GetCellType(position) == CellType.Floor;
        }

        private void Mark(GridPosition position, CellType cellType)
        {
            if (!IsFreeFloor(position))
                throw new ArgumentException("Cell " + position + " is outside the grid or already taken.");

            grid.SetCellType(position, cellType);
        }
    }
}
