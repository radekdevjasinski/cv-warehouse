using System;
using System.Collections.Generic;

namespace CvWarehouse.Core.Cv
{
    public sealed class CvBlockReveal
    {
        private readonly bool[] revealedLetters;
        private readonly bool[] freshLetters;
        private float freshSeconds;

        public CvBlockReveal(CvBlock block)
        {
            Block = block;
            revealedLetters = new bool[block.LetterCount];
            freshLetters = new bool[block.LetterCount];
        }

        public CvBlock Block { get; }

        public int RevealedCount { get; private set; }

        public bool IsComplete => RevealedCount == revealedLetters.Length;

        public bool HasFreshLetters => FreshSecondsLeft > 0f;

        public float FreshSecondsLeft { get; internal set; }

        public float FreshFraction => HasFreshLetters ? FreshSecondsLeft / freshSeconds : 0f;

        public bool IsFresh(int letter)
        {
            return HasFreshLetters && letter >= 0 && letter < freshLetters.Length && freshLetters[letter];
        }

        public bool IsRevealed(int letter)
        {
            return letter < 0 || letter >= revealedLetters.Length || revealedLetters[letter];
        }

        public bool IsWordComplete(int wordIndex)
        {
            CvWord word = Block.Words[wordIndex];
            for (int letter = word.FirstLetter; letter < word.FirstLetter + word.LetterCount; letter++)
                if (!revealedLetters[letter])
                    return false;
            return true;
        }

        internal int Reveal(int firstLetter, int letterCount)
        {
            int first = Math.Max(firstLetter, 0);
            int end = Math.Min(firstLetter + letterCount, revealedLetters.Length);
            int newlyRevealed = 0;
            for (int letter = first; letter < end; letter++)
            {
                if (revealedLetters[letter])
                    continue;

                revealedLetters[letter] = true;
                newlyRevealed++;
            }

            RevealedCount += newlyRevealed;
            return newlyRevealed;
        }

        internal void MarkFresh(IReadOnlyList<int> letters, float seconds)
        {
            Array.Clear(freshLetters, 0, freshLetters.Length);
            for (int index = 0; index < letters.Count; index++)
                if (letters[index] >= 0 && letters[index] < freshLetters.Length)
                    freshLetters[letters[index]] = true;
            freshSeconds = seconds;
            FreshSecondsLeft = seconds;
        }

        internal int Clear()
        {
            int cleared = RevealedCount;
            FreshSecondsLeft = 0f;
            Array.Clear(revealedLetters, 0, revealedLetters.Length);
            RevealedCount = 0;
            return cleared;
        }
    }
}
