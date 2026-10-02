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

        private readonly Dictionary<string, CvRevealable> revealables = new Dictionary<string, CvRevealable>();
        private CvRevealState revealState;

        public void Show(CvDocument document, CvRevealState state)
        {
            revealState = state;
            headerView.Show(document.Header, state, revealables);
            foreach (CvSection section in document.Sections)
                Instantiate(sectionPrefab, sectionsRoot).Show(section, state, revealables);
            footerText.SetText(document.Footer);

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
