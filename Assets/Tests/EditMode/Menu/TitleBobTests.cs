using CvWarehouse.Core.Menu;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Menu
{
    public sealed class TitleBobTests
    {
        private const float Period = 2f;
        private const float Amplitude = 8f;
        private const float Step = 4f;
        private const float Tolerance = 0.0001f;

        [TestCase(0f, 0f)]
        [TestCase(0.5f, 8f)]
        [TestCase(1f, 0f)]
        [TestCase(1.5f, -8f)]
        [TestCase(2f, 0f)]
        public void OffsetAt_PointsOfTheWave_FollowsTheSine(float seconds, float expected)
        {
            Assert.AreEqual(expected, TitleBob.OffsetAt(seconds, Period, Amplitude, Step), Tolerance);
        }

        [Test]
        public void OffsetAt_AnyTime_LandsOnWholeSteps()
        {
            for (float seconds = 0f; seconds < Period; seconds += 0.07f)
            {
                float offset = TitleBob.OffsetAt(seconds, Period, Amplitude, Step);
                Assert.AreEqual(0f, offset % Step, Tolerance, "at " + seconds);
                Assert.LessOrEqual(System.Math.Abs(offset), Amplitude);
            }
        }

        [Test]
        public void OffsetAt_NoStep_GivesTheSmoothWave()
        {
            Assert.AreEqual(Amplitude * 0.70710678f, TitleBob.OffsetAt(0.25f, Period, Amplitude, 0f), 0.001f);
        }

        [Test]
        public void OffsetAt_NoPeriod_StaysStill()
        {
            Assert.AreEqual(0f, TitleBob.OffsetAt(1.3f, 0f, Amplitude, Step), Tolerance);
        }
    }
}
