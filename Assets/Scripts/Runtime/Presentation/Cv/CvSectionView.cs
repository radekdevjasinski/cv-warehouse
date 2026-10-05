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
        [SerializeField] private CvEntryView keyValueEntryPrefab;
        [SerializeField] private CvChipView chipPrefab;
        [SerializeField] private float widestKeyColumn = 160f;

        private readonly List<CvEntryView> rows = new List<CvEntryView>();

        public void Show(CvSection section, CvRevealState revealState, IDictionary<string, CvRevealable> revealables)
        {
            bool isInline = section.Style == CvListStyle.Inline;
            CvEntryView rowPrefab = section.Style == CvListStyle.KeyValue ? keyValueEntryPrefab : entryPrefab;
            titleText.SetText(section.Title);
            rowsRoot.gameObject.SetActive(!isInline);
            inlineRoot.gameObject.SetActive(isInline);

            foreach (CvEntry entry in section.Entries)
            {
                if (!revealState.TryGetBlock(entry.Id, out CvBlockReveal blockReveal))
                    continue;

                revealables[entry.Id] = isInline ? AddChip(blockReveal, entry.Link) : AddRow(rowPrefab, blockReveal, entry.Link);
            }

            if (section.Style == CvListStyle.KeyValue)
                AlignKeyColumn();
        }

        private CvRevealable AddChip(CvBlockReveal blockReveal, string link)
        {
            CvChipView chip = Instantiate(chipPrefab, inlineRoot);
            chip.Show(blockReveal, link);
            return chip.Revealable;
        }

        private CvRevealable AddRow(CvEntryView rowPrefab, CvBlockReveal blockReveal, string link)
        {
            CvEntryView row = Instantiate(rowPrefab, rowsRoot);
            row.Show(blockReveal, link);
            rows.Add(row);
            return row.Revealable;
        }

        private void AlignKeyColumn()
        {
            float keyColumnWidth = 0f;
            foreach (CvEntryView row in rows)
                keyColumnWidth = Mathf.Max(keyColumnWidth, row.HeadingWidth);

            keyColumnWidth = Mathf.Min(keyColumnWidth, widestKeyColumn);
            foreach (CvEntryView row in rows)
                row.SetHeadingColumnWidth(keyColumnWidth);
        }
    }
}
