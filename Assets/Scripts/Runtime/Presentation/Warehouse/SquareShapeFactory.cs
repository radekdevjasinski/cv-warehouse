using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class SquareShapeFactory : IDisposable
    {
        private const int TextureSide = 4;

        private readonly Material material;
        private readonly Texture2D texture;
        private readonly Sprite square;

        public SquareShapeFactory(Material material)
        {
            this.material = material;
            texture = new Texture2D(TextureSide, TextureSide, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
            var pixels = new Color32[TextureSide * TextureSide];
            Array.Fill(pixels, new Color32(255, 255, 255, 255));
            texture.SetPixels32(pixels);
            texture.Apply(false, true);
            square = Sprite.Create(texture, new Rect(0f, 0f, TextureSide, TextureSide), new Vector2(0.5f, 0.5f), TextureSide);
        }

        public SpriteRenderer CreateSquare(string name, Transform parent, int sortingOrder)
        {
            var shape = new GameObject(name);
            shape.transform.SetParent(parent, false);
            SpriteRenderer spriteRenderer = shape.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = square;
            spriteRenderer.sharedMaterial = material;
            spriteRenderer.sortingOrder = sortingOrder;
            return spriteRenderer;
        }

        public void Dispose()
        {
            Object.Destroy(square);
            Object.Destroy(texture);
        }
    }
}
