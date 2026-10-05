namespace CvWarehouse.Core.Cv
{
    public sealed class CvDeliveryReveal
    {
        private readonly CvRevealState state;
        private readonly CvPiecePicker picker;

        public CvDeliveryReveal(CvRevealState state, CvPiecePicker picker)
        {
            this.state = state;
            this.picker = picker;
        }

        public void ShowProgress(int deliveredPieces, int totalPieces)
        {
            int targetLetters = TargetLetterCount(deliveredPieces, totalPieces);
            while (state.RevealedLetterCount < targetLetters && TryRevealNextLetter())
            {
            }
        }

        private int TargetLetterCount(int deliveredPieces, int totalPieces)
        {
            if (deliveredPieces >= totalPieces)
                return state.TotalLetterCount;

            return (int)((long)state.TotalLetterCount * deliveredPieces / totalPieces);
        }

        private bool TryRevealNextLetter()
        {
            return state.TryGetFirstIncompleteBlock(out CvBlockReveal block)
                && picker.TryPickHiddenLetter(block, out int letter)
                && state.RevealLetter(block.Block.Id, letter) > 0;
        }
    }
}
