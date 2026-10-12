using CvWarehouse.Core.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Cv
{
    public sealed class CvFollowTimerTests
    {
        private const float IdleSeconds = 5f;
        private const float SecondsBetweenScrolls = 2f;

        private CvFollowTimer timer;

        [SetUp]
        public void SetUp()
        {
            timer = new CvFollowTimer(IdleSeconds, SecondsBetweenScrolls);
        }

        [Test]
        public void NewTimer_NobodyTouchedTheCvYet_FollowsAndMayScrollStraightAway()
        {
            Assert.IsTrue(timer.IsFollowing);
            Assert.IsTrue(timer.TryScroll());
        }

        [Test]
        public void NoteActivity_StopsFollowingUntilTheIdleTimeHasPassed()
        {
            timer.NoteActivity();
            Assert.IsFalse(timer.IsFollowing);
            Assert.IsFalse(timer.TryScroll());

            timer.Advance(IdleSeconds - 0.1f);
            Assert.IsFalse(timer.IsFollowing);

            timer.Advance(0.1f);
            Assert.IsTrue(timer.IsFollowing);
            Assert.IsTrue(timer.TryScroll());
        }

        [Test]
        public void NoteActivity_WhileIdleTimeIsRunning_StartsTheWaitAgain()
        {
            timer.NoteActivity();
            timer.Advance(IdleSeconds - 1f);

            timer.NoteActivity();
            timer.Advance(IdleSeconds - 1f);

            Assert.IsFalse(timer.IsFollowing);
        }

        [Test]
        public void TryScroll_RightAfterAScroll_HoldsThePageForAWhile()
        {
            Assert.IsTrue(timer.TryScroll());
            Assert.IsFalse(timer.TryScroll());

            timer.Advance(SecondsBetweenScrolls - 0.1f);
            Assert.IsFalse(timer.TryScroll());

            timer.Advance(0.1f);
            Assert.IsTrue(timer.TryScroll());
        }
    }
}
