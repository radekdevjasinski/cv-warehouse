using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Cv
{
    public sealed class CvHighlightPool
    {
        private readonly Stack<Image> spare = new Stack<Image>();
        private readonly Transform storage;
        private readonly Color color;

        public CvHighlightPool(Transform storage, Color color)
        {
            this.storage = storage;
            this.color = color;
        }

        public int RentedCount { get; private set; }

        public Image Rent(Transform parent)
        {
            Image mark = spare.Count > 0 ? spare.Pop() : Create();
            mark.transform.SetParent(parent, false);
            mark.gameObject.SetActive(true);
            RentedCount++;
            return mark;
        }

        public void Return(Image mark)
        {
            mark.gameObject.SetActive(false);
            mark.transform.SetParent(storage, false);
            spare.Push(mark);
            RentedCount--;
        }

        private Image Create()
        {
            var markObject = new GameObject("FreshMark", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            Image mark = markObject.GetComponent<Image>();
            mark.color = color;
            mark.raycastTarget = false;
            return mark;
        }
    }
}
