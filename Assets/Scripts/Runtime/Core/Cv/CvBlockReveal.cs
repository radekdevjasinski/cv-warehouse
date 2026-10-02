using System;

namespace CvWarehouse.Core.Cv
{
    public sealed class CvBlockReveal
    {
        private readonly bool[] revealedLetters;

        public CvBlockReveal(CvBlock block)
        {
            Block = block;
            revealedLetters = new bool[block.LetterCount];
        }

        public CvBlock Block { get; }

        public int RevealedCount { get; private set; }

        public bool IsComplete => RevealedCount == revealedLetters.Length;

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

        internal int Clear()
        {
            int cleared = RevealedCount;
            Array.Clear(revealedLetters, 0, revealedLetters.Length);
            RevealedCount = 0;
            return cleared;
        }
    }
}
