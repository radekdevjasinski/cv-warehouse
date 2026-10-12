using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Warehouse
{
    [RequireComponent(typeof(RawImage))]
    public sealed class ScanlinesView : MonoBehaviour
    {
        [SerializeField] private int linePeriodPixels = 3;
        [SerializeField, Range(0f, 1f)] private float lineOpacity = 0.32f;

        private RawImage image;
        private Texture2D texture;
        private float fittedPixelHeight;

        public int LinePeriodPixels => linePeriodPixels;

        public float ScreenPixelHeight => image.rectTransform.rect.height * image.canvas.scaleFactor;

        public float ShownRepeats => image.uvRect.height;

        private void Awake()
        {
            image = GetComponent<RawImage>();
            texture = CreateTexture();
            ViewOverlay.Cover(image, texture);
        }

        private void LateUpdate()
        {
            float pixelHeight = ScreenPixelHeight;
            if (Mathf.Approximately(pixelHeight, fittedPixelHeight))
                return;

            fittedPixelHeight = pixelHeight;
            image.uvRect = new Rect(0f, 0f, 1f, pixelHeight / linePeriodPixels);
        }

        private void OnDestroy()
        {
            Destroy(texture);
        }

        private Texture2D CreateTexture()
        {
            var lines = new Texture2D(1, linePeriodPixels, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Repeat
            };
            var pixels = new Color32[linePeriodPixels];
            pixels[0] = new Color(0f, 0f, 0f, lineOpacity);
            lines.SetPixels32(pixels);
            lines.Apply(false, true);
            return lines;
        }
    }
}
