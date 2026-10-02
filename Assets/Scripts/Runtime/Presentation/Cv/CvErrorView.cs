using TMPro;
using UnityEngine;

namespace CvWarehouse.Presentation.Cv
{
    public sealed class CvErrorView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text messageText;

        public bool IsVisible => panel.activeSelf;

        public string Message => messageText.text;

        public void Show(string message)
        {
            messageText.SetText(message);
            panel.SetActive(true);
        }
    }
}
