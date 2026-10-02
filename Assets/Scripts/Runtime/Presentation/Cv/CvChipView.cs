using CvWarehouse.Core.Cv;
using TMPro;
using UnityEngine;

namespace CvWarehouse.Presentation.Cv
{
    public sealed class CvChipView : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private CvLinkButton linkButton;
        [SerializeField] private CvRevealable revealable;

        public CvRevealable Revealable => revealable;

        public void Show(CvBlockReveal blockReveal, string link)
        {
            label.SetText(blockReveal.Block.Segments[0]);
            linkButton.SetLink(link);
            revealable.Bind(blockReveal);
        }
    }
}
