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
        private const int EveryWord = int.MaxValue;

        private Box box;
        private Random random;
        private List<int> claimed;

        [SetUp]
        public void SetUp()
        {
            box = new Box(0, new GridPosition(0, 0), TestBoxKinds.Medium, CvTestBlocks.FromText(Text));
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
        public void Claim_OneWord_TakesAWholeWordPerTrip()
        {
            var wordLengths = new List<int>();

            for (int trip = 0; trip < WordCount; trip++)
            {
                box.Claim(1, random, claimed);
                wordLengths.Add(claimed.Count);
                for (int index = 1; index < claimed.Count; index++)
                    Assert.AreEqual(claimed[index - 1] + 1, claimed[index]);
            }

            CollectionAssert.AreEquivalent(new[] { 5, 3, 5 }, wordLengths);
            Assert.AreEqual(0, box.UnclaimedPieces);
        }

        [Test]
        public void Claim_TwoWords_TakesTwoDifferentWholeWords()
        {
            int claimedLetters = box.Claim(2, random, claimed);

            Assert.That(claimedLetters, Is.EqualTo(8).Or.EqualTo(10));
            CollectionAssert.AllItemsAreUnique(claimed);
            Assert.AreEqual(LetterCount - claimedLetters, box.UnclaimedPieces);
        }

        [Test]
        public void Claim_MoreWordsThanAreLeft_TakesEverythingThatIsLeft()
        {
            box.Claim(1, random, claimed);
            int firstWordLetters = claimed.Count;

            Assert.AreEqual(LetterCount - firstWordLetters, box.Claim(5, random, claimed));
            Assert.AreEqual(0, box.UnclaimedPieces);
        }

        [Test]
        public void Claim_NothingLeftUnclaimed_ClaimsNothing()
        {
            box.Claim(EveryWord, random, claimed);

            Assert.AreEqual(0, box.Claim(1, random, claimed));
            Assert.IsEmpty(claimed);
        }

        [Test]
        public void ReleaseClaim_ClaimedWord_PutsItsLettersBack()
        {
            box.Claim(1, random, claimed);

            box.ReleaseClaim(claimed);

            Assert.AreEqual(LetterCount, box.UnclaimedPieces);
            Assert.IsEmpty(claimed);
            Assert.AreEqual(LetterCount, box.Claim(EveryWord, random, claimed));
        }

        [Test]
        public void TakeClaimed_EveryLetter_EmptiesTheBox()
        {
            box.TakeClaimed(box.Claim(EveryWord, random, claimed));

            Assert.IsTrue(box.IsEmpty);
        }

        [Test]
        public void Claim_SameSeed_PicksTheSameWords()
        {
            var otherBox = new Box(0, new GridPosition(0, 0), TestBoxKinds.Medium, CvTestBlocks.FromText(Text));
            var otherRandom = new Random(Seed);
            var otherClaimed = new List<int>();

            for (int trip = 0; trip < WordCount; trip++)
            {
                box.Claim(1, random, claimed);
                otherBox.Claim(1, otherRandom, otherClaimed);
                CollectionAssert.AreEqual(claimed, otherClaimed);
            }
        }
    }
}
