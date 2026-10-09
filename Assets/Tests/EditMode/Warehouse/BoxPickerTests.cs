using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Tests.EditMode.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class BoxPickerTests
    {
        private const int Letters = 10;

        private Box heavyBox;
        private Box lightBox;
        private List<Box> boxes;

        [SetUp]
        public void SetUp()
        {
            heavyBox = new Box(0, new GridPosition(3, 2), WeightClass.Heavy, CvTestBlocks.WithLetters(Letters));
            lightBox = new Box(1, new GridPosition(5, 2), WeightClass.Light, CvTestBlocks.WithLetters(Letters));
            boxes = new List<Box> { heavyBox, lightBox };
        }

        [Test]
        public void TryPick_PointInsideABox_PicksThatBox()
        {
            Assert.IsTrue(BoxPicker.TryPick(boxes, 5.5f, 2.5f, out Box picked));
            Assert.AreSame(lightBox, picked);
        }

        [Test]
        public void TryPick_PointInAnyCellOfABigBox_PicksIt()
        {
            Assert.IsTrue(BoxPicker.TryPick(boxes, 3.1f, 2.1f, out Box bottomLeft));
            Assert.IsTrue(BoxPicker.TryPick(boxes, 4.9f, 3.9f, out Box topRight));
            Assert.AreSame(heavyBox, bottomLeft);
            Assert.AreSame(heavyBox, topRight);
        }

        [Test]
        public void TryPick_PointOnTheSharedEdgeOfTwoBoxes_PicksTheBoxThatStartsThere()
        {
            Assert.IsTrue(BoxPicker.TryPick(boxes, 5f, 2.5f, out Box picked));
            Assert.AreSame(lightBox, picked);
        }

        [Test]
        public void TryPick_PointOnTheFloor_PicksNothing()
        {
            Assert.IsFalse(BoxPicker.TryPick(boxes, 5.5f, 3.5f, out Box picked));
            Assert.IsNull(picked);
        }

        [Test]
        public void TryPick_PointJustOutsideABox_PicksNothing()
        {
            Assert.IsFalse(BoxPicker.TryPick(boxes, 2.99f, 2.5f, out _));
            Assert.IsFalse(BoxPicker.TryPick(boxes, 6f, 2.5f, out _));
        }

        [Test]
        public void TryPick_PointInsideAnEmptiedBox_PicksNothing()
        {
            lightBox.TakeClaimed(lightBox.TotalPieces);

            Assert.IsFalse(BoxPicker.TryPick(boxes, 5.5f, 2.5f, out _));
        }
    }
}
