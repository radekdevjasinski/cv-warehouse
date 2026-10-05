using CvWarehouse.Core.Warehouse;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class WarehouseGridTests
    {
        private const int Up = 0;
        private const int Right = 1;
        private const int UpRight = 4;

        private static readonly GridPosition Start = new GridPosition(1, 1);

        private WarehouseGrid grid;

        [SetUp]
        public void SetUp()
        {
            grid = new WarehouseGrid(3, 3);
        }

        [Test]
        public void CanStep_OpenFloor_AllowsAllEightDirections()
        {
            for (int direction = 0; direction < GridDirections.Count; direction++)
                Assert.IsTrue(grid.CanStep(Start, direction));
        }

        [Test]
        public void CanStep_OutsideGrid_ReturnsFalse()
        {
            Assert.IsFalse(grid.CanStep(new GridPosition(2, 2), Up));
        }

        [Test]
        public void CanStep_BlockedTarget_ReturnsFalse()
        {
            grid.SetCellType(new GridPosition(2, 1), CellType.Blocked);

            Assert.IsFalse(grid.CanStep(Start, Right));
        }

        [Test]
        public void CanStep_DiagonalPastBlockedCorner_ReturnsFalse()
        {
            grid.SetCellType(new GridPosition(2, 1), CellType.Blocked);

            Assert.IsFalse(grid.CanStep(Start, UpRight));
        }
    }
}
