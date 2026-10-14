using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using TMPro;
using UnityEngine;

namespace CvWarehouse.Presentation.Cv
{
    public sealed class CvDocumentView : MonoBehaviour
    {
        [SerializeField] private CvHeaderView headerView;
        [SerializeField] private RectTransform sectionsRoot;
        [SerializeField] private CvSectionView sectionPrefab;
        [SerializeField] private TMP_Text footerText;
        [SerializeField] private CvAutoScroll autoScroll;
        [SerializeField] private Color freshColor = new Color32(0xe8, 0xc5, 0x47, 0x80);
        [SerializeField] private Color hiddenWordColor = new Color32(0xdd, 0xd5, 0xc2, 0xff);

        private readonly Dictionary<string, CvRevealable> revealables = new Dictionary<string, CvRevealable>();
        private CvRevealState revealState;
        private CvHighlightPool highlights;
        private CvHighlightPool hiddenWords;

        public int FreshMarkCount => highlights == null ? 0 : highlights.RentedCount;

        public int HiddenWordMarkCount => hiddenWords == null ? 0 : hiddenWords.RentedCount;

        public void Show(CvDocument document, CvRevealState state)
        {
            revealState = state;
            headerView.Show(document.Header, state, revealables);
            foreach (CvSection section in document.Sections)
                Instantiate(sectionPrefab, sectionsRoot).Show(section, state, revealables);
            footerText.SetText(document.Footer);
            highlights = new CvHighlightPool(autoScroll.transform, freshColor);
            hiddenWords = new CvHighlightPool(autoScroll.transform, hiddenWordColor);
            foreach (CvRevealable revealable in revealables.Values)
            {
                revealable.MarkFreshLettersWith(highlights);
                revealable.MarkHiddenWordsWith(hiddenWords);
            }
            autoScroll.Follow(state, revealables);

            if (isActiveAndEnabled)
                revealState.BlockChanged += OnBlockChanged;
        }

        private void OnEnable()
        {
            if (revealState == null)
                return;

            foreach (CvRevealable revealable in revealables.Values)
                revealable.Refresh();
            revealState.BlockChanged += OnBlockChanged;
        }

        private void OnDisable()
        {
            if (revealState != null)
                revealState.BlockChanged -= OnBlockChanged;
        }

        private void OnBlockChanged(string id)
        {
            if (revealables.TryGetValue(id, out CvRevealable revealable))
                revealable.Refresh();
        }
    }
}
