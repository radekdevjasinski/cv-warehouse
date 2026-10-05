using CvWarehouse.Presentation.Cv;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Tests.PlayMode.Cv
{
    public sealed class CvLayoutTests
    {
        private const float RowWidth = 100f;
        private const float ItemWidth = 40f;
        private const float ItemHeight = 10f;
        private const float RowSpacing = 4f;
        private const float HeadingRowWidth = 250f;
        private const float FontSize = 10f;
        private const float Tolerance = 0.5f;

        private GameObject canvas;

        [SetUp]
        public void SetUp()
        {
            canvas = new GameObject("Canvas", typeof(Canvas));
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(canvas);
        }

        [Test]
        public void FlowLayoutGroup_ItemsWiderThanTheRow_WrapOntoTheNextRow()
        {
            RectTransform row = AddChild<FlowLayoutGroup>(canvas.transform, RowWidth);
            RectTransform first = AddItem(row);
            RectTransform second = AddItem(row);
            RectTransform third = AddItem(row);

            LayoutRebuilder.ForceRebuildLayoutImmediate(row);

            Assert.AreEqual(first.anchoredPosition.y, second.anchoredPosition.y, Tolerance);
            Assert.Greater(second.anchoredPosition.x, first.anchoredPosition.x);
            Assert.Less(third.anchoredPosition.y, first.anchoredPosition.y);
            Assert.AreEqual(first.anchoredPosition.x, third.anchoredPosition.x, Tolerance);
            Assert.AreEqual(ItemHeight * 2f + RowSpacing, LayoutUtility.GetPreferredHeight(row), Tolerance);
        }

        [Test]
        public void KeepPreferredWidth_RowTooNarrowForBothTexts_KeepsTheLinkWholeAndShrinksTheHeading()
        {
            RectTransform row = AddChild<HorizontalLayoutGroup>(canvas.transform, HeadingRowWidth);
            var group = row.GetComponent<HorizontalLayoutGroup>();
            group.childControlWidth = true;
            group.childForceExpandWidth = false;
            TMP_Text heading = AddText(row, "Machine Learning FPS with Unity and ML-Agents and more", TextWrappingModes.Normal);
            heading.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            TMP_Text link = AddText(row, "radekdev.itch.io/machine-learning-fps", TextWrappingModes.NoWrap);
            link.gameObject.AddComponent<KeepPreferredWidth>();

            LayoutRebuilder.ForceRebuildLayoutImmediate(row);

            Assert.AreEqual(link.preferredWidth, link.rectTransform.rect.width, Tolerance);
            Assert.AreEqual(HeadingRowWidth - link.preferredWidth, heading.rectTransform.rect.width, Tolerance);
        }

        private static RectTransform AddItem(RectTransform row)
        {
            RectTransform item = AddChild<LayoutElement>(row, ItemWidth);
            var element = item.GetComponent<LayoutElement>();
            element.preferredWidth = ItemWidth;
            element.preferredHeight = ItemHeight;
            return item;
        }

        private static TMP_Text AddText(RectTransform row, string content, TextWrappingModes wrapping)
        {
            TMP_Text text = AddChild<TextMeshProUGUI>(row, HeadingRowWidth).GetComponent<TMP_Text>();
            text.fontSize = FontSize;
            text.textWrappingMode = wrapping;
            text.text = content;
            return text;
        }

        private static RectTransform AddChild<TComponent>(Transform parent, float width) where TComponent : Component
        {
            var child = new GameObject(typeof(TComponent).Name, typeof(RectTransform), typeof(TComponent));
            var rectTransform = (RectTransform)child.transform;
            rectTransform.SetParent(parent, false);
            rectTransform.sizeDelta = new Vector2(width, ItemHeight);
            return rectTransform;
        }
    }
}
