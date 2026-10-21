using CvWarehouse.Core.Menu;
using UnityEngine;

namespace CvWarehouse.Presentation.Menu
{
    public sealed class MenuTitleBob : MonoBehaviour
    {
        [SerializeField] private MenuBackdropConfig config;
        [SerializeField] private RectTransform title;

        private Vector2 restingPosition;
        private float elapsedSeconds;
        private float shownOffset;

        public float Offset => shownOffset;

        private void Awake()
        {
            restingPosition = title.anchoredPosition;
        }

        private void Update()
        {
            elapsedSeconds += Time.deltaTime;
            float offset = TitleBob.OffsetAt(elapsedSeconds, config.TitleBobSeconds, config.TitleBobHeight, config.TitleBobStep);
            if (offset == shownOffset)
                return;

            shownOffset = offset;
            title.anchoredPosition = new Vector2(restingPosition.x, restingPosition.y + offset);
        }
    }
}
