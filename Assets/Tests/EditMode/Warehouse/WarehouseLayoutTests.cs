using System;
using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Tests.EditMode.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class WarehouseLayoutTests
    {
        private const int GridSide = 6;
        private const int Pieces = 3;

        private static readonly GridPosition DropCell = new GridPosition(0, 5);
        private static readonly GridPosition BoxOrigin = new GridPosition(2, 2);

        [TestCase(BoxSizeClass.Small, 4)]
        [TestCase(BoxSizeClass.Medium, 6)]
        [TestCase(BoxSizeClass.Large, 8)]
        public void Build_BoxOnOpenFloor_HasAccessCellsOnAllFourSides(BoxSizeClass sizeClass, int expectedAccessCells)
        {
            WarehouseLayout layout = NewBuilder().AddBox(BoxOrigin, TestBoxKinds.Of(sizeClass), CvTestBlocks.WithLetters(Pieces)).Build();

            Assert.AreEqual(expectedAccessCells, layout.Boxes[0].AccessCells.Count);
        }

        [Test]
        public void Build_BoxInCorner_HasOnlyAccessCellsInsideTheGrid()
        {
            WarehouseLayout layout = NewBuilder().AddBox(new GridPosition(0, 0), TestBoxKinds.Small, CvTestBlocks.WithLetters(Pieces)).Build();

            Assert.AreEqual(2, layout.Boxes[0].AccessCells.Count);
        }

        [Test]
        public void Build_BoxWalledOffFromTheTruck_HasNoAccessCells()
        {
            WarehouseLayout layout = NewBuilder()
                .Block(new GridPosition(0, 3), new GridSize(GridSide, 1))
                .AddBox(new GridPosition(2, 1), TestBoxKinds.Small, CvTestBlocks.WithLetters(Pieces))
                .Build();

            Assert.IsEmpty(layout.Boxes[0].AccessCells);
        }

        [Test]
        public void RemoveEmptyBox_BoxBuriedInTheCorner_GetsAnAccessCellOnTheFreedFloor()
        {
            var freedCell = new GridPosition(1, 0);
            WarehouseLayout layout = NewBuilder()
                .AddBox(new GridPosition(0, 0), TestBoxKinds.Small, CvTestBlocks.WithLetters(Pieces))
                .AddBox(freedCell, TestBoxKinds.Small, CvTestBlocks.WithLetters(Pieces))
                .AddBox(new GridPosition(0, 1), TestBoxKinds.Small, CvTestBlocks.WithLetters(Pieces))
                .Build();
            Box buried = layout.Boxes[0];
            Box neighbour = layout.Boxes[1];
            Assert.IsEmpty(buried.AccessCells);

            neighbour.TakeClaimed(neighbour.Claim(int.MaxValue, new Random(0), new List<int>()));
            layout.RemoveEmptyBox(neighbour);

            CollectionAssert.AreEqual(new[] { freedCell }, buried.AccessCells);
            Assert.IsEmpty(neighbour.AccessCells);
            Assert.IsTrue(layout.Grid.IsWalkable(freedCell));
        }

        [Test]
        public void RemoveEmptyBox_BoxStillHoldingPieces_Throws()
        {
            WarehouseLayout layout = NewBuilder().AddBox(BoxOrigin, TestBoxKinds.Small, CvTestBlocks.WithLetters(Pieces)).Build();

            Assert.Throws<InvalidOperationException>(() => layout.RemoveEmptyBox(layout.Boxes[0]));
        }

        [Test]
        public void Build_Boxes_BlockTheirFootprintAndSumTheirPieces()
        {
            WarehouseLayout layout = NewBuilder()
                .AddBox(BoxOrigin, TestBoxKinds.Large, CvTestBlocks.WithLetters(Pieces))
                .AddBox(new GridPosition(5, 0), TestBoxKinds.Small, CvTestBlocks.WithLetters(Pieces))
                .Build();

            Assert.IsFalse(layout.Grid.IsWalkable(new GridPosition(3, 3)));
            Assert.AreEqual(Pieces * 2, layout.TotalPieces);
        }

        [Test]
        public void AddBox_OverlappingAnotherBox_Throws()
        {
            WarehouseLayoutBuilder builder = NewBuilder().AddBox(BoxOrigin, TestBoxKinds.Large, CvTestBlocks.WithLetters(Pieces));

            Assert.Throws<ArgumentException>(() => builder.AddBox(new GridPosition(3, 3), TestBoxKinds.Small, CvTestBlocks.WithLetters(Pieces)));
        }

        [Test]
        public void AddBox_OutsideTheGrid_Throws()
        {
            Assert.Throws<ArgumentException>(() => NewBuilder().AddBox(new GridPosition(5, 5), TestBoxKinds.Large, CvTestBlocks.WithLetters(Pieces)));
        }

        [Test]
        public void TruckDistances_OpenFloor_UseStraightAndDiagonalCosts()
        {
            WarehouseLayout layout = NewBuilder().Build();

            Assert.AreEqual(0, layout.TruckDistances.GetDistance(DropCell));
            Assert.AreEqual(2 * GridDirections.StraightCost, layout.TruckDistances.GetDistance(new GridPosition(2, 5)));
            Assert.AreEqual(2 * GridDirections.DiagonalCost, layout.TruckDistances.GetDistance(new GridPosition(2, 3)));
        }

        private static WarehouseLayoutBuilder NewBuilder()
        {
            return new WarehouseLayoutBuilder(GridSide, GridSide).AddDropCell(DropCell);
        }
    }
}
