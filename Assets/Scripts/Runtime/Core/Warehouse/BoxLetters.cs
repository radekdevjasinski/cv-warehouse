using System;
using System.Collections.Generic;
using CvWarehouse.Core.Cv;

namespace CvWarehouse.Core.Warehouse
{
    internal sealed class BoxLetters
    {
        private readonly CvBlock block;
        private readonly bool[] claimedLetters;

        public BoxLetters(CvBlock block)
        {
            this.block = block;
            claimedLetters = new bool[block.LetterCount];
            UnclaimedCount = block.LetterCount;
        }

        public int UnclaimedCount { get; private set; }

        public void ClaimLetter(Random random, List<int> claimed)
        {
            int unclaimedToSkip = random.Next(UnclaimedCount);
            int letter = 0;
            while (claimedLetters[letter] || unclaimedToSkip-- > 0)
                letter++;
            Claim(letter, claimed);
        }

        public void ClaimWord(Random random, List<int> claimed)
        {
            int openWordsToSkip = random.Next(CountOpenWords());
            int wordIndex = 0;
            while (!IsOpen(block.Words[wordIndex]) || openWordsToSkip-- > 0)
                wordIndex++;

            CvWord word = block.Words[wordIndex];
            for (int letter = word.FirstLetter; letter < word.FirstLetter + word.LetterCount; letter++)
                if (!claimedLetters[letter])
                    Claim(letter, claimed);
        }

        public void ClaimRest(List<int> claimed)
        {
            for (int letter = 0; letter < claimedLetters.Length; letter++)
                if (!claimedLetters[letter])
                    Claim(letter, claimed);
        }

        public void Release(List<int> claimed)
        {
            for (int index = 0; index < claimed.Count; index++)
                claimedLetters[claimed[index]] = false;
            UnclaimedCount += claimed.Count;
        }

        private void Claim(int letter, List<int> claimed)
        {
            claimedLetters[letter] = true;
            UnclaimedCount--;
            claimed.Add(letter);
        }

        private int CountOpenWords()
        {
            int openWords = 0;
            for (int wordIndex = 0; wordIndex < block.Words.Count; wordIndex++)
                if (IsOpen(block.Words[wordIndex]))
                    openWords++;
            return openWords;
        }

        private bool IsOpen(CvWord word)
        {
            for (int letter = word.FirstLetter; letter < word.FirstLetter + word.LetterCount; letter++)
                if (!claimedLetters[letter])
                    return true;
            return false;
        }
    }
}
