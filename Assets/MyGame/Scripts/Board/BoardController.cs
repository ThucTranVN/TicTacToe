using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BoardController : BaseManager<BoardController>
{
    public GameObject winLinePrefab;
    public GameObject tilePrefab;
    public int borderSize;

    private BoardType defaultBoardType;
    private BoardType currenttBoardType;
    private int width;
    private int height;
    private Tile[,] tiles;

    private const string PREFAB_TILE_PATH = "Prefabs/Tile/TilePrefab";
    private Player currentPlayer = Player.PlayerA;
    private bool isGameOver = false;

    private static readonly Vector2Int Horizontal = new Vector2Int(1, 0);
    private static readonly Vector2Int Vertical = new Vector2Int(0, 1);
    private static readonly Vector2Int DiagonalRight = new Vector2Int(1, 1);
    private static readonly Vector2Int DiagonalLeft = new Vector2Int(1, -1);

    private List<Tile> winningTiles = new();

    private int maxWinLines => System.Enum.GetValues(typeof(CheckWinDirection)).Length;
    private List<GameObject> winLineObjects = new();
    private int usedWinLineCount = 0;
    private int maxCandidateMoves = 10;



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
        else
        {
            currenttBoardType = boardType;
        }
        SetupBoardType(boardType);
        SetupCameraPosition();
        InitWinLines();
        ResetWinLines();
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

        SwitchPlayer();

        if (GameManager.HasInstance)
        {
            if (GameManager.Instance.CurrenGameMode == GameMode.PVE && currentPlayer == Player.PlayerB)
            {
                StartCoroutine(DelayedAIMove());
            }
        }

    }
    private void SwitchPlayer()
    {
        if (GameManager.HasInstance)
        {
            switch (GameManager.Instance.CurrenGameMode)
            {
                case GameMode.PVP:
                    if (currentPlayer == Player.PlayerA)
                    {
                        currentPlayer = Player.PlayerB;
                    }
                    else
                    {
                        currentPlayer = Player.PlayerA;
                    }
                    break;

                case GameMode.PVE:
                    currentPlayer = (currentPlayer == Player.PlayerA) ? Player.PlayerB : Player.PlayerA;
                    break;

                case GameMode.Unknown:
                default:
                    Debug.LogWarning("Chế độ chơi chưa xác định. Không thể chuyển lượt.");
                    break;
            }
        }

    }

    private bool IsBoardFull()
    {
        foreach (Tile tile in tiles)
        {
            if (tile.state == TileState.Unknown) return false;
        }
        return true;
    }

    private int CheckWin(Tile tile, TileState state)
    {
        //winningTiles.Clear(); // to do: move cai logic nay khi hien popup thang thua
        CheckWinDirection[] directions = (CheckWinDirection[])System.Enum.GetValues(typeof(CheckWinDirection));
        int totalWinCount = 0;

        for (int i = 0; i < directions.Length; i++)
        {
            List<Tile> matched = new();
            int winCount = CheckDirection(tile, state, directions[i], out matched);

            if (winCount > 0)
            {
                totalWinCount += winCount;

                for (int j = 0; j < matched.Count; j++)
                {
                    if (!winningTiles.Contains(matched[j]))
                    {
                        winningTiles.Add(matched[j]);
                    }
                }

                DrawWinLine(matched);
            }
        }

        return totalWinCount;
    }

    private int CheckDirection(Tile tile, TileState state, CheckWinDirection direction, out List<Tile> matchedTiles)
    {
        matchedTiles = new List<Tile>();
        Vector2Int dir = GetDirectionOffset(direction);

        List<Tile> matched = new() { tile };
        matched.AddRange(CollectTiles(tile, state, dir));
        matched.AddRange(CollectTiles(tile, state, -dir));

        int nearCount = matched.Count - 1;
        int winLength = GetWinLengthFromBoardSize(width);

        if (nearCount >= winLength - 1)
        {
            matchedTiles = matched;
            return 1;
        }

        return 0;
    }

    private List<Tile> CollectTiles(Tile start, TileState state, Vector2Int dir)
    {
        List<Tile> result = new List<Tile>();
        int x = start.xIndex + dir.x;
        int y = start.yIndex + dir.y;
        while (true)
        {
            if (!IsInBounds(x, y) || tiles[x, y].state != state)
            {
                break;
            }
            result.Add(tiles[x, y]);
            x += dir.x;
            y += dir.y;
        }

        return result;
    }

    public bool IsInBounds(int x, int y)
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

    public Vector2Int GetDirectionOffset(CheckWinDirection direction)
    {
        return direction switch
        {
            CheckWinDirection.Horizontal => Horizontal,
            CheckWinDirection.Vertical => Vertical,
            CheckWinDirection.DiagonalRight => DiagonalRight,
            CheckWinDirection.DiagonalLeft => DiagonalLeft,
            _ => Vector2Int.zero
        };
    }

    private void InitWinLines()
    {
        if (winLinePrefab == null) return;

        for (int i = 0; i < maxWinLines; i++)
        {
            GameObject lineObject = Instantiate(winLinePrefab, Vector3.zero, Quaternion.identity, this.transform);
            lineObject.SetActive(false);
            winLineObjects.Add(lineObject);
        }
    }

    private void ResetWinLines()
    {
        usedWinLineCount = 0;
        foreach (var obj in winLineObjects)
        {
            obj.SetActive(false);
        }
    }

    private void DrawWinLine(List<Tile> matched)
    {
        if (matched == null || matched.Count < 2 || winLineObjects.Count == 0 || usedWinLineCount >= maxWinLines)
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

            GameObject lineObject = winLineObjects[usedWinLineCount++];
            lineObject.SetActive(true);
            lineObject.transform.SetAsLastSibling();

            LineRenderer lr = lineObject.GetComponent<LineRenderer>();
            lr.startColor = DataManager.Instance.GlobalConfig.winLineColor;
            lr.endColor = DataManager.Instance.GlobalConfig.winLineColor;
            lr.startWidth = DataManager.Instance.GlobalConfig.winLineWidth;
            lr.endWidth = DataManager.Instance.GlobalConfig.winLineWidth;
            lr.sortingLayerName = DataManager.Instance.GlobalConfig.winLineSortingLayerName;
            lr.sortingOrder = DataManager.Instance.GlobalConfig.winLineSortingOrder;
            lr.positionCount = DataManager.Instance.GlobalConfig.winLinePositionCount;
            lr.SetPosition(0, start);
            lr.SetPosition(1, start);

            StartCoroutine(AnimateLine(lr, start, end, DataManager.Instance.GlobalConfig.winLineDrawDuration));
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
    private void AIMove()
    {
        // Get WinLength from Width
        int winLength = GetWinLengthFromBoardSize(width);

        // 1. Init Board
        BoardState boardState = new(width, height);
        boardState.LoadFrom(tiles);


        if (GameManager.HasInstance)
        {
            if(DataManager.HasInstance)
            {
                int maxDepth = (int)GameManager.Instance.AiDifficulty; //GameManager.Instance.Difficult;
                                                                       // 2. Init AI
                MinimaxAI ai = new(TileState.X, maxDepth, winLength, DataManager.Instance.GlobalConfig);
                // 3. Find Move
                Vector2Int move = ai.FindBestMove(boardState, currenttBoardType);
                Debug.Log("Số node Minimax đã duyệt: " + ai.NodeCount);
                // 4. Take that move
                if (IsInBounds(move.x, move.y) && tiles[move.x, move.y].state == TileState.Unknown)
                {
                    OnTileClicked(tiles[move.x, move.y]);
                }
            }
           
        }
    }

    private IEnumerator DelayedAIMove()
    {
        yield return new WaitForSeconds(0.25f); // Cho có độ trễ nhẹ
        AIMove();
    }

}