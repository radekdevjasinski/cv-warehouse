using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Cv
{
    public sealed class CvRevealStateTests
    {
        private const int NameLetterCount = 11;
        private const int ProfileLetterCount = 13;

        private CvRevealState state;
        private List<string> changedBlocks;

        [SetUp]
        public void SetUp()
        {
            state = new CvRevealState(CvTestBlocks.Build(CvTestBlocks.NameAndProfileCv));
            changedBlocks = new List<string>();
            state.BlockChanged += changedBlocks.Add;
        }

        [Test]
        public void NewState_HasEverythingHidden()
        {
            Assert.AreEqual(NameLetterCount + ProfileLetterCount, state.TotalLetterCount);
            Assert.AreEqual(0, state.RevealedLetterCount);
            Assert.IsFalse(state.IsComplete);
            Assert.IsFalse(GetBlock(CvItemId.Name).IsRevealed(0));
        }

        [Test]
        public void RevealLetter_HiddenLetter_RevealsOnlyThatLetter()
        {
            int revealed = state.RevealLetter(CvItemId.Name, 4);

            CvBlockReveal block = GetBlock(CvItemId.Name);
            Assert.AreEqual(1, revealed);
            Assert.IsTrue(block.IsRevealed(4));
            Assert.IsFalse(block.IsRevealed(3));
            Assert.AreEqual(1, state.RevealedLetterCount);
            CollectionAssert.AreEqual(new[] { CvItemId.Name }, changedBlocks);
        }

        [Test]
        public void RevealLetter_SameLetterTwice_ChangesNothingTheSecondTime()
        {
            state.RevealLetter(CvItemId.Name, 4);

            int revealedAgain = state.RevealLetter(CvItemId.Name, 4);

            Assert.AreEqual(0, revealedAgain);
            Assert.AreEqual(1, changedBlocks.Count);
        }

        [TestCase("missing", 0)]
        [TestCase(CvItemId.Name, -1)]
        [TestCase(CvItemId.Name, NameLetterCount)]
        public void RevealLetter_UnknownBlockOrLetter_IsIgnored(string id, int letter)
        {
            Assert.AreEqual(0, state.RevealLetter(id, letter));
            Assert.AreEqual(0, state.RevealedLetterCount);
            Assert.IsEmpty(changedBlocks);
        }

        [Test]
        public void RevealWord_PartlyRevealedWord_RevealsOnlyItsRemainingLetters()
        {
            state.RevealLetter(CvTestBlocks.ProfileId, 6);

            int revealed = state.RevealWord(CvTestBlocks.ProfileId, 1);

            CvBlockReveal block = GetBlock(CvTestBlocks.ProfileId);
            Assert.AreEqual(2, revealed);
            Assert.IsTrue(block.IsWordComplete(1));
            Assert.IsFalse(block.IsWordComplete(0));
            Assert.AreEqual(3, block.RevealedCount);
        }

        [TestCase(-1)]
        [TestCase(3)]
        public void RevealWord_UnknownWord_IsIgnored(int wordIndex)
        {
            Assert.AreEqual(0, state.RevealWord(CvTestBlocks.ProfileId, wordIndex));
        }

        [Test]
        public void RevealBlock_CompletesTheBlockAndLeavesOthersHidden()
        {
            state.RevealBlock(CvTestBlocks.ProfileId);

            Assert.IsTrue(GetBlock(CvTestBlocks.ProfileId).IsComplete);
            Assert.AreEqual(0, GetBlock(CvItemId.Name).RevealedCount);
            Assert.AreEqual(ProfileLetterCount, state.RevealedLetterCount);
        }

        [Test]
        public void TryGetFirstIncompleteBlock_SkipsCompleteBlocks()
        {
            state.RevealBlock(CvItemId.Name);

            Assert.IsTrue(state.TryGetFirstIncompleteBlock(out CvBlockReveal block));
            Assert.AreEqual(CvTestBlocks.ProfileId, block.Block.Id);
        }

        [Test]
        public void RevealAll_CompletesEverythingAndNotifiesOnlyChangedBlocks()
        {
            state.RevealBlock(CvItemId.Name);
            changedBlocks.Clear();

            state.RevealAll();

            Assert.IsTrue(state.IsComplete);
            Assert.IsFalse(state.TryGetFirstIncompleteBlock(out _));
            CollectionAssert.AreEqual(new[] { CvTestBlocks.ProfileId }, changedBlocks);
        }

        [Test]
        public void Reset_HidesEverythingAndNotifiesOnlyChangedBlocks()
        {
            state.RevealLetter(CvItemId.Name, 2);
            changedBlocks.Clear();

            state.Reset();

            Assert.AreEqual(0, state.RevealedLetterCount);
            Assert.IsFalse(GetBlock(CvItemId.Name).IsRevealed(2));
            CollectionAssert.AreEqual(new[] { CvItemId.Name }, changedBlocks);
        }

        private CvBlockReveal GetBlock(string id)
        {
            Assert.IsTrue(state.TryGetBlock(id, out CvBlockReveal block));
            return block;
        }
    }
}
