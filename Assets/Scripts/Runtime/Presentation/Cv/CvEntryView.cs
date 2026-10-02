using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using TMPro;
using UnityEngine;

namespace CvWarehouse.Presentation.Cv
{
    public sealed class CvEntryView : MonoBehaviour
    {
        private const int HeadingSegment = 0;
        private const int MetaSegment = 1;
        private const int BodySegment = 2;

        [SerializeField] private GameObject headingRow;
        [SerializeField] private TMP_Text headingText;
        [SerializeField] private TMP_Text metaText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private CvLinkButton linkButton;
        [SerializeField] private CvRevealable revealable;

        public CvRevealable Revealable => revealable;

        public void Show(CvBlockReveal blockReveal, string link)
        {
            IReadOnlyList<string> segments = blockReveal.Block.Segments;
            headingText.SetText(segments[HeadingSegment]);
            metaText.SetText(segments[MetaSegment]);
            bodyText.SetText(segments[BodySegment]);
            headingRow.SetActive(segments[HeadingSegment].Length > 0 || segments[MetaSegment].Length > 0);
            bodyText.gameObject.SetActive(segments[BodySegment].Length > 0);
            linkButton.SetLink(link);
            revealable.Bind(blockReveal);
        }
    }
}
