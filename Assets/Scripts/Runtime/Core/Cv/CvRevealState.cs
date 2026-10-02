using System;
using System.Collections.Generic;

namespace CvWarehouse.Core.Cv
{
    public sealed class CvRevealState
    {
        private readonly List<CvBlockReveal> blocks = new List<CvBlockReveal>();
        private readonly Dictionary<string, CvBlockReveal> blocksById = new Dictionary<string, CvBlockReveal>();

        public CvRevealState(IReadOnlyList<CvBlock> cvBlocks)
        {
            foreach (CvBlock cvBlock in cvBlocks)
            {
                var block = new CvBlockReveal(cvBlock);
                blocks.Add(block);
                blocksById[cvBlock.Id] = block;
                TotalLetterCount += cvBlock.LetterCount;
            }
        }

        public event Action<string> BlockChanged;

        public IReadOnlyList<CvBlockReveal> Blocks => blocks;

        public int TotalLetterCount { get; }

        public int RevealedLetterCount { get; private set; }

        public bool IsComplete => RevealedLetterCount == TotalLetterCount;

        public bool TryGetBlock(string id, out CvBlockReveal block)
        {
            return blocksById.TryGetValue(id, out block);
        }

        public bool TryGetFirstIncompleteBlock(out CvBlockReveal incompleteBlock)
        {
            foreach (CvBlockReveal block in blocks)
            {
                if (block.IsComplete)
                    continue;

                incompleteBlock = block;
                return true;
            }

            incompleteBlock = null;
            return false;
        }

        public int RevealLetter(string id, int letter)
        {
            return blocksById.TryGetValue(id, out CvBlockReveal block)
                ? Apply(block, block.Reveal(letter, 1))
                : 0;
        }

        public int RevealWord(string id, int wordIndex)
        {
            if (!blocksById.TryGetValue(id, out CvBlockReveal block))
                return 0;
            if (wordIndex < 0 || wordIndex >= block.Block.Words.Count)
                return 0;

            CvWord word = block.Block.Words[wordIndex];
            return Apply(block, block.Reveal(word.FirstLetter, word.LetterCount));
        }

        public int RevealBlock(string id)
        {
            return blocksById.TryGetValue(id, out CvBlockReveal block)
                ? Apply(block, block.Reveal(0, block.Block.LetterCount))
                : 0;
        }

        public void RevealAll()
        {
            foreach (CvBlockReveal block in blocks)
                Apply(block, block.Reveal(0, block.Block.LetterCount));
        }

        public void Reset()
        {
            foreach (CvBlockReveal block in blocks)
                Apply(block, -block.Clear());
        }

        private int Apply(CvBlockReveal block, int revealedLetterChange)
        {
            if (revealedLetterChange == 0)
                return 0;

            RevealedLetterCount += revealedLetterChange;
            BlockChanged?.Invoke(block.Block.Id);
            return revealedLetterChange;
        }
    }
}
