using CvWarehouse.Presentation.Warehouse;
using NUnit.Framework;
using UnityEngine;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class CameraFramingTests
    {
        private const int SpritePixelsPerUnit = 16;
        private const float ClosestViewCells = 5f;
        private const float FarthestViewCells = 20f;
        private const float Tolerance = 0.001f;

        private static readonly Vector2 MapSize = new Vector2(40f, 30f);
        private static readonly Vector2Int ViewPixels = new Vector2Int(640, 320);

        private CameraFraming framing;

        [SetUp]
        public void SetUp()
        {
            framing = CreateFraming(MapSize, ViewPixels);
        }

        [Test]
        public void ZoomOutFully_View_ScalesEverySpritePixelToWholeScreenPixels()
        {
            framing.ZoomIn();
            framing.ZoomOutFully();

            Assert.AreEqual(1, framing.Scale);
            Assert.AreEqual(ViewPixels.y * 0.5f / SpritePixelsPerUnit, framing.Size, Tolerance);
        }

        [Test]
        public void ZoomIn_PastTheClosestLimit_StopsAtTheLastScaleShowingTheClosestViewCells()
        {
            for (int step = 0; step < 10; step++)
                framing.ZoomIn();

            Assert.AreEqual(4, framing.Scale);
            Assert.AreEqual(ClosestViewCells, framing.Size * 2f, Tolerance);
        }

        [Test]
        public void ZoomOut_PastTheFarthestLimit_StopsAtTheFarthestScale()
        {
            framing.ZoomIn();

            framing.ZoomOut();
            framing.ZoomOut();

            Assert.AreEqual(1, framing.Scale);
        }

        [Test]
        public void ZoomOutFully_BigMap_ShowsNoMoreThanTheFarthestViewCells()
        {
            CameraFraming bigMap = CreateFraming(new Vector2(200f, 200f), new Vector2Int(1280, 700));

            bigMap.ZoomOutFully();

            Assert.AreEqual(3, bigMap.Scale);
            Assert.LessOrEqual(bigMap.Size * 2f, FarthestViewCells);
        }

        [Test]
        public void ZoomOutFully_MapNarrowerThanTheView_KeepsTheWholeViewInsideTheMap()
        {
            CameraFraming smallMap = CreateFraming(new Vector2(30f, 30f), new Vector2Int(1280, 640));

            smallMap.ZoomOutFully();

            Assert.AreEqual(3, smallMap.Scale);
            Assert.LessOrEqual(1280 * smallMap.UnitsPerScreenPixel, 30f);
        }

        [Test]
        public void SetViewPixels_TwiceAsManyPixels_ShowsTheSameCellsAtTwiceTheScale()
        {
            framing.SetViewPixels(ViewPixels * 2);
            framing.ZoomOutFully();

            Assert.AreEqual(2, framing.Scale);
            Assert.AreEqual(ViewPixels.y * 0.5f / SpritePixelsPerUnit, framing.Size, Tolerance);
        }

        [Test]
        public void Pan_FarPastTheCorner_StopsWithTheViewOnTheMapEdges()
        {
            framing.ZoomIn();
            framing.ZoomIn();

            framing.Pan(new Vector2(-1000f, 1000f));

            Assert.AreEqual(0f, framing.Centre.x - ViewPixels.x * 0.5f * framing.UnitsPerScreenPixel, Tolerance);
            Assert.AreEqual(MapSize.y, framing.Centre.y + framing.Size, Tolerance);
        }

        [Test]
        public void Pan_StepsSmallerThanAScreenPixel_StillAddUp()
        {
            framing.ZoomIn();
            framing.ZoomIn();

            for (int step = 0; step < 100; step++)
                framing.Pan(new Vector2(0.005f, 0f));

            Assert.AreEqual(MapSize.x * 0.5f + 0.5f, framing.Centre.x, Tolerance);
        }

        [TestCase(640, 320)]
        [TestCase(641, 321)]
        public void Centre_AfterAnyPan_PutsTheViewEdgesOnWholeScreenPixels(int viewWidth, int viewHeight)
        {
            CameraFraming odd = CreateFraming(MapSize, new Vector2Int(viewWidth, viewHeight));
            odd.ZoomIn();
            odd.ZoomIn();

            odd.Pan(new Vector2(0.013f, -0.021f));

            AssertOnWholeScreenPixel(odd.Centre.x / odd.UnitsPerScreenPixel - viewWidth * 0.5f);
            AssertOnWholeScreenPixel(odd.Centre.y / odd.UnitsPerScreenPixel - viewHeight * 0.5f);
        }

        private static CameraFraming CreateFraming(Vector2 mapSize, Vector2Int viewPixels)
        {
            var limits = new CameraZoomLimits(SpritePixelsPerUnit, ClosestViewCells, FarthestViewCells);
            var created = new CameraFraming(mapSize, limits);
            created.SetViewPixels(viewPixels);
            created.ZoomOutFully();
            return created;
        }

        private static void AssertOnWholeScreenPixel(float pixel)
        {
            Assert.AreEqual(Mathf.Round(pixel), pixel, Tolerance);
        }
    }
}
