using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Cv
{
    public sealed class CvFreshLettersTests
    {
        private const float FreshSeconds = 2f;
        private const float Tolerance = 0.0001f;

        private CvRevealState state;
        private List<string> changedBlocks;
        private List<string> deliveredBlocks;

        [SetUp]
        public void SetUp()
        {
            state = new CvRevealState(CvTestBlocks.Build(CvTestBlocks.NameAndProfileCv));
            changedBlocks = new List<string>();
            deliveredBlocks = new List<string>();
            state.BlockChanged += changedBlocks.Add;
            state.Delivered += deliveredBlocks.Add;
        }

        [Test]
        public void DeliverLetters_HiddenLetters_RevealsThemAndMarksOnlyThemFresh()
        {
            int revealed = state.DeliverLetters(CvItemId.Name, new[] { 0, 1, 2 }, FreshSeconds);

            CvBlockReveal block = Block(CvItemId.Name);
            Assert.AreEqual(3, revealed);
            Assert.IsTrue(block.IsRevealed(1));
            Assert.IsTrue(block.HasFreshLetters);
            Assert.IsTrue(block.IsFresh(0));
            Assert.IsTrue(block.IsFresh(2));
            Assert.IsFalse(block.IsFresh(3));
            CollectionAssert.AreEqual(new[] { CvItemId.Name }, deliveredBlocks);
        }

        [Test]
        public void DeliverLetters_SecondDeliveryToTheSameBlock_ReplacesItsFreshLetters()
        {
            state.DeliverLetters(CvItemId.Name, new[] { 0, 1 }, FreshSeconds);

            state.DeliverLetters(CvItemId.Name, new[] { 5 }, FreshSeconds);

            CvBlockReveal block = Block(CvItemId.Name);
            Assert.IsFalse(block.IsFresh(0));
            Assert.IsTrue(block.IsFresh(5));
            Assert.IsTrue(block.IsRevealed(0));
        }

        [Test]
        public void DeliverLetters_UnknownBlock_IsIgnored()
        {
            Assert.AreEqual(0, state.DeliverLetters("no.such.block", new[] { 0 }, FreshSeconds));
            CollectionAssert.IsEmpty(deliveredBlocks);
        }

        [Test]
        public void RevealLetters_WithoutADelivery_MarksNothingFresh()
        {
            state.RevealLetters(CvItemId.Name, new[] { 0, 1 });

            Assert.IsFalse(Block(CvItemId.Name).HasFreshLetters);
            Assert.IsFalse(Block(CvItemId.Name).IsFresh(0));
        }

        [Test]
        public void Advance_BeforeTheFreshTimeIsUp_KeepsTheLettersFresh()
        {
            state.DeliverLetters(CvItemId.Name, new[] { 0 }, FreshSeconds);
            changedBlocks.Clear();

            state.Advance(FreshSeconds * 0.5f);

            Assert.IsTrue(Block(CvItemId.Name).IsFresh(0));
            CollectionAssert.IsEmpty(changedBlocks);
        }

        [Test]
        public void FreshFraction_AsTimePasses_FallsFromOneToZero()
        {
            CvBlockReveal block = Block(CvItemId.Name);
            Assert.AreEqual(0f, block.FreshFraction, Tolerance);

            state.DeliverLetters(CvItemId.Name, new[] { 0 }, FreshSeconds);
            Assert.AreEqual(1f, block.FreshFraction, Tolerance);

            state.Advance(FreshSeconds * 0.25f);
            Assert.AreEqual(0.75f, block.FreshFraction, Tolerance);
            Assert.AreEqual(FreshSeconds * 0.75f, block.FreshSecondsLeft, Tolerance);

            state.Advance(FreshSeconds);
            Assert.AreEqual(0f, block.FreshFraction, Tolerance);
        }

        [Test]
        public void Advance_PastTheFreshTime_ClearsTheFreshLettersAndReportsTheBlockOnce()
        {
            state.DeliverLetters(CvItemId.Name, new[] { 0 }, FreshSeconds);
            changedBlocks.Clear();

            state.Advance(FreshSeconds);
            state.Advance(FreshSeconds);

            Assert.IsFalse(Block(CvItemId.Name).HasFreshLetters);
            Assert.IsFalse(Block(CvItemId.Name).IsFresh(0));
            Assert.IsTrue(Block(CvItemId.Name).IsRevealed(0));
            CollectionAssert.AreEqual(new[] { CvItemId.Name }, changedBlocks);
        }

        [Test]
        public void Advance_TwoBlocksDeliveredAtDifferentTimes_ExpiresEachOnItsOwnClock()
        {
            state.DeliverLetters(CvItemId.Name, new[] { 0 }, FreshSeconds);
            state.Advance(FreshSeconds * 0.75f);
            state.DeliverLetters(CvTestBlocks.ProfileId, new[] { 0 }, FreshSeconds);

            state.Advance(FreshSeconds * 0.5f);

            Assert.IsFalse(Block(CvItemId.Name).HasFreshLetters);
            Assert.IsTrue(Block(CvTestBlocks.ProfileId).IsFresh(0));
        }

        [Test]
        public void Reset_FreshLetters_AreClearedWithTheRest()
        {
            state.DeliverLetters(CvItemId.Name, new[] { 0 }, FreshSeconds);

            state.Reset();

            Assert.IsFalse(Block(CvItemId.Name).HasFreshLetters);
        }

        private CvBlockReveal Block(string id)
        {
            Assert.IsTrue(state.TryGetBlock(id, out CvBlockReveal block));
            return block;
        }
    }
}
