using CvWarehouse.Presentation.Warehouse;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class DepthSortingTests
    {
        private const float OnePixel = 1f / DepthSorting.PixelsPerUnit;
        private const int TopLayer = DepthSorting.LayersPerPixel - 1;

        [Test]
        public void OrderAt_LowerOnTheScreen_IsDrawnInFront()
        {
            Assert.Greater(DepthSorting.OrderAt(3f, 0), DepthSorting.OrderAt(4f, 0));
        }

        [Test]
        public void OrderAt_HigherLayerAtTheSameHeight_IsDrawnInFront()
        {
            Assert.Greater(DepthSorting.OrderAt(3f, 1), DepthSorting.OrderAt(3f, 0));
        }

        [Test]
        public void OrderAt_TopLayerOnePixelHigher_StaysBehindTheBottomLayerBelowIt()
        {
            Assert.Less(DepthSorting.OrderAt(3f + OnePixel, TopLayer), DepthSorting.OrderAt(3f, 0));
        }

        [Test]
        public void OrderAt_TallWarehouse_StaysAboveTheFloorAndInsideTheSortingRange()
        {
            int order = DepthSorting.OrderAt(200f, 0);

            Assert.Greater(order, 1);
            Assert.LessOrEqual(DepthSorting.OrderAt(0f, TopLayer), short.MaxValue);
        }
    }
}
