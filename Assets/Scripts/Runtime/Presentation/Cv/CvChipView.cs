using CvWarehouse.Core.Cv;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Cv
{
    public sealed class CvChipView : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private CvLinkButton linkButton;
        [SerializeField] private CvRevealable revealable;
        [SerializeField] private Image icon;
        [SerializeField] private CvContactIcons icons;
        [SerializeField] private float iconSpace = 21f;

        public CvRevealable Revealable => revealable;

        public bool HasIcon => icon.gameObject.activeSelf;

        public bool IsIconVisible => HasIcon && icon.enabled;

        public Sprite ShownIcon => icon.sprite;

        public void Show(CvBlockReveal blockReveal, string link)
        {
            label.SetText(blockReveal.Block.Segments[0]);
            linkButton.SetLink(link);
            revealable.Bind(blockReveal);
        }

        public void ShowContact(CvBlockReveal blockReveal, CvContact contact)
        {
            icon.sprite = icons.IconOf(contact.Kind);
            icon.color = label.color;
            icon.gameObject.SetActive(true);
            label.margin = new Vector4(iconSpace, 0f, 0f, 0f);
            Show(blockReveal, contact.Link);
        }
    }
}
