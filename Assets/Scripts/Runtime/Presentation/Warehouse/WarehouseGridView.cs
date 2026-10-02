using CvWarehouse.Core.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class WarehouseGridView : MonoBehaviour
    {
        private const int SortingOrder = 0;

        [SerializeField] private Color32 floorColor = new Color32(58, 62, 70, 255);
        [SerializeField] private Color32 blockedColor = new Color32(30, 32, 38, 255);
        [SerializeField] private Color32 accessColor = new Color32(74, 80, 92, 255);
        [SerializeField] private Color32 dropColor = new Color32(70, 160, 110, 255);
        [SerializeField] private Color32 parkingColor = new Color32(70, 110, 170, 255);
        [SerializeField] private Color32 truckColor = new Color32(220, 220, 225, 255);

        private WarehouseLayout layout;
        private Texture2D texture;
        private Sprite sprite;
        private Color32[] pixels;
        private int shownVersion;

        public void Show(WarehouseLayout shownLayout, SquareShapeFactory shapes)
        {
            layout = shownLayout;
            WarehouseGrid grid = layout.Grid;
            pixels = new Color32[grid.CellCount];
            texture = new Texture2D(grid.Width, grid.Height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            sprite = Sprite.Create(texture, new Rect(0f, 0f, grid.Width, grid.Height), Vector2.zero, 1f);
            shapes.CreateSquare("Grid", transform, SortingOrder).sprite = sprite;
            Repaint();
        }

        private void LateUpdate()
        {
            if (layout != null && shownVersion != layout.Version)
                Repaint();
        }

        private void OnDestroy()
        {
            Destroy(sprite);
            Destroy(texture);
        }

        private void Repaint()
        {
            shownVersion = layout.Version;
            WarehouseGrid grid = layout.Grid;
            for (int cellIndex = 0; cellIndex < pixels.Length; cellIndex++)
                pixels[cellIndex] = ColorOf(grid.GetCellType(grid.PositionOf(cellIndex)));

            for (int box = 0; box < layout.Boxes.Count; box++)
                PaintAccessCells(layout.Boxes[box]);

            PaintTruck();
            texture.SetPixels32(pixels);
            texture.Apply(false);
        }

        private void PaintAccessCells(Box box)
        {
            for (int index = 0; index < box.AccessCells.Count; index++)
                pixels[layout.Grid.IndexOf(box.AccessCells[index])] = accessColor;
        }

        private void PaintTruck()
        {
            GridPosition origin = layout.TruckOrigin;
            for (int y = origin.Y; y < origin.Y + layout.TruckSize.Height; y++)
                for (int x = origin.X; x < origin.X + layout.TruckSize.Width; x++)
                    pixels[layout.Grid.IndexOf(new GridPosition(x, y))] = truckColor;
        }

        private Color32 ColorOf(CellType cellType)
        {
            switch (cellType)
            {
                case CellType.Blocked:
                    return blockedColor;
                case CellType.Drop:
                    return dropColor;
                case CellType.Parking:
                    return parkingColor;
                default:
                    return floorColor;
            }
        }
    }
}
