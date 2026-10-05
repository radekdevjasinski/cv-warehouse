using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Cv
{
    [RequireComponent(typeof(ScrollRect))]
    public sealed class CvPageWidth : UIBehaviour
    {
        [SerializeField] private float widestPage = 920f;

        private ScrollRect scrollRect;

        protected override void Awake()
        {
            base.Awake();
            scrollRect = GetComponent<ScrollRect>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Fit();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (isActiveAndEnabled)
                Fit();
        }

        private void Fit()
        {
            RectTransform page = scrollRect.content;
            float width = Mathf.Min(widestPage, scrollRect.viewport.rect.width);
            if (!Mathf.Approximately(page.rect.width, width))
                page.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }
    }
}
