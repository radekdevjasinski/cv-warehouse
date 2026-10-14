using CvWarehouse.Core.Cv;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Cv
{
    public sealed class CvRevealable : MonoBehaviour
    {
        [SerializeField] private CvLetterMask[] masks;
        [SerializeField] private CanvasGroup content;
        [SerializeField] private Graphic shownOnceStarted;

        public CvBlockReveal Reveal { get; private set; }

        public int HiddenWordMarkCount
        {
            get
            {
                int count = 0;
                foreach (CvLetterMask mask in masks)
                    count += mask.HiddenWordMarkCount;
                return count;
            }
        }

        public void Bind(CvBlockReveal blockReveal)
        {
            Reveal = blockReveal;
            for (int segmentIndex = 0; segmentIndex < masks.Length; segmentIndex++)
                masks[segmentIndex].Bind(blockReveal, blockReveal.Block.SegmentFirstGlyphs[segmentIndex]);
            Refresh();
        }

        public void MarkFreshLettersWith(CvHighlightPool pool)
        {
            foreach (CvLetterMask mask in masks)
                mask.MarkFreshLettersWith(pool);
        }

        public void MarkHiddenWordsWith(CvHighlightPool pool)
        {
            foreach (CvLetterMask mask in masks)
                mask.MarkHiddenWordsWith(pool);
        }

        public void Refresh()
        {
            if (shownOnceStarted != null)
                shownOnceStarted.enabled = Reveal.RevealedCount > 0;
            content.blocksRaycasts = Reveal.IsComplete;
            foreach (CvLetterMask mask in masks)
                mask.Refresh();
        }
    }
}
