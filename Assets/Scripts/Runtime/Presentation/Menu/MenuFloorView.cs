using UnityEngine;
using UnityEngine.Tilemaps;

namespace CvWarehouse.Presentation.Menu
{
    public sealed class MenuFloorView : MonoBehaviour
    {
        private const int SortingOrder = 0;
        private const int SpeckEvery = 5;
        private const int SpeckStepX = 7;
        private const int SpeckStepY = 13;

        [Header("Floor, one sprite for each colour of the checkerboard")]
        [SerializeField] private Sprite[] floorSprites;
        [SerializeField] private Sprite[] speckledFloorSprites;
        [SerializeField] private Material material;

        [Header("Size, in cells")]
        [SerializeField] private Vector2Int size = new Vector2Int(40, 16);

        private Tile[] floorTiles;
        private Tile[] speckledFloorTiles;

        public Vector2 Size => size;

        private void Awake()
        {
            floorTiles = CreateTiles(floorSprites);
            speckledFloorTiles = CreateTiles(speckledFloorSprites);
            Paint(CreateTilemap());
        }

        private void OnDestroy()
        {
            if (floorTiles == null)
                return;

            DestroyTiles(floorTiles);
            DestroyTiles(speckledFloorTiles);
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

        private Tilemap CreateTilemap()
        {
            var gridObject = new GameObject("Floor", typeof(Grid));
            gridObject.transform.SetParent(transform, false);
            var tilemapObject = new GameObject("Tiles", typeof(Tilemap), typeof(TilemapRenderer));
            tilemapObject.transform.SetParent(gridObject.transform, false);
            TilemapRenderer tilemapRenderer = tilemapObject.GetComponent<TilemapRenderer>();
            tilemapRenderer.sharedMaterial = material;
            tilemapRenderer.sortingOrder = SortingOrder;
            return tilemapObject.GetComponent<Tilemap>();
        }

        private void Paint(Tilemap tilemap)
        {
            for (int y = 0; y < size.y; y++)
                for (int x = 0; x < size.x; x++)
                    tilemap.SetTile(new Vector3Int(x, y, 0), FloorTileAt(x, y));
        }

        private Tile FloorTileAt(int x, int y)
        {
            int colour = (x + y) % floorTiles.Length;
            bool hasSpeck = (x * SpeckStepX + y * SpeckStepY) % SpeckEvery == 0;
            return hasSpeck ? speckledFloorTiles[colour] : floorTiles[colour];
        }
    }
}
