using CvWarehouse.Core.Effects;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Effects
{
    public sealed class PointsPopupQueueTests
    {
        private const float Duration = 1f;
        private const float Rise = 2f;
        private const float FadeStart = 0.5f;
        private const int Capacity = 3;
        private const float Tolerance = 0.0001f;
        private const float X = 4.5f;
        private const float Y = 7.5f;
        private const int Points = 6;
        private const int Steps = 20;
        private const float OverlapWidth = 1.5f;
        private const float LineHeight = 0.6f;

        private PointsPopupQueue queue;

        [SetUp]
        public void SetUp()
        {
            var settings = new PointsPopupSettings
            {
                DurationSeconds = Duration,
                RiseDistance = Rise,
                FadeStartFraction = FadeStart,
                OverlapWidth = OverlapWidth,
                LineHeight = LineHeight
            };
            queue = new PointsPopupQueue(settings, Capacity);
        }

        [Test]
        public void Add_NewPopup_StartsAtItsSpotFullyVisible()
        {
            queue.Add(X, Y, Points);

            Assert.AreEqual(1, queue.Count);
            Assert.AreEqual(X, queue[0].X);
            Assert.AreEqual(Y, queue[0].Y);
            Assert.AreEqual(Points, queue[0].Points);
            Assert.AreEqual(0f, queue.RiseOf(0), Tolerance);
            Assert.AreEqual(1f, queue.AlphaOf(0), Tolerance);
        }

        [TestCase(0)]
        [TestCase(-3)]
        public void Add_NoPoints_ShowsNothing(int points)
        {
            queue.Add(X, Y, points);

            Assert.AreEqual(0, queue.Count);
        }

        [Test]
        public void Advance_OverItsLifetime_RisesAllTheWayWithoutEverDropping()
        {
            queue.Add(X, Y, Points);
            float lastRise = 0f;

            for (int step = 0; step < Steps - 1; step++)
            {
                queue.Advance(Duration / Steps);
                Assert.GreaterOrEqual(queue.RiseOf(0), lastRise);
                lastRise = queue.RiseOf(0);
            }

            Assert.Less(lastRise, Rise);
            Assert.Greater(lastRise, Rise * 0.9f);
        }

        [Test]
        public void Advance_BeforeTheFadeStarts_StaysFullyVisible()
        {
            queue.Add(X, Y, Points);

            queue.Advance(Duration * FadeStart);

            Assert.AreEqual(1f, queue.AlphaOf(0), Tolerance);
        }

        [Test]
        public void Advance_AfterTheFadeStarts_FadesOutTowardsTheEnd()
        {
            queue.Add(X, Y, Points);

            queue.Advance(Duration * 0.75f);

            Assert.AreEqual(0.5f, queue.AlphaOf(0), Tolerance);
        }

        [Test]
        public void Advance_PastItsLifetime_RemovesThePopup()
        {
            queue.Add(X, Y, Points);

            queue.Advance(Duration);

            Assert.AreEqual(0, queue.Count);
        }

        [Test]
        public void Advance_PopupsAddedAtDifferentTimes_RemovesOnlyTheExpiredOne()
        {
            queue.Add(X, Y, 1);
            queue.Advance(Duration * 0.6f);
            queue.Add(X, Y, 2);

            queue.Advance(Duration * 0.6f);

            Assert.AreEqual(1, queue.Count);
            Assert.AreEqual(2, queue[0].Points);
        }

        [Test]
        public void Add_NextToAFreshPopup_StacksAboveItInsteadOfOverlapping()
        {
            queue.Add(X, Y, 1);

            queue.Add(X + 1f, Y, 2);

            Assert.AreEqual(Y + LineHeight, queue[1].Y, Tolerance);
        }

        [Test]
        public void Add_ThreeBotsCloseTogether_StacksEachPopupOneLineHigher()
        {
            queue.Add(X, Y, 1);
            queue.Add(X + 1f, Y, 2);

            queue.Add(X + 0.5f, Y, 3);

            Assert.AreEqual(Y + LineHeight * 2f, queue[2].Y, Tolerance);
        }

        [Test]
        public void Add_FarFromOtherPopups_KeepsItsOwnHeight()
        {
            queue.Add(X, Y, 1);

            queue.Add(X + OverlapWidth, Y, 2);

            Assert.AreEqual(Y, queue[1].Y, Tolerance);
        }

        [Test]
        public void Add_AfterTheNeighbourHasRisenAway_KeepsItsOwnHeight()
        {
            queue.Add(X, Y, 1);
            queue.Advance(Duration * 0.5f);

            queue.Add(X, Y, 2);

            Assert.AreEqual(Y, queue[1].Y, Tolerance);
        }

        [Test]
        public void Add_MoreThanTheCapacity_DropsTheOldestPopup()
        {
            for (int points = 1; points <= Capacity + 1; points++)
                queue.Add(X, Y, points);

            Assert.AreEqual(Capacity, queue.Count);
            Assert.AreEqual(2, queue[0].Points);
            Assert.AreEqual(Capacity + 1, queue[Capacity - 1].Points);
        }
    }
}
