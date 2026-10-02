using System;
using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Cv
{
    public sealed class CvPiecePickerTests
    {
        private const int Seed = 12345;
        private const int ProfileLetterCount = 13;
        private const int ProfileWordCount = 3;

        private CvRevealState state;
        private CvBlockReveal profile;

        [SetUp]
        public void SetUp()
        {
            state = new CvRevealState(CvTestBlocks.Build(CvTestBlocks.NameAndProfileCv));
            state.TryGetBlock(CvTestBlocks.ProfileId, out profile);
        }

        [Test]
        public void TryPickHiddenLetter_RepeatedUntilEmpty_PicksEveryLetterExactlyOnce()
        {
            List<int> pickedLetters = PickAllLetters(new CvPiecePicker(new Random(Seed)));

            Assert.AreEqual(ProfileLetterCount, pickedLetters.Count);
            CollectionAssert.AllItemsAreUnique(pickedLetters);
            Assert.IsTrue(profile.IsComplete);
        }

        [Test]
        public void TryPickHiddenLetter_SameSeed_PicksTheSameOrder()
        {
            List<int> firstRun = PickAllLetters(new CvPiecePicker(new Random(Seed)));
            state.Reset();

            List<int> secondRun = PickAllLetters(new CvPiecePicker(new Random(Seed)));

            CollectionAssert.AreEqual(firstRun, secondRun);
        }

        [Test]
        public void TryPickHiddenLetter_SeededOrder_IsNotReadingOrder()
        {
            List<int> pickedLetters = PickAllLetters(new CvPiecePicker(new Random(Seed)));

            Assert.That(pickedLetters, Is.Not.Ordered);
        }

        [Test]
        public void TryPickHiddenLetter_CompleteBlock_ReturnsFalse()
        {
            state.RevealBlock(CvTestBlocks.ProfileId);

            Assert.IsFalse(new CvPiecePicker(new Random(Seed)).TryPickHiddenLetter(profile, out _));
        }

        [Test]
        public void TryPickUnfinishedWord_RepeatedUntilEmpty_PicksEveryWordExactlyOnce()
        {
            var picker = new CvPiecePicker(new Random(Seed));
            var pickedWords = new List<int>();

            while (picker.TryPickUnfinishedWord(profile, out int wordIndex))
            {
                pickedWords.Add(wordIndex);
                state.RevealWord(CvTestBlocks.ProfileId, wordIndex);
            }

            Assert.AreEqual(ProfileWordCount, pickedWords.Count);
            CollectionAssert.AllItemsAreUnique(pickedWords);
            Assert.IsTrue(profile.IsComplete);
        }

        [Test]
        public void TryPickUnfinishedWord_OneWordLeft_PicksThatWord()
        {
            state.RevealWord(CvTestBlocks.ProfileId, 0);
            state.RevealWord(CvTestBlocks.ProfileId, 2);
            state.RevealLetter(CvTestBlocks.ProfileId, 5);

            Assert.IsTrue(new CvPiecePicker(new Random(Seed)).TryPickUnfinishedWord(profile, out int wordIndex));
            Assert.AreEqual(1, wordIndex);
        }

        private List<int> PickAllLetters(CvPiecePicker picker)
        {
            var pickedLetters = new List<int>();
            while (picker.TryPickHiddenLetter(profile, out int letter))
            {
                pickedLetters.Add(letter);
                state.RevealLetter(CvTestBlocks.ProfileId, letter);
            }
            return pickedLetters;
        }
    }
}
