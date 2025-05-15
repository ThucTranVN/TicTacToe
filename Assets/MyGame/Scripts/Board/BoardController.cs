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
        float verticalSize = (float)height / 2f + (float)borderSize;
        float horizontalSize = ((float)width / 2f + (float)borderSize) / aspectRatio;
        Camera.main.orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;
    }

    public void OnTileClicked(Tile tile)
    {
        if (isGameOver || tile.state != TileState.Unknown)
            return;

        TileState playerState;

        if (currentPlayer == Player.PlayerA)
        {
            playerState = TileState.O;
        }
        else
        {
            playerState = TileState.X;
        }
        tile.SetState(playerState);

        if (CheckWin(tile, playerState))
        {
            Debug.Log($"{currentPlayer} WIN!");
            isGameOver = true;
            return;
        }
        if (currentPlayer == Player.PlayerA)
        {
            currentPlayer = Player.PlayerB;
        }
        else
        {
            currentPlayer = Player.PlayerA;
        }
    }

    private bool CheckWin(Tile tile, TileState state)
    {
        winningTiles.Clear();

        CheckWinDirection[] directions = (CheckWinDirection[])System.Enum.GetValues(typeof(CheckWinDirection));
        for (int i = 0; i < directions.Length; i++)
        {
            if (CheckDirection(tile, state, directions[i]))
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckDirection(Tile tile, TileState state, CheckWinDirection direction)
    {
        var (dx, dy) = GetDirectionOffset(direction);

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

    private (int dx, int dy) GetDirectionOffset(CheckWinDirection direction)
    {
        return direction switch
        {
            CheckWinDirection.Horizontal => (1, 0),
            CheckWinDirection.Vertical => (0, 1),
            CheckWinDirection.DiagonalRight => (1, 1),
            CheckWinDirection.DiagonalLeft => (1, -1),
            _ => (0, 0)
        };
    }
}
