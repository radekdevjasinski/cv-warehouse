using UnityEngine;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Warehouse
{
    [RequireComponent(typeof(RawImage))]
    public sealed class VignetteView : MonoBehaviour
    {
        private const int TextureSide = 64;

        [SerializeField, Range(0f, 1f)] private float clearRadius = 0.55f;
        [SerializeField, Range(0f, 1f)] private float cornerOpacity = 0.45f;

        private Texture2D texture;

        private void Awake()
        {
            texture = CreateTexture();
            ViewOverlay.Cover(GetComponent<RawImage>(), texture);
        }

        private void OnDestroy()
        {
            Destroy(texture);
        }

        private Texture2D CreateTexture()
        {
            var vignette = new Texture2D(TextureSide, TextureSide, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            var pixels = new Color32[TextureSide * TextureSide];
            for (int y = 0; y < TextureSide; y++)
                for (int x = 0; x < TextureSide; x++)
                    pixels[y * TextureSide + x] = new Color(0f, 0f, 0f, OpacityAt(x, y));

            vignette.SetPixels32(pixels);
            vignette.Apply(false, true);
            return vignette;
        }

        private float OpacityAt(int x, int y)
        {
            float halfSide = (TextureSide - 1) * 0.5f;
            var fromCentre = new Vector2((x - halfSide) / halfSide, (y - halfSide) / halfSide);
            return VignetteShape.OpacityAt(fromCentre.magnitude / Mathf.Sqrt(2f), clearRadius, cornerOpacity);
        }
    }
}
