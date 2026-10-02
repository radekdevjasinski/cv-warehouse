using System;

namespace CvWarehouse.Core.Warehouse
{
    public static class GridDirections
    {
        public const int Count = 8;
        public const int StraightCount = 4;
        public const int StraightCost = 10;
        public const int DiagonalCost = 14;

        private static readonly int[] DeltaX = { 0, 1, 0, -1, 1, 1, -1, -1 };
        private static readonly int[] DeltaY = { 1, 0, -1, 0, 1, -1, -1, 1 };

        public static bool IsDiagonal(int direction)
        {
            return direction >= StraightCount;
        }

        public static int CostOf(int direction)
        {
            return IsDiagonal(direction) ? DiagonalCost : StraightCost;
        }

        public static int DeltaXOf(int direction)
        {
            return DeltaX[direction];
        }

        public static int DeltaYOf(int direction)
        {
            return DeltaY[direction];
        }

        public static GridPosition Step(GridPosition from, int direction)
        {
            return from.Offset(DeltaX[direction], DeltaY[direction]);
        }

        public static int Between(GridPosition from, GridPosition to)
        {
            for (int direction = 0; direction < Count; direction++)
                if (Step(from, direction) == to)
                    return direction;

            throw new ArgumentException("Cells " + from + " and " + to + " are not neighbours.");
        }

        public static int OctileDistance(GridPosition from, GridPosition to)
        {
            int distanceX = Math.Abs(from.X - to.X);
            int distanceY = Math.Abs(from.Y - to.Y);
            int diagonalSteps = Math.Min(distanceX, distanceY);
            int straightSteps = Math.Max(distanceX, distanceY) - diagonalSteps;
            return diagonalSteps * DiagonalCost + straightSteps * StraightCost;
        }
    }
}
