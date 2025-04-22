using UnityEngine;

public class BoardController : BaseManager<BoardController>
{
    public GameObject tilePrefab;
    public int borderSize;
    private BoardType defaultBoardType;
    private int width;
    private int height;
    private Tile[,] tiles;
    private const string PREFAB_TILE_PATH = "Prefabs/Tile/TilePrefab";

    private void Start()
    {
        if(DataManager.HasInstance)
        {
            defaultBoardType = DataManager.Instance.GetCurrentBoardType();
        }    
       
    }
    public void InitBoard(BoardType boardType)
    {
        if (boardType == BoardType.Unknown)
        {
            boardType = defaultBoardType;
        }
        SetupBoardType(boardType);
        SetupCameraPosition();
    }

    private void SetupBoardType(BoardType boardType)
    {
        width = height = (int)boardType;
        SetupTile(width, height);
    }

    private void SetupTile(int width, int height)
    {
        if (tilePrefab == null)
        {
            tilePrefab = Resources.Load<GameObject>(PREFAB_TILE_PATH);
        }
        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tileObject = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity, this.transform);
                tileObject.name = $"Tile({x},{y})";
                Tile tile = tileObject.GetOrAddComponent<Tile>();
                tile.xIndex = x;
                tile.yIndex = y;
                tiles[x, y] = tile;
            }
        }
    }
    private void SetupCameraPosition()
    {
        Vector2 cameraPosition = new((float)(width - 1) / 2f, (float)(height - 1) / 2f);
        Camera.main.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, -10);
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float verticleSize = (float)height / 2f + (float)borderSize;
        float horizontalSize = ((float)width / 2f + (float)borderSize) / aspectRatio;
        Camera.main.orthographicSize = (verticleSize > horizontalSize) ? verticleSize : horizontalSize;
    }
}
