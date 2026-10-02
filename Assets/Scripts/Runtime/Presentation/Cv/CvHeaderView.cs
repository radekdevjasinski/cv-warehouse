using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using TMPro;
using UnityEngine;

namespace CvWarehouse.Presentation.Cv
{
    public sealed class CvHeaderView : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private CvRevealable nameRevealable;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private CvRevealable titleRevealable;
        [SerializeField] private RectTransform contactsRoot;
        [SerializeField] private CvChipView chipPrefab;

        public void Show(CvHeader header, CvRevealState revealState, IDictionary<string, CvRevealable> revealables)
        {
            nameText.SetText(header.Name);
            titleText.SetText(header.Title);
            BindLine(CvItemId.Name, nameRevealable, revealState, revealables);
            BindLine(CvItemId.JobTitle, titleRevealable, revealState, revealables);

            foreach (CvContact contact in header.Contacts)
            {
                if (!revealState.TryGetBlock(contact.Id, out CvBlockReveal blockReveal))
                    continue;

                CvChipView chip = Instantiate(chipPrefab, contactsRoot);
                chip.Show(blockReveal, contact.Link);
                revealables[contact.Id] = chip.Revealable;
            }
        }

        private static void BindLine(
            string id,
            CvRevealable revealable,
            CvRevealState revealState,
            IDictionary<string, CvRevealable> revealables)
        {
            if (!revealState.TryGetBlock(id, out CvBlockReveal blockReveal))
            {
                revealable.gameObject.SetActive(false);
                return;
            }

            revealable.Bind(blockReveal);
            revealables[id] = revealable;
        }
    }
}
