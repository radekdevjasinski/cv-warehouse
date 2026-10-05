using System;
using CvWarehouse.Core.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Cv
{
    public sealed class CvDeliveryRevealTests
    {
        private const int Seed = 12345;
        private const int NameLetterCount = 11;
        private const int TotalLetterCount = 24;
        private const int TotalPieces = 48;

        private CvRevealState state;
        private CvDeliveryReveal reveal;

        [SetUp]
        public void SetUp()
        {
            state = NewState();
            reveal = NewReveal(state);
        }

        [Test]
        public void ShowProgress_NothingDelivered_RevealsNothing()
        {
            reveal.ShowProgress(0, TotalPieces);

            Assert.AreEqual(0, state.RevealedLetterCount);
        }

        [Test]
        public void ShowProgress_HalfDelivered_RevealsHalfOfTheLetters()
        {
            reveal.ShowProgress(TotalPieces / 2, TotalPieces);

            Assert.AreEqual(TotalLetterCount / 2, state.RevealedLetterCount);
        }

        [Test]
        public void ShowProgress_EverythingDelivered_CompletesTheCv()
        {
            reveal.ShowProgress(TotalPieces, TotalPieces);

            Assert.IsTrue(state.IsComplete);
        }

        [Test]
        public void ShowProgress_WarehouseWithoutPieces_CompletesTheCv()
        {
            reveal.ShowProgress(0, 0);

            Assert.IsTrue(state.IsComplete);
        }

        [Test]
        public void ShowProgress_SameProgressTwice_RevealsNothingMore()
        {
            reveal.ShowProgress(TotalPieces / 2, TotalPieces);
            reveal.ShowProgress(TotalPieces / 2, TotalPieces);

            Assert.AreEqual(TotalLetterCount / 2, state.RevealedLetterCount);
        }

        [Test]
        public void ShowProgress_EnoughForTheName_FillsTheNameBeforeTheProfile()
        {
            reveal.ShowProgress(NameLetterCount, TotalLetterCount);

            state.TryGetBlock(CvItemId.Name, out CvBlockReveal name);
            state.TryGetBlock(CvTestBlocks.ProfileId, out CvBlockReveal profile);
            Assert.IsTrue(name.IsComplete);
            Assert.AreEqual(0, profile.RevealedCount);
        }

        [Test]
        public void ShowProgress_SameSeed_RevealsTheSameLetters()
        {
            CvRevealState otherState = NewState();

            reveal.ShowProgress(5, TotalLetterCount);
            NewReveal(otherState).ShowProgress(5, TotalLetterCount);

            state.TryGetBlock(CvItemId.Name, out CvBlockReveal name);
            otherState.TryGetBlock(CvItemId.Name, out CvBlockReveal otherName);
            for (int letter = 0; letter < NameLetterCount; letter++)
                Assert.AreEqual(name.IsRevealed(letter), otherName.IsRevealed(letter));
        }

        private static CvRevealState NewState()
        {
            return new CvRevealState(CvTestBlocks.Build(CvTestBlocks.NameAndProfileCv));
        }

        private static CvDeliveryReveal NewReveal(CvRevealState revealState)
        {
            return new CvDeliveryReveal(revealState, new CvPiecePicker(new Random(Seed)));
        }
    }
}
