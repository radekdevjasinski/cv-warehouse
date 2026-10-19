using CvWarehouse.Core.Effects;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Effects
{
    public sealed class EffectCurvesTests
    {
        private const float Overshoot = 1.7f;
        private const float PeakScale = 1.25f;
        private const float Tolerance = 0.0001f;
        private const int Steps = 50;

        [Test]
        public void PopIn_StartAndEnd_GrowsFromNothingToFullSize()
        {
            Assert.AreEqual(0f, EffectCurves.PopIn(0f, Overshoot), Tolerance);
            Assert.AreEqual(1f, EffectCurves.PopIn(1f, Overshoot), Tolerance);
        }

        [Test]
        public void PopIn_OnTheWay_OvershootsFullSizeBeforeSettling()
        {
            float largest = 0f;
            for (int step = 0; step <= Steps; step++)
                largest = System.Math.Max(largest, EffectCurves.PopIn(step / (float)Steps, Overshoot));

            Assert.Greater(largest, 1f);
            Assert.Less(largest, 1.25f);
        }

        [Test]
        public void PopIn_NoOvershoot_NeverGrowsPastFullSize()
        {
            for (int step = 0; step <= Steps; step++)
                Assert.LessOrEqual(EffectCurves.PopIn(step / (float)Steps, 0f), 1f + Tolerance);
        }

        [Test]
        public void Pulse_OverItsLifetime_SwellsToThePeakAndBack()
        {
            Assert.AreEqual(1f, EffectCurves.Pulse(0f, PeakScale), Tolerance);
            Assert.AreEqual(PeakScale, EffectCurves.Pulse(0.5f, PeakScale), Tolerance);
            Assert.AreEqual(1f, EffectCurves.Pulse(1f, PeakScale), Tolerance);
        }

        [Test]
        public void Flash_OverItsLifetime_FadesFromFullToNothing()
        {
            Assert.AreEqual(1f, EffectCurves.Flash(0f), Tolerance);
            Assert.AreEqual(0.25f, EffectCurves.Flash(0.5f), Tolerance);
            Assert.AreEqual(0f, EffectCurves.Flash(1f), Tolerance);
        }

        [Test]
        public void Curves_ProgressOutOfRange_AreClampedToTheEnds()
        {
            Assert.AreEqual(1f, EffectCurves.PopIn(2f, Overshoot), Tolerance);
            Assert.AreEqual(1f, EffectCurves.Pulse(-1f, PeakScale), Tolerance);
            Assert.AreEqual(0f, EffectCurves.Flash(3f), Tolerance);
        }
    }
}
