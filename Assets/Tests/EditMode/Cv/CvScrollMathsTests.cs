using CvWarehouse.Core.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Cv
{
    public sealed class CvScrollMathsTests
    {
        private const float ViewportHeight = 400f;
        private const float ContentHeight = 2000f;
        private const float Tolerance = 0.001f;

        [Test]
        public void FurthestOffset_ContentTallerThanTheView_IsTheHiddenPart()
        {
            Assert.AreEqual(1600f, CvScrollMaths.FurthestOffset(ViewportHeight, ContentHeight), Tolerance);
        }

        [Test]
        public void FurthestOffset_ContentShorterThanTheView_IsZero()
        {
            Assert.AreEqual(0f, CvScrollMaths.FurthestOffset(ViewportHeight, 300f), Tolerance);
        }

        [Test]
        public void OffsetCentring_TargetInTheMiddleOfThePage_PutsItInTheMiddleOfTheView()
        {
            Assert.AreEqual(800f, CvScrollMaths.OffsetCentring(1000f, ViewportHeight, ContentHeight), Tolerance);
        }

        [Test]
        public void OffsetCentring_TargetNearTheTop_StopsAtTheTop()
        {
            Assert.AreEqual(0f, CvScrollMaths.OffsetCentring(50f, ViewportHeight, ContentHeight), Tolerance);
        }

        [Test]
        public void OffsetCentring_TargetNearTheBottom_StopsAtTheBottom()
        {
            Assert.AreEqual(1600f, CvScrollMaths.OffsetCentring(1950f, ViewportHeight, ContentHeight), Tolerance);
        }

        [TestCase(500f, true)]
        [TestCase(900f, true)]
        [TestCase(499f, false)]
        [TestCase(901f, false)]
        public void IsInView_ViewScrolledToFiveHundred_SeesOnlyWhatLiesInside(float targetFromTop, bool isExpectedInView)
        {
            Assert.AreEqual(isExpectedInView, CvScrollMaths.IsInView(targetFromTop, 500f, ViewportHeight));
        }
    }
}
