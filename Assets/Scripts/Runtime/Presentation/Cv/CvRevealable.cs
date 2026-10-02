using CvWarehouse.Core.Cv;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Cv
{
    public sealed class CvRevealable : MonoBehaviour
    {
        [SerializeField] private CvLetterMask[] masks;
        [SerializeField] private CanvasGroup content;
        [SerializeField] private Graphic placeholder;

        public CvBlockReveal Reveal { get; private set; }

        public void Bind(CvBlockReveal blockReveal)
        {
            Reveal = blockReveal;
            for (int segmentIndex = 0; segmentIndex < masks.Length; segmentIndex++)
                masks[segmentIndex].Bind(blockReveal, blockReveal.Block.SegmentFirstLetters[segmentIndex]);
            Refresh();
        }

        public void Refresh()
        {
            placeholder.enabled = Reveal.RevealedCount == 0;
            content.blocksRaycasts = Reveal.IsComplete;
            foreach (CvLetterMask mask in masks)
                mask.Refresh();
        }
    }
}
