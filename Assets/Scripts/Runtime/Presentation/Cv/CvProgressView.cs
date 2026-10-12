using CvWarehouse.Core.Cv;
using TMPro;
using UnityEngine;

namespace CvWarehouse.Presentation.Cv
{
    public sealed class CvProgressView : MonoBehaviour
    {
        private const int NothingShown = -1;
        private const float WholePercent = 100f;

        [SerializeField] private RectTransform fill;
        [SerializeField] private TMP_Text percentText;

        private CvRevealState revealState;

        public int ShownPercent { get; private set; } = NothingShown;

        public void Show(CvRevealState state)
        {
            revealState = state;
            Refresh();
            if (isActiveAndEnabled)
                revealState.BlockChanged += OnBlockChanged;
        }

        private void OnEnable()
        {
            if (revealState == null)
                return;

            Refresh();
            revealState.BlockChanged += OnBlockChanged;
        }

        private void OnDisable()
        {
            if (revealState != null)
                revealState.BlockChanged -= OnBlockChanged;
        }

        private void OnBlockChanged(string id)
        {
            Refresh();
        }

        private void Refresh()
        {
            int percent = revealState.RevealedPercent;
            if (ShownPercent == percent)
                return;

            ShownPercent = percent;
            fill.anchorMax = new Vector2(percent / WholePercent, 1f);
            percentText.SetText("{0}%", percent);
        }
    }
}
