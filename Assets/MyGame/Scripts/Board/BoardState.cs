using System.Collections.Generic;
using UnityEngine;

public class BoardState
{
    private TileState[,] board;
    private int width;
    private int height;
    private List<Vector2Int> moveHistory = new();

    public BoardState(int width, int height)
    {
        this.width = width;
        this.height = height;
        board = new TileState[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                board[x, y] = TileState.Unknown;
    }

    public TileState[,] GetBoard() => board;

    public List<Vector2Int> GetAvailableMoves()
    {
        List<Vector2Int> moves = new();
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (board[x, y] == TileState.Unknown)
                    moves.Add(new Vector2Int(x, y));
        return moves;
    }

    public List<Vector2Int> GetRecentMoves(int count)
    {
        int take = Mathf.Min(count, moveHistory.Count);
        return moveHistory.GetRange(moveHistory.Count - take, take);
    }
    public void MakeMove(int x, int y, TileState player)
    {
        board[x, y] = player;
        moveHistory.Add(new Vector2Int(x, y));
    }

    public void UndoMove(int x, int y)
    {
        board[x, y] = TileState.Unknown;
        if (moveHistory.Count > 0 && moveHistory[^1].x == x && moveHistory[^1].y == y) // ^1 => Get last element index
            moveHistory.RemoveAt(moveHistory.Count - 1);
    }

    public bool IsFull()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (board[x, y] == TileState.Unknown)
                    return false;
        return true;
    }

    public TileState CheckWinner(int winLength)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int(1, 0),   // Horizontal
        new Vector2Int(0, 1),   // Vertical
        new Vector2Int(1, 1),   // Diagonal right
        new Vector2Int(1, -1),  // Diagonal left
        };

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileState current = board[x, y];
                if (current == TileState.Unknown)
                    continue;

                foreach (var dir in directions)
                {
                    int count = 1;
                    int nx = x + dir.x, ny = y + dir.y;

                    while (IsInBounds(nx, ny) && board[nx, ny] == current)
                    {
                        count++;
                        if (count >= winLength)
                            return current;

                        nx += dir.x;
                        ny += dir.y;
                    }
                }
            }
        }

        return TileState.Unknown;
    }
    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
    public void LoadFrom(Tile[,] tiles)
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                board[x, y] = tiles[x, y].state;
    }

}