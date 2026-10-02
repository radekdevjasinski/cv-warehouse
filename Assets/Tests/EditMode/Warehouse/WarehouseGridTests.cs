using CvWarehouse.Core.Warehouse;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class WarehouseGridTests
    {
        private const int Up = 0;
        private const int Right = 1;
        private const int UpRight = 4;
        private const int FirstBot = 0;
        private const int SecondBot = 1;

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

        [Test]
        public void IsStepFreeFor_TargetHeldByAnotherBot_ReturnsFalse()
        {
            grid.Occupy(new GridPosition(2, 1), SecondBot);

            Assert.IsFalse(grid.IsStepFreeFor(Start, Right, FirstBot));
            Assert.IsTrue(grid.IsStepFreeFor(Start, Right, SecondBot));
        }

        [Test]
        public void IsStepFreeFor_BotBesideDiagonal_ReturnsFalse()
        {
            grid.Occupy(new GridPosition(1, 2), SecondBot);

            Assert.IsFalse(grid.IsStepFreeFor(Start, UpRight, FirstBot));
        }

        [Test]
        public void Vacate_ByAnotherBot_KeepsTheCellOccupied()
        {
            grid.Occupy(Start, FirstBot);

            grid.Vacate(Start, SecondBot);

            Assert.AreEqual(FirstBot, grid.GetBotAt(Start));
        }
    }
}
