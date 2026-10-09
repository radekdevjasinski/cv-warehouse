using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class BoxPanelView : MonoBehaviour
    {
        private const string CategoryPrefix = "Category: ";
        private const string NoCategory = "Category: none";

        [SerializeField] private GameObject window;
        [SerializeField] private TMP_Text weightText;
        [SerializeField] private TMP_Text categoryText;
        [SerializeField] private Button closeButton;

        private BoxSelection selection;
        private Box shownBox;

        public void Show(BoxSelection boxSelection)
        {
            selection = boxSelection;
            Refresh();
        }

        private static string LabelOf(WeightClass weightClass)
        {
            switch (weightClass)
            {
                case WeightClass.Heavy:
                    return "Weight: heavy";
                case WeightClass.Medium:
                    return "Weight: medium";
                default:
                    return "Weight: light";
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
            weightText.SetText(LabelOf(shownBox.WeightClass));
            categoryText.SetText(string.IsNullOrEmpty(categoryName) ? NoCategory : CategoryPrefix + categoryName);
        }
    }
}
