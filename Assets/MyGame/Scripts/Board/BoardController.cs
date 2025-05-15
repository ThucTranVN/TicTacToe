using System.Collections.Generic;
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
    private Player currentPlayer = Player.PlayerA;
    private bool isGameOver = false;

    private List<Tile> winningTiles = new();

    private void Start()
    {
        if (DataManager.HasInstance)
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
                tile.SetState(TileState.Unknown);
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

    public void OnTileClicked(Tile tile)
    {
        if (isGameOver || tile.state != TileState.Unknown)
            return;

        TileState playerState = (currentPlayer == Player.PlayerA) ? TileState.O : TileState.X;
        tile.SetState(playerState);

        if (CheckWin(tile, playerState))
        {
            Debug.Log($"{currentPlayer} WIN!");
            isGameOver = true;
            return;
        }

        currentPlayer = (currentPlayer == Player.PlayerA) ? Player.PlayerB : Player.PlayerA;
    }

    private bool CheckWin(Tile tile, TileState state)
    {
        winningTiles.Clear();

        return CheckDirection(tile, state, 1, 0) ||  
               CheckDirection(tile, state, 0, 1) ||  
               CheckDirection(tile, state, 1, 1) ||  
               CheckDirection(tile, state, 1, -1);   
    }

    private bool CheckDirection(Tile tile, TileState state, int dx, int dy)
    {
        List<Tile> matched = new() { tile };

        matched.AddRange(CollectTiles(tile, state, dx, dy));
        matched.AddRange(CollectTiles(tile, state, -dx, -dy));

        int winLength = GetWinLengthFromBoardSize(width);
        if (matched.Count >= winLength)
        {
            winningTiles = matched;
            return true;
        }

        return false;
    }

    private List<Tile> CollectTiles(Tile tile, TileState state, int dx, int dy)
    {
        List<Tile> result = new();
        int x = tile.xIndex + dx;
        int y = tile.yIndex + dy;

        while (IsInBounds(x, y) && tiles[x, y].state == state)
        {
            result.Add(tiles[x, y]);
            x += dx;
            y += dy;
        }

        return result;
    }

    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    private int GetWinLengthFromBoardSize(int size)
    {
        return size switch
        {
            3 => 3,
            6 => 5,
            9 => 5,
            11 => 5,
            _ => 3
        };
    }
}
