using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Presentation.Warehouse;
using CvWarehouse.Tests.EditMode.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class BoxSelectionTests
    {
        private const int Letters = 4;

        private Box firstBox;
        private Box secondBox;
        private BoxSelection selection;

        [SetUp]
        public void SetUp()
        {
            firstBox = new Box(0, new GridPosition(0, 0), WeightClass.Light, CvTestBlocks.WithLetters(Letters));
            secondBox = new Box(1, new GridPosition(1, 0), WeightClass.Light, CvTestBlocks.WithLetters(Letters));
            selection = new BoxSelection();
        }

        [Test]
        public void HasBox_NothingSelected_IsFalseAndThereIsNoSelectedBox()
        {
            Assert.IsFalse(selection.HasBox);
            Assert.IsNull(selection.SelectedBox);
        }

        [Test]
        public void Select_ABox_MakesItTheSelectedBox()
        {
            selection.Select(firstBox);

            Assert.IsTrue(selection.HasBox);
            Assert.AreSame(firstBox, selection.SelectedBox);
        }

        [Test]
        public void Select_AnotherBox_ReplacesTheSelection()
        {
            selection.Select(firstBox);

            selection.Select(secondBox);

            Assert.AreSame(secondBox, selection.SelectedBox);
        }

        [Test]
        public void Clear_BoxSelected_SelectsNothing()
        {
            selection.Select(firstBox);

            selection.Clear();

            Assert.IsFalse(selection.HasBox);
        }

        [Test]
        public void HasBox_SelectedBoxWasEmptied_IsFalse()
        {
            selection.Select(firstBox);

            firstBox.TakeClaimed(firstBox.TotalPieces);

            Assert.IsFalse(selection.HasBox);
            Assert.IsNull(selection.SelectedBox);
        }
    }
}
