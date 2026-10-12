using CvWarehouse.Presentation.Warehouse;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class BotAnimationTests
    {
        private const int TicksPerStepFrame = 8;
        private const float SecondsBetweenBlinks = 3.2f;
        private const float MidBlink = 0.05f;
        private const float AfterBlink = 0.5f;

        [Test]
        public void IsStepping_BotStandingStill_NeverSteps()
        {
            var animation = new BotAnimation();

            for (int tick = 0; tick < TicksPerStepFrame * 4; tick++)
                Assert.IsFalse(animation.IsStepping(false, tick));
        }

        [Test]
        public void IsStepping_BotMoving_SwapsFrameEveryFewTicks()
        {
            var animation = new BotAnimation();

            Assert.IsFalse(animation.IsStepping(true, 0));
            Assert.IsFalse(animation.IsStepping(true, TicksPerStepFrame - 1));
            Assert.IsTrue(animation.IsStepping(true, TicksPerStepFrame));
            Assert.IsTrue(animation.IsStepping(true, TicksPerStepFrame * 2 - 1));
            Assert.IsFalse(animation.IsStepping(true, TicksPerStepFrame * 2));
        }

        [Test]
        public void IsBlinking_FirstCrewSlot_BlinksBrieflyAtTheStartOfEveryPeriod()
        {
            var animation = new BotAnimation();

            Assert.IsTrue(animation.IsBlinking(0, MidBlink));
            Assert.IsFalse(animation.IsBlinking(0, AfterBlink));
            Assert.IsTrue(animation.IsBlinking(0, SecondsBetweenBlinks + MidBlink));
        }

        [Test]
        public void IsBlinking_AnotherCrewSlot_BlinksAtADifferentMoment()
        {
            var animation = new BotAnimation();

            Assert.IsFalse(animation.IsBlinking(1, MidBlink));
        }
    }
}
