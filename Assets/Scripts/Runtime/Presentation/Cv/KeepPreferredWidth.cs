using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Cv
{
    [RequireComponent(typeof(TMP_Text))]
    public sealed class KeepPreferredWidth : MonoBehaviour, ILayoutElement
    {
        private const float NotSet = -1f;
        private const int AboveTheText = 2;

        [SerializeField] private float widestKept = 260f;

        private TMP_Text text;

        public float minWidth => Mathf.Min(text.preferredWidth, widestKept);

        public float preferredWidth => NotSet;

        public float flexibleWidth => NotSet;

        public float minHeight => NotSet;

        public float preferredHeight => NotSet;

        public float flexibleHeight => NotSet;

        public int layoutPriority => AboveTheText;

        public void CalculateLayoutInputHorizontal()
        {
        }

        public void CalculateLayoutInputVertical()
        {
        }

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
        }
    }
}
