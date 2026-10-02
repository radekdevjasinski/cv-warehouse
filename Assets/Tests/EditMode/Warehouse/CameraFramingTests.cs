using CvWarehouse.Presentation.Warehouse;
using NUnit.Framework;
using UnityEngine;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class CameraFramingTests
    {
        private const float ClosestSize = 5f;
        private const float Margin = 2f;
        private const float Tolerance = 0.001f;

        private static readonly Vector2 MapSize = new Vector2(80f, 56f);

        private CameraFraming framing;

        [SetUp]
        public void SetUp()
        {
            framing = new CameraFraming(MapSize, ClosestSize, Margin);
            framing.SetAspect(2f);
            framing.LookAt(MapSize * 0.5f, 10f);
        }

        [Test]
        public void Pan_InsideTheMap_MovesTheCentre()
        {
            framing.Pan(new Vector2(3f, -2f));

            Assert.AreEqual(43f, framing.Centre.x, Tolerance);
            Assert.AreEqual(26f, framing.Centre.y, Tolerance);
        }

        [Test]
        public void Pan_FarPastTheEdge_StopsAtTheMapEdgePlusMargin()
        {
            framing.Pan(new Vector2(-1000f, 1000f));

            Assert.AreEqual(20f - Margin, framing.Centre.x, Tolerance);
            Assert.AreEqual(MapSize.y - 10f + Margin, framing.Centre.y, Tolerance);
        }

        [Test]
        public void Zoom_InPastTheLimit_StopsAtTheClosestSize()
        {
            framing.Zoom(0.01f);

            Assert.AreEqual(ClosestSize, framing.Size, Tolerance);
        }

        [Test]
        public void Zoom_OutPastTheLimit_ShowsTheWholeMapCentred()
        {
            framing.Pan(new Vector2(20f, 10f));

            framing.Zoom(100f);

            Assert.AreEqual(MapSize.y * 0.5f + Margin, framing.Size, Tolerance);
            Assert.AreEqual(MapSize.x * 0.5f, framing.Centre.x, Tolerance);
            Assert.AreEqual(MapSize.y * 0.5f, framing.Centre.y, Tolerance);
        }

        [Test]
        public void SetAspect_NarrowScreen_AllowsZoomingOutToTheFullWidth()
        {
            framing.SetAspect(0.5f);

            framing.Zoom(100f);

            Assert.AreEqual(MapSize.x + Margin, framing.Size, Tolerance);
        }
    }
}
