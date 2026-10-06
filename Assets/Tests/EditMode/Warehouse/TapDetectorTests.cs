using CvWarehouse.Presentation.Warehouse;
using NUnit.Framework;
using UnityEngine;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class TapDetectorTests
    {
        private const float MaxTravel = 10f;

        private static readonly Vector2 PressPoint = new Vector2(200f, 100f);

        private TapDetector detector;

        [SetUp]
        public void SetUp()
        {
            detector = new TapDetector();
            detector.Press(PressPoint);
        }

        [Test]
        public void IsTap_PressedAndReleasedInPlace_IsATap()
        {
            Assert.IsTrue(detector.IsTap(MaxTravel));
        }

        [Test]
        public void IsTap_PointerMovedALittle_IsStillATap()
        {
            detector.Move(PressPoint + new Vector2(3f, 4f));

            Assert.IsTrue(detector.IsTap(MaxTravel));
        }

        [Test]
        public void IsTap_PointerDraggedAway_IsNotATap()
        {
            detector.Move(PressPoint + new Vector2(30f, 0f));

            Assert.IsFalse(detector.IsTap(MaxTravel));
        }

        [Test]
        public void IsTap_PointerDraggedAwayAndBack_IsNotATap()
        {
            detector.Move(PressPoint + new Vector2(30f, 0f));
            detector.Move(PressPoint);

            Assert.IsFalse(detector.IsTap(MaxTravel));
        }

        [Test]
        public void Press_AfterADrag_StartsANewTap()
        {
            detector.Move(PressPoint + new Vector2(30f, 0f));

            detector.Press(PressPoint);

            Assert.IsTrue(detector.IsTap(MaxTravel));
        }
    }
}
