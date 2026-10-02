using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Cv
{
    [RequireComponent(typeof(Button))]
    public sealed class CvLinkButton : MonoBehaviour
    {
        private static readonly string[] AllowedSchemes = { "https://", "http://", "mailto:", "tel:" };

        [SerializeField] private Button button;
        [SerializeField] private TMP_Text label;
        [SerializeField] private Color linkColor = Color.blue;

        private string link = string.Empty;

        public string Link => link;

        public void SetLink(string newLink)
        {
            link = IsAllowed(newLink) ? newLink : string.Empty;
            button.interactable = link.Length > 0;
            if (link.Length > 0)
                label.color = linkColor;
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OpenLink);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OpenLink);
        }

        private void OpenLink()
        {
            if (link.Length > 0)
                Application.OpenURL(link);
        }

        private static bool IsAllowed(string candidate)
        {
            foreach (string scheme in AllowedSchemes)
                if (candidate.StartsWith(scheme, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }
    }
}
