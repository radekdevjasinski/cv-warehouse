using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class BoxPanelView : MonoBehaviour
    {
        private const string CategoryPrefix = "Category: ";
        private const string NoCategory = "Category: none";

        [SerializeField] private GameObject window;
        [SerializeField, FormerlySerializedAs("weightText")] private TMP_Text sizeText;
        [SerializeField] private TMP_Text importanceText;
        [SerializeField] private TMP_Text categoryText;
        [SerializeField] private Button closeButton;

        private BoxSelection selection;
        private Box shownBox;

        public void Show(BoxSelection boxSelection)
        {
            selection = boxSelection;
            Refresh();
        }

        private static string SizeLabelOf(BoxSizeClass sizeClass)
        {
            switch (sizeClass)
            {
                case BoxSizeClass.Large:
                    return "Size: large";
                case BoxSizeClass.Medium:
                    return "Size: medium";
                default:
                    return "Size: small";
            }
        }

        private static string ImportanceLabelOf(Importance importance)
        {
            switch (importance)
            {
                case Importance.Key:
                    return "Importance: key";
                case Importance.Important:
                    return "Importance: important";
                default:
                    return "Importance: normal";
            }
        }

        private void OnEnable()
        {
            closeButton.onClick.AddListener(Close);
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(Close);
        }

        private void LateUpdate()
        {
            if (selection != null)
                Refresh();
        }

        private void Close()
        {
            selection.Clear();
        }

        private void Refresh()
        {
            if (window.activeSelf != selection.HasBox)
                window.SetActive(selection.HasBox);
            if (!selection.HasBox || shownBox == selection.SelectedBox)
                return;

            shownBox = selection.SelectedBox;
            string categoryName = shownBox.Block.CategoryName;
            sizeText.SetText(SizeLabelOf(shownBox.SizeClass));
            importanceText.SetText(ImportanceLabelOf(shownBox.Importance));
            categoryText.SetText(string.IsNullOrEmpty(categoryName) ? NoCategory : CategoryPrefix + categoryName);
        }
    }
}
