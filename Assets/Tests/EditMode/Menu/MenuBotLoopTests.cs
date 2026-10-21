using System;
using CvWarehouse.Core.Menu;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Menu
{
    public sealed class MenuBotLoopTests
    {
        private const float TakeOutSeconds = 1f;
        private const float WalkSeconds = 2f;
        private const float PutInSeconds = 0.5f;
        private const float WholeTripSeconds = TakeOutSeconds + WalkSeconds + PutInSeconds + WalkSeconds;
        private const float Tolerance = 0.0001f;

        [Test]
        public void New_StartsTakingOutAtTheSourceBoxWithEmptyHands()
        {
            MenuBotLoop loop = NewLoop();

            Assert.AreEqual(MenuBotPhase.TakingOut, loop.Phase);
            Assert.AreEqual(0f, loop.Position, Tolerance);
            Assert.IsFalse(loop.IsMoving);
            Assert.AreEqual(0, loop.CargoWords);
        }

        [TestCase(0.5f, MenuBotPhase.TakingOut)]
        [TestCase(1.5f, MenuBotPhase.Carrying)]
        [TestCase(3.25f, MenuBotPhase.PuttingIn)]
        [TestCase(4f, MenuBotPhase.Returning)]
        [TestCase(5.75f, MenuBotPhase.TakingOut)]
        public void Advance_ElapsedTime_PicksThePhaseInOrder(float seconds, MenuBotPhase expectedPhase)
        {
            MenuBotLoop loop = NewLoop();

            loop.Advance(seconds);

            Assert.AreEqual(expectedPhase, loop.Phase);
        }

        [Test]
        public void Advance_HalfwayThroughCarrying_StandsHalfwayAndMovesWithCargo()
        {
            MenuBotLoop loop = NewLoop();

            loop.Advance(TakeOutSeconds + WalkSeconds * 0.5f);

            Assert.AreEqual(0.5f, loop.Position, Tolerance);
            Assert.IsTrue(loop.IsMoving);
            Assert.IsTrue(loop.IsCarrying);
            Assert.AreEqual(1, loop.CargoWords);
        }

        [Test]
        public void Advance_PuttingIn_StandsAtTheTargetBoxStillHoldingTheCargo()
        {
            MenuBotLoop loop = NewLoop();

            loop.Advance(TakeOutSeconds + WalkSeconds + PutInSeconds * 0.5f);

            Assert.AreEqual(1f, loop.Position, Tolerance);
            Assert.IsFalse(loop.IsMoving);
            Assert.AreEqual(1, loop.CargoWords);
        }

        [Test]
        public void Advance_QuarterOfTheWayBack_WalksBackWithEmptyHands()
        {
            MenuBotLoop loop = NewLoop();

            loop.Advance(TakeOutSeconds + WalkSeconds + PutInSeconds + WalkSeconds * 0.25f);

            Assert.AreEqual(0.75f, loop.Position, Tolerance);
            Assert.IsTrue(loop.IsMoving);
            Assert.AreEqual(0, loop.CargoWords);
        }

        [Test]
        public void Advance_ManyTrips_CyclesThroughTheCargoSizes()
        {
            MenuBotLoop loop = NewLoop();
            var carried = new int[4];

            for (int trip = 0; trip < carried.Length; trip++)
            {
                loop.Advance(trip == 0 ? TakeOutSeconds + 0.1f : WholeTripSeconds);
                carried[trip] = loop.CargoWords;
            }

            CollectionAssert.AreEqual(new[] { 1, 3, 1, 3 }, carried);
            Assert.AreEqual(3, loop.DeliveredTrips);
        }

        [Test]
        public void Advance_SmallSteps_EndsWhereOneBigStepEnds()
        {
            MenuBotLoop stepped = NewLoop();
            MenuBotLoop jumped = NewLoop();

            for (int frame = 0; frame < 600; frame++)
                stepped.Advance(1f / 60f);
            jumped.Advance(10f);

            Assert.AreEqual(jumped.Phase, stepped.Phase);
            Assert.AreEqual(jumped.DeliveredTrips, stepped.DeliveredTrips);
            Assert.AreEqual(jumped.Position, stepped.Position, 0.01f);
        }

        [Test]
        public void Advance_NegativeTime_ChangesNothing()
        {
            MenuBotLoop loop = NewLoop();

            loop.Advance(-5f);

            Assert.AreEqual(MenuBotPhase.TakingOut, loop.Phase);
            Assert.AreEqual(0f, loop.PhaseProgress, Tolerance);
        }

        [Test]
        public void New_PhaseWithoutDuration_Throws()
        {
            var settings = new MenuBotLoopSettings { WalkSeconds = 0f };

            Assert.Throws<ArgumentException>(() => new MenuBotLoop(settings));
        }

        [Test]
        public void New_NoCargoSizes_Throws()
        {
            var settings = new MenuBotLoopSettings { CargoWordsPerTrip = new int[0] };

            Assert.Throws<ArgumentException>(() => new MenuBotLoop(settings));
        }

        private static MenuBotLoop NewLoop()
        {
            return new MenuBotLoop(new MenuBotLoopSettings
            {
                TakeOutSeconds = TakeOutSeconds,
                WalkSeconds = WalkSeconds,
                PutInSeconds = PutInSeconds,
                CargoWordsPerTrip = new[] { 1, 3 }
            });
        }
    }
}
