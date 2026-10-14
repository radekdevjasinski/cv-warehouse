using CvWarehouse.Core.Warehouse;
using CvWarehouse.Tests.EditMode.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class BoxReachabilityTests
    {
        private const int GridSide = 6;
        private const int Pieces = 3;

        private static readonly GridPosition DropCell = new GridPosition(0, 5);

        [Test]
        public void CanEmptyEveryBox_BoxOnOpenFloor_ReturnsTrue()
        {
            WarehouseLayoutBuilder builder = NewBuilder()
                .AddBox(new GridPosition(2, 2), TestBoxKinds.Small, CvTestBlocks.WithLetters(Pieces));

            Assert.IsTrue(builder.CanEmptyEveryBox());
        }

        [Test]
        public void CanEmptyEveryBox_BoxBuriedBehindOtherBoxes_ReturnsTrue()
        {
            WarehouseLayoutBuilder builder = NewBuilder()
                .AddBox(new GridPosition(0, 0), TestBoxKinds.Small, CvTestBlocks.WithLetters(Pieces))
                .AddBox(new GridPosition(1, 0), TestBoxKinds.Small, CvTestBlocks.WithLetters(Pieces))
                .AddBox(new GridPosition(0, 1), TestBoxKinds.Small, CvTestBlocks.WithLetters(Pieces));

            Assert.IsTrue(builder.CanEmptyEveryBox());
        }

        [Test]
        public void CanEmptyEveryBox_BoxWalledOffFromTheTruck_ReturnsFalse()
        {
            WarehouseLayoutBuilder builder = NewBuilder()
                .Block(new GridPosition(0, 3), new GridSize(GridSide, 1))
                .AddBox(new GridPosition(2, 1), TestBoxKinds.Small, CvTestBlocks.WithLetters(Pieces));

            Assert.IsFalse(builder.CanEmptyEveryBox());
        }

        [Test]
        public void CanEmptyEveryBox_Checked_LeavesTheBuilderGridUntouched()
        {
            var boxOrigin = new GridPosition(2, 2);
            WarehouseLayoutBuilder builder = NewBuilder()
                .AddBox(boxOrigin, TestBoxKinds.Small, CvTestBlocks.WithLetters(Pieces));

            builder.CanEmptyEveryBox();

            Assert.IsFalse(builder.Build().Grid.IsWalkable(boxOrigin));
        }

        private static WarehouseLayoutBuilder NewBuilder()
        {
            return new WarehouseLayoutBuilder(GridSide, GridSide).AddDropCell(DropCell);
        }
    }
}
