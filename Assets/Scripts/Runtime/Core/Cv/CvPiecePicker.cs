using System;

namespace CvWarehouse.Core.Cv
{
    public sealed class CvPiecePicker
    {
        private readonly Random random;

        public CvPiecePicker(Random random)
        {
            this.random = random;
        }

        public bool TryPickHiddenLetter(CvBlockReveal block, out int pickedLetter)
        {
            int hiddenCount = block.Block.LetterCount - block.RevealedCount;
            pickedLetter = 0;
            if (hiddenCount == 0)
                return false;

            int hiddenToSkip = random.Next(hiddenCount);
            while (block.IsRevealed(pickedLetter) || hiddenToSkip-- > 0)
                pickedLetter++;
            return true;
        }

        public bool TryPickUnfinishedWord(CvBlockReveal block, out int pickedWord)
        {
            int unfinishedCount = CountUnfinishedWords(block);
            pickedWord = 0;
            if (unfinishedCount == 0)
                return false;

            int unfinishedToSkip = random.Next(unfinishedCount);
            while (block.IsWordComplete(pickedWord) || unfinishedToSkip-- > 0)
                pickedWord++;
            return true;
        }

        private static int CountUnfinishedWords(CvBlockReveal block)
        {
            int unfinishedCount = 0;
            for (int wordIndex = 0; wordIndex < block.Block.Words.Count; wordIndex++)
                if (!block.IsWordComplete(wordIndex))
                    unfinishedCount++;
            return unfinishedCount;
        }
    }
}
