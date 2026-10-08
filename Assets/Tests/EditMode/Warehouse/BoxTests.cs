using System;
using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Tests.EditMode.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class BoxTests
    {
        private const int Seed = 12345;
        private const string Text = "Hello big world";
        private const int LetterCount = 13;
        private const int WordCount = 3;

        private Box box;
        private Random random;
        private List<int> claimed;

        [SetUp]
        public void SetUp()
        {
            box = new Box(0, new GridPosition(0, 0), WeightClass.Medium, CvTestBlocks.FromText(Text));
            random = new Random(Seed);
            claimed = new List<int>();
        }

        [Test]
        public void NewBox_HoldsEveryLetterOfItsBlock()
        {
            Assert.AreEqual(LetterCount, box.TotalPieces);
            Assert.AreEqual(LetterCount, box.RemainingPieces);
            Assert.AreEqual(LetterCount, box.UnclaimedPieces);
        }

        [Test]
        public void Claim_Letter_TakesOneLetterAtATimeAndNeverTheSameOneTwice()
        {
            var everyLetter = new HashSet<int>();

            for (int trip = 0; trip < LetterCount; trip++)
            {
                Assert.AreEqual(1, box.Claim(CarrySize.Letter, random, claimed));
                Assert.IsTrue(everyLetter.Add(claimed[0]));
            }

            Assert.AreEqual(LetterCount, everyLetter.Count);
            Assert.AreEqual(0, box.UnclaimedPieces);
        }

        [Test]
        public void Claim_Word_TakesAWholeWordPerTrip()
        {
            var wordLengths = new List<int>();

            for (int trip = 0; trip < WordCount; trip++)
            {
                box.Claim(CarrySize.Word, random, claimed);
                wordLengths.Add(claimed.Count);
                for (int index = 1; index < claimed.Count; index++)
                    Assert.AreEqual(claimed[index - 1] + 1, claimed[index]);
            }

            CollectionAssert.AreEquivalent(new[] { 5, 3, 5 }, wordLengths);
            Assert.AreEqual(0, box.UnclaimedPieces);
        }

        [Test]
        public void Claim_WordAfterALetterWasTaken_TakesOnlyTheRestOfThatWord()
        {
            var bigBox = new Box(0, new GridPosition(0, 0), WeightClass.Light, CvTestBlocks.FromText("big"));
            bigBox.Claim(CarrySize.Letter, random, claimed);
            int takenLetter = claimed[0];

            Assert.AreEqual(2, bigBox.Claim(CarrySize.Word, random, claimed));

            CollectionAssert.DoesNotContain(claimed, takenLetter);
        }

        [Test]
        public void Claim_Box_TakesEverythingThatIsLeft()
        {
            box.Claim(CarrySize.Letter, random, claimed);

            Assert.AreEqual(LetterCount - 1, box.Claim(CarrySize.Box, random, claimed));
            Assert.AreEqual(0, box.UnclaimedPieces);
        }

        [Test]
        public void Claim_NothingLeftUnclaimed_ClaimsNothing()
        {
            box.Claim(CarrySize.Box, random, claimed);

            Assert.AreEqual(0, box.Claim(CarrySize.Letter, random, claimed));
            Assert.IsEmpty(claimed);
        }

        [Test]
        public void ReleaseClaim_ClaimedWord_PutsItsLettersBack()
        {
            box.Claim(CarrySize.Word, random, claimed);

            box.ReleaseClaim(claimed);

            Assert.AreEqual(LetterCount, box.UnclaimedPieces);
            Assert.IsEmpty(claimed);
            Assert.AreEqual(LetterCount, box.Claim(CarrySize.Box, random, claimed));
        }

        [Test]
        public void TakeClaimed_EveryLetter_EmptiesTheBox()
        {
            box.TakeClaimed(box.Claim(CarrySize.Box, random, claimed));

            Assert.IsTrue(box.IsEmpty);
        }

        [Test]
        public void Claim_SameSeed_PicksTheSameLetters()
        {
            var otherBox = new Box(0, new GridPosition(0, 0), WeightClass.Medium, CvTestBlocks.FromText(Text));
            var otherRandom = new Random(Seed);
            var otherClaimed = new List<int>();

            for (int trip = 0; trip < LetterCount; trip++)
            {
                box.Claim(CarrySize.Letter, random, claimed);
                otherBox.Claim(CarrySize.Letter, otherRandom, otherClaimed);
                Assert.AreEqual(claimed[0], otherClaimed[0]);
            }
        }
    }
}
