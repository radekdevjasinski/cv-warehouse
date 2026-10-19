using CvWarehouse.Core.Warehouse;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class TruckPlanTests
    {
        private const int DefaultWidth = 10;
        private const float FrontWheelCentre = 12f / 16f;
        private const float DriveWheelCentre = 36f / 16f;
        private const float RearWheelCentre = DefaultWidth - 22f / 16f;
        private const float RearWheelSpacing = 12f / 16f;
        private const float CabEnd = 26f / 16f;
        private const float WheelRadius = 5f / 16f;
        private const float HalfLogoWidth = 13f / 16f;
        private const float BoxStart = 24f / 16f;
        private const float Tolerance = 0.0001f;

        [Test]
        public void Parts_DefaultTruck_CabThenALongTrailerEndingInTheRear()
        {
            var plan = new TruckPlan(DefaultWidth);

            Assert.AreEqual(DefaultWidth, plan.Parts.Count);
            Assert.AreEqual(TruckPart.CabFront, plan.Parts[0]);
            Assert.AreEqual(TruckPart.CabBack, plan.Parts[1]);
            for (int column = 2; column < DefaultWidth - 1; column++)
                Assert.AreEqual(TruckPart.TrailerMiddle, plan.Parts[column]);
            Assert.AreEqual(TruckPart.TrailerRear, plan.Parts[DefaultWidth - 1]);
        }

        [Test]
        public void Parts_NarrowestTruck_IsACabAndARear()
        {
            var plan = new TruckPlan(TruckPlan.MinWidth);

            CollectionAssert.AreEqual(new[] { TruckPart.CabFront, TruckPart.CabBack, TruckPart.TrailerRear }, plan.Parts);
            Assert.AreEqual(2, plan.WheelCentres.Count);
        }

        [TestCase(2)]
        [TestCase(1)]
        public void Parts_TooNarrowForACab_FillsTheSpaceWithTrailerWithoutWheelsOrLogo(int width)
        {
            var plan = new TruckPlan(width);

            Assert.AreEqual(width, plan.Parts.Count);
            foreach (TruckPart part in plan.Parts)
                Assert.AreEqual(TruckPart.TrailerMiddle, part);
            Assert.AreEqual(0, plan.WheelCentres.Count);
            Assert.IsFalse(plan.HasLogo);
        }

        [TestCase(4, 2)]
        [TestCase(5, 3)]
        [TestCase(7, 4)]
        [TestCase(10, 5)]
        [TestCase(13, 6)]
        [TestCase(16, 7)]
        public void WheelCentres_LongerTruck_AddsMoreRearWheels(int width, int expectedWheels)
        {
            var plan = new TruckPlan(width);

            Assert.AreEqual(expectedWheels, plan.WheelCentres.Count);
        }

        [Test]
        public void WheelCentres_DefaultTruck_CabWheelDriveWheelAndThreeRearWheelsGroupedAtTheBack()
        {
            var plan = new TruckPlan(DefaultWidth);

            Assert.AreEqual(5, plan.WheelCentres.Count);
            Assert.AreEqual(FrontWheelCentre, plan.WheelCentres[0], Tolerance);
            Assert.AreEqual(DriveWheelCentre, plan.WheelCentres[1], Tolerance);
            Assert.AreEqual(RearWheelCentre - RearWheelSpacing * 2f, plan.WheelCentres[2], Tolerance);
            Assert.AreEqual(RearWheelCentre - RearWheelSpacing, plan.WheelCentres[3], Tolerance);
            Assert.AreEqual(RearWheelCentre, plan.WheelCentres[4], Tolerance);
        }

        [TestCase(4)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(30)]
        public void WheelCentres_AnyWidth_RearWheelsStayUnderTheBoxBehindTheCabWithoutOverlapping(int width)
        {
            var plan = new TruckPlan(width);

            for (int wheel = 1; wheel < plan.WheelCentres.Count; wheel++)
            {
                Assert.Greater(plan.WheelCentres[wheel] - WheelRadius, CabEnd);
                Assert.Less(plan.WheelCentres[wheel] + WheelRadius, width);
                Assert.GreaterOrEqual(plan.WheelCentres[wheel] - plan.WheelCentres[wheel - 1], WheelRadius * 2f);
            }
        }

        [TestCase(4)]
        [TestCase(10)]
        [TestCase(30)]
        public void LogoCentre_AnyWidth_SitsOnAWholePixelInsideTheBox(int width)
        {
            var plan = new TruckPlan(width);

            Assert.IsTrue(plan.HasLogo);
            Assert.Greater(plan.LogoCentre - HalfLogoWidth, BoxStart);
            Assert.Less(plan.LogoCentre + HalfLogoWidth, width);
            float centrePixels = plan.LogoCentre * 16f;
            Assert.AreEqual(System.Math.Round(centrePixels), centrePixels, Tolerance);
        }

        [Test]
        public void HasLogo_NarrowestTruck_LeavesTheBoxTooShortForTheLogoBlank()
        {
            var plan = new TruckPlan(TruckPlan.MinWidth);

            Assert.IsFalse(plan.HasLogo);
        }

        [Test]
        public void LogoCentre_DefaultTruck_IsInTheMiddleOfTheBox()
        {
            var plan = new TruckPlan(DefaultWidth);

            Assert.AreEqual(91f / 16f, plan.LogoCentre, Tolerance);
        }
    }
}
