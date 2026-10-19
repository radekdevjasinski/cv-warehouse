using CvWarehouse.Core.Warehouse;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class WarehouseGridView : MonoBehaviour
    {
        private const int SortingOrder = 0;
        private const int SpeckEvery = 5;
        private const int SpeckStepX = 7;
        private const int SpeckStepY = 13;

        [Header("Floor, one sprite for each colour of the checkerboard")]
        [SerializeField] private Sprite[] floorSprites;
        [SerializeField] private Sprite[] speckledFloorSprites;
        [SerializeField] private Sprite wallSprite;

        private WarehouseLayout layout;
        private Tilemap tilemap;
        private Tile[] floorTiles;
        private Tile[] speckledFloorTiles;
        private Tile wallTile;
        private bool[] isUnderBox;
        private int shownVersion;

        public Sprite SpriteAt(GridPosition cell)
        {
            return tilemap.GetSprite(new Vector3Int(cell.X, cell.Y, 0));
        }

        public void Show(WarehouseLayout shownLayout, Material material)
        {
            layout = shownLayout;
            isUnderBox = new bool[layout.Grid.CellCount];
            CreateTiles();
            CreateTilemap(material);
            Repaint();
        }

        private void LateUpdate()
        {
            if (layout != null && shownVersion != layout.Version)
                Repaint();
        }

        private void OnDestroy()
        {
            if (floorTiles == null)
                return;

            DestroyTiles(floorTiles);
            DestroyTiles(speckledFloorTiles);
            Destroy(wallTile);
        }

        private static Tile CreateTile(Sprite sprite)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.colliderType = Tile.ColliderType.None;
            return tile;
        }

        private static Tile[] CreateTiles(Sprite[] sprites)
        {
            var tiles = new Tile[sprites.Length];
            for (int index = 0; index < sprites.Length; index++)
                tiles[index] = CreateTile(sprites[index]);
            return tiles;
        }

        private static void DestroyTiles(Tile[] tiles)
        {
            for (int index = 0; index < tiles.Length; index++)
                Destroy(tiles[index]);
        }

        private void CreateTiles()
        {
            floorTiles = CreateTiles(floorSprites);
            speckledFloorTiles = CreateTiles(speckledFloorSprites);
            wallTile = CreateTile(wallSprite);
        }

        private void CreateTilemap(Material material)
        {
            var gridObject = new GameObject("Floor", typeof(Grid));
            gridObject.transform.SetParent(transform, false);
            var tilemapObject = new GameObject("Tiles", typeof(Tilemap), typeof(TilemapRenderer));
            tilemapObject.transform.SetParent(gridObject.transform, false);
            tilemap = tilemapObject.GetComponent<Tilemap>();
            TilemapRenderer tilemapRenderer = tilemapObject.GetComponent<TilemapRenderer>();
            tilemapRenderer.sharedMaterial = material;
            tilemapRenderer.sortingOrder = SortingOrder;
        }

        private void Repaint()
        {
            shownVersion = layout.Version;
            MarkCellsUnderBoxes();
            WarehouseGrid grid = layout.Grid;
            for (int cellIndex = 0; cellIndex < grid.CellCount; cellIndex++)
            {
                GridPosition cell = grid.PositionOf(cellIndex);
                tilemap.SetTile(new Vector3Int(cell.X, cell.Y, 0), TileOf(cell, cellIndex));
            }

            PaintWallBehindTruck();
        }

        private void MarkCellsUnderBoxes()
        {
            System.Array.Clear(isUnderBox, 0, isUnderBox.Length);
            for (int index = 0; index < layout.Boxes.Count; index++)
            {
                Box box = layout.Boxes[index];
                if (!box.IsEmpty)
                    MarkCellsUnder(box);
            }
        }

        private void MarkCellsUnder(Box box)
        {
            for (int y = box.Origin.Y; y < box.Origin.Y + box.Size.Height; y++)
                for (int x = box.Origin.X; x < box.Origin.X + box.Size.Width; x++)
                    isUnderBox[layout.Grid.IndexOf(new GridPosition(x, y))] = true;
        }

        private void PaintWallBehindTruck()
        {
            GridPosition origin = layout.TruckOrigin;
            for (int y = origin.Y; y < origin.Y + layout.TruckSize.Height; y++)
                for (int x = origin.X; x < origin.X + layout.TruckSize.Width; x++)
                    tilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
        }

        private Tile TileOf(GridPosition cell, int cellIndex)
        {
            bool isWall = layout.Grid.GetCellType(cell) == CellType.Blocked && !isUnderBox[cellIndex];
            return isWall ? wallTile : FloorTileOf(cell);
        }

        private Tile FloorTileOf(GridPosition cell)
        {
            int colour = (cell.X + cell.Y) % floorTiles.Length;
            bool hasSpeck = (cell.X * SpeckStepX + cell.Y * SpeckStepY) % SpeckEvery == 0;
            return hasSpeck ? speckledFloorTiles[colour] : floorTiles[colour];
        }
    }
}
