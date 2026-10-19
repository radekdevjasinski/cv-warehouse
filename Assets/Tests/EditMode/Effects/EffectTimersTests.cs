using CvWarehouse.Core.Effects;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Effects
{
    public sealed class EffectTimersTests
    {
        private const float Duration = 1f;
        private const float Tolerance = 0.0001f;
        private const int FirstId = 3;
        private const int SecondId = 7;
        private const int ThirdId = 9;

        private EffectTimers timers;

        [SetUp]
        public void SetUp()
        {
            timers = new EffectTimers(4) { DurationSeconds = Duration };
        }

        [Test]
        public void TryGetProgress_NeverStarted_ReportsNothing()
        {
            Assert.IsFalse(timers.TryGetProgress(FirstId, out _));
        }

        [Test]
        public void Advance_TwoEffectsStartedApart_KeepsTheirOwnProgress()
        {
            timers.Start(FirstId);
            timers.Advance(Duration * 0.5f);
            timers.Start(SecondId);
            timers.Advance(Duration * 0.25f);

            Assert.IsTrue(timers.TryGetProgress(FirstId, out float firstProgress));
            Assert.IsTrue(timers.TryGetProgress(SecondId, out float secondProgress));
            Assert.AreEqual(0.75f, firstProgress, Tolerance);
            Assert.AreEqual(0.25f, secondProgress, Tolerance);
        }

        [Test]
        public void Advance_PastTheDuration_ForgetsTheFinishedEffectOnly()
        {
            timers.Start(FirstId);
            timers.Start(SecondId);
            timers.Advance(Duration * 0.5f);
            timers.Start(ThirdId);

            timers.Advance(Duration * 0.6f);

            Assert.AreEqual(1, timers.Count);
            Assert.IsFalse(timers.TryGetProgress(FirstId, out _));
            Assert.IsFalse(timers.TryGetProgress(SecondId, out _));
            Assert.IsTrue(timers.TryGetProgress(ThirdId, out float thirdProgress));
            Assert.AreEqual(0.6f, thirdProgress, Tolerance);
        }

        [Test]
        public void Start_SameIdAgain_RestartsInsteadOfAddingASecondEffect()
        {
            timers.Start(FirstId);
            timers.Advance(Duration * 0.5f);

            timers.Start(FirstId);

            Assert.AreEqual(1, timers.Count);
            Assert.IsTrue(timers.TryGetProgress(FirstId, out float progress));
            Assert.AreEqual(0f, progress, Tolerance);
        }
    }
}
