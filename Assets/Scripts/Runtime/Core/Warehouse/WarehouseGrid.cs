using System;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class WarehouseGrid
    {
        private readonly CellType[] cellTypes;

        public WarehouseGrid(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("The grid needs a positive width and height.");

            Width = width;
            Height = height;
            cellTypes = new CellType[width * height];
        }

        public int Width { get; }

        public int Height { get; }

        public int CellCount => cellTypes.Length;

        public bool Contains(GridPosition position)
        {
            return position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height;
        }

        public int IndexOf(GridPosition position)
        {
            return position.Y * Width + position.X;
        }

        public GridPosition PositionOf(int cellIndex)
        {
            return new GridPosition(cellIndex % Width, cellIndex / Width);
        }

        public CellType GetCellType(GridPosition position)
        {
            return cellTypes[IndexOf(position)];
        }

        public void SetCellType(GridPosition position, CellType cellType)
        {
            cellTypes[IndexOf(position)] = cellType;
        }

        public bool IsWalkable(GridPosition position)
        {
            return Contains(position) && cellTypes[IndexOf(position)] != CellType.Blocked;
        }

        public bool CanStep(GridPosition from, int direction)
        {
            if (!IsWalkable(GridDirections.Step(from, direction)))
                return false;
            if (!GridDirections.IsDiagonal(direction))
                return true;

            return IsWalkable(from.Offset(GridDirections.DeltaXOf(direction), 0))
                && IsWalkable(from.Offset(0, GridDirections.DeltaYOf(direction)));
        }
    }
}
