using CvWarehouse.Core.Cv;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Cv
{
    public sealed class CvDebugRevealPanel : MonoBehaviour
    {
        [SerializeField] private Button revealLetterButton;
        [SerializeField] private Button revealWordButton;
        [SerializeField] private Button revealBlockButton;
        [SerializeField] private Button revealAllButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private int seed = 1;

        private CvRevealState revealState;
        private CvPiecePicker picker;

        public void Bind(CvRevealState state)
        {
            revealState = state;
            picker = new CvPiecePicker(new System.Random(seed));
        }

        private void OnEnable()
        {
            revealLetterButton.onClick.AddListener(RevealLetter);
            revealWordButton.onClick.AddListener(RevealWord);
            revealBlockButton.onClick.AddListener(RevealBlock);
            revealAllButton.onClick.AddListener(RevealAll);
            resetButton.onClick.AddListener(ResetCv);
        }

        private void OnDisable()
        {
            revealLetterButton.onClick.RemoveListener(RevealLetter);
            revealWordButton.onClick.RemoveListener(RevealWord);
            revealBlockButton.onClick.RemoveListener(RevealBlock);
            revealAllButton.onClick.RemoveListener(RevealAll);
            resetButton.onClick.RemoveListener(ResetCv);
        }

        private void RevealLetter()
        {
            if (TryGetIncompleteBlock(out CvBlockReveal block) && picker.TryPickHiddenLetter(block, out int letter))
                revealState.RevealLetter(block.Block.Id, letter);
        }

        private void RevealWord()
        {
            if (TryGetIncompleteBlock(out CvBlockReveal block) && picker.TryPickUnfinishedWord(block, out int wordIndex))
                revealState.RevealWord(block.Block.Id, wordIndex);
        }

        private void RevealBlock()
        {
            if (TryGetIncompleteBlock(out CvBlockReveal block))
                revealState.RevealBlock(block.Block.Id);
        }

        private void RevealAll()
        {
            if (revealState != null)
                revealState.RevealAll();
        }

        private void ResetCv()
        {
            if (revealState != null)
                revealState.Reset();
        }

        private bool TryGetIncompleteBlock(out CvBlockReveal block)
        {
            block = null;
            return revealState != null && revealState.TryGetFirstIncompleteBlock(out block);
        }
    }
}
