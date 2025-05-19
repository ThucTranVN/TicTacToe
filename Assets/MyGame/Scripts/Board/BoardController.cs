using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardController : BaseManager<BoardController>
{
    public GameObject winLinePrefab;
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

        TileState playerState = currentPlayer == Player.PlayerA ? TileState.O : TileState.X;
        tile.SetState(playerState);

        int winCount = CheckWin(tile, playerState);

        if (winCount > 0)
        {
            Debug.Log($"{currentPlayer} WIN with {winCount} line(s)!");
            isGameOver = true;
            return;
        }
        if (IsBoardFull())
        {
            Debug.Log("DRAW! No more moves.");
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

    private bool IsBoardFull()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tiles[x, y].state == TileState.Unknown)
                    return false;
            }
        }
        return true;
    }

    private int CheckWin(Tile tile, TileState state)
    {
        winningTiles.Clear();

        CheckWinDirection[] directions = (CheckWinDirection[])System.Enum.GetValues(typeof(CheckWinDirection));
        int totalWinCount = 0;

        for (int i = 0; i < directions.Length; i++)
        {
            List<Tile> matched;
            int winCount = CheckDirection(tile, state, directions[i], out matched);

            if (winCount > 0)
            {
                totalWinCount += winCount;

                for (int j = 0; j < matched.Count; j++)
                {
                    winningTiles.Add(matched[j]);
                }

                DrawWinLine(matched);
            }
        }

        return totalWinCount;
    }

    private int CheckDirection(Tile tile, TileState state, CheckWinDirection direction, out List<Tile> matchedTiles)
    {
        matchedTiles = new List<Tile>();
        var (dx, dy) = GetDirectionOffset(direction);

        List<Tile> matched = new() { tile };
        matched.AddRange(CollectTiles(tile, state, dx, dy));
        matched.AddRange(CollectTiles(tile, state, -dx, -dy));

        int nearCount = matched.Count - 1;
        int winLength = GetWinLengthFromBoardSize(width);

        if (nearCount >= winLength - 1)
        {
            matchedTiles = matched;
            return 1;
        }

        return 0;
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

    private void DrawWinLine(List<Tile> matched)
    {
        if (matched == null || matched.Count < 2 || winLinePrefab == null)
            return;

        if (DataManager.HasInstance)
        {
            matched.Sort((a, b) =>
            {
                if (a.xIndex == b.xIndex)
                    return a.yIndex.CompareTo(b.yIndex);
                return a.xIndex.CompareTo(b.xIndex);
            });

            Vector3 start = matched[0].transform.position;
            Vector3 end = matched[matched.Count - 1].transform.position;

            GameObject lineObject = Instantiate(winLinePrefab, Vector3.zero, Quaternion.identity, this.transform);
            lineObject.transform.SetAsLastSibling();
            LineRenderer lr = lineObject.GetComponent<LineRenderer>();
            lr.startColor = DataManager.Instance.GlobalConfig.winLineColor;
            lr.endColor = DataManager.Instance.GlobalConfig.winLineColor;
            lr.startWidth = DataManager.Instance.GlobalConfig.winLineWidth;
            lr.endWidth = DataManager.Instance.GlobalConfig.winLineWidth;
            lr.sortingLayerName = "UI";  
            lr.sortingOrder = 10;        
            lr.positionCount = 2;
            lr.SetPosition(0, start);
            lr.SetPosition(1, start);
            StartCoroutine(AnimateLine(lr, start, end, 0.5f));
        }
    }


    private IEnumerator AnimateLine(LineRenderer lr, Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector3 current = Vector3.Lerp(start, end, t);
            lr.SetPosition(1, current);
            yield return null;
        }

        lr.SetPosition(1, end);
    }
}