using CvWarehouse.Presentation.Warehouse;
using NUnit.Framework;
using UnityEngine;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class EdgePanTests
    {
        private const float EdgeWidth = 30f;

        private static readonly Rect View = new Rect(0f, 0f, 1000f, 600f);

        [Test]
        public void DirectionAt_MiddleOfTheView_DoesNotPan()
        {
            Assert.AreEqual(Vector2.zero, EdgePan.DirectionAt(new Vector2(500f, 300f), View, EdgeWidth));
        }

        [Test]
        public void DirectionAt_LeftEdge_PansLeft()
        {
            Assert.AreEqual(Vector2.left, EdgePan.DirectionAt(new Vector2(10f, 300f), View, EdgeWidth));
        }

        [Test]
        public void DirectionAt_RightEdge_PansRight()
        {
            Assert.AreEqual(Vector2.right, EdgePan.DirectionAt(new Vector2(990f, 300f), View, EdgeWidth));
        }

        [Test]
        public void DirectionAt_TopRightCorner_PansDiagonally()
        {
            Assert.AreEqual(new Vector2(1f, 1f), EdgePan.DirectionAt(new Vector2(990f, 590f), View, EdgeWidth));
        }

        [Test]
        public void DirectionAt_BottomEdge_PansDown()
        {
            Assert.AreEqual(Vector2.down, EdgePan.DirectionAt(new Vector2(500f, 5f), View, EdgeWidth));
        }

        [Test]
        public void DirectionAt_PointerOutsideTheView_DoesNotPan()
        {
            Assert.AreEqual(Vector2.zero, EdgePan.DirectionAt(new Vector2(1010f, 300f), View, EdgeWidth));
        }

        [Test]
        public void DirectionAt_ViewNotStartingAtTheScreenCorner_MeasuresFromTheViewEdge()
        {
            var offsetView = new Rect(200f, 100f, 1000f, 600f);

            Assert.AreEqual(Vector2.left, EdgePan.DirectionAt(new Vector2(210f, 400f), offsetView, EdgeWidth));
            Assert.AreEqual(Vector2.zero, EdgePan.DirectionAt(new Vector2(500f, 400f), offsetView, EdgeWidth));
        }
    }
}
