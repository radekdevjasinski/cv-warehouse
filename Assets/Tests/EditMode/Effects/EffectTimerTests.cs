using CvWarehouse.Core.Effects;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Effects
{
    public sealed class EffectTimerTests
    {
        private const float Duration = 0.4f;
        private const float Tolerance = 0.0001f;

        private EffectTimer timer;

        [SetUp]
        public void SetUp()
        {
            timer = new EffectTimer { DurationSeconds = Duration };
        }

        [Test]
        public void New_NeverStarted_IsNotRunning()
        {
            Assert.IsFalse(timer.IsRunning);
        }

        [Test]
        public void Advance_HalfTheDuration_IsHalfwayAndStillRunning()
        {
            timer.Start();

            timer.Advance(Duration / 2f);

            Assert.IsTrue(timer.IsRunning);
            Assert.AreEqual(0.5f, timer.Progress, Tolerance);
        }

        [Test]
        public void Advance_PastTheDuration_StopsAtTheEnd()
        {
            timer.Start();

            timer.Advance(Duration * 2f);

            Assert.IsFalse(timer.IsRunning);
            Assert.AreEqual(1f, timer.Progress, Tolerance);
        }

        [Test]
        public void Start_WhileRunning_StartsOverFromTheBeginning()
        {
            timer.Start();
            timer.Advance(Duration * 0.75f);

            timer.Start();

            Assert.IsTrue(timer.IsRunning);
            Assert.AreEqual(0f, timer.Progress, Tolerance);
        }
    }
}
