using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using TMPro;
using UnityEngine;

namespace CvWarehouse.Presentation.Cv
{
    public sealed class CvSectionView : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private RectTransform rowsRoot;
        [SerializeField] private RectTransform inlineRoot;
        [SerializeField] private CvEntryView entryPrefab;
        [SerializeField] private CvChipView chipPrefab;

        public void Show(CvSection section, CvRevealState revealState, IDictionary<string, CvRevealable> revealables)
        {
            bool isInline = section.Style == CvListStyle.Inline;
            titleText.SetText(section.Title);
            rowsRoot.gameObject.SetActive(!isInline);
            inlineRoot.gameObject.SetActive(isInline);

            foreach (CvEntry entry in section.Entries)
            {
                if (!revealState.TryGetBlock(entry.Id, out CvBlockReveal blockReveal))
                    continue;

                revealables[entry.Id] = isInline ? AddChip(blockReveal, entry.Link) : AddRow(blockReveal, entry.Link);
            }
        }

        private CvRevealable AddChip(CvBlockReveal blockReveal, string link)
        {
            CvChipView chip = Instantiate(chipPrefab, inlineRoot);
            chip.Show(blockReveal, link);
            return chip.Revealable;
        }

        private CvRevealable AddRow(CvBlockReveal blockReveal, string link)
        {
            CvEntryView row = Instantiate(entryPrefab, rowsRoot);
            row.Show(blockReveal, link);
            return row.Revealable;
        }
    }
}
