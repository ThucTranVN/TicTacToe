using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Linq;

public class MinimaxAI
{
    private TileState aiPlayer;
    private TileState opponent;
    private int maxDepth;
    private int winLength;
    private GlobalConfig globalConfig;
    private int nodeCount = 0;
    public int NodeCount => nodeCount;

    public MinimaxAI(TileState aiPlayer, int maxDepth, int winLength, GlobalConfig globalConfig)
    {
        this.aiPlayer = aiPlayer;
        this.opponent = aiPlayer == TileState.X ? TileState.O : TileState.X;
        this.maxDepth = maxDepth;
        this.winLength = winLength;
        this.globalConfig = globalConfig;
    }

    public Vector2Int FindBestMove(BoardState board, BoardType boardType)
    {
        nodeCount = 0;
        Stopwatch stopwatch = new();
        stopwatch.Start();

        if (DataManager.HasInstance)
            globalConfig = DataManager.Instance.GlobalConfig;

        var availableMoves = board.GetAvailableMoves();
        int width = board.GetBoard().GetLength(0);
        int height = board.GetBoard().GetLength(1);

        // 1. Ngay lập tức kiểm tra nước thắng của AI
        foreach (var move in availableMoves)
        {
            board.MakeMove(move.x, move.y, aiPlayer);
            bool isWin = board.CheckWinner(winLength) == aiPlayer;
            board.UndoMove(move.x, move.y);
            if (isWin)
            {
                stopwatch.Stop();
                UnityEngine.Debug.Log($"⏱️ Time: {stopwatch.ElapsedMilliseconds} ms | Nodes: {nodeCount} (Immediate Win)");
                return move;
            }
        }

        // 2. Kiểm tra threat của đối thủ để chặn
        var threats = GetBlockingMoves(board, opponent, winLength);
        if (threats.Count > 0)
        {
            var blockMove = threats.OrderBy(m => GetDistanceToCenter(m.x, m.y, width, height)).First();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"⏱️ Time: {stopwatch.ElapsedMilliseconds} ms | Nodes: {nodeCount} (Blocking Move)");
            return blockMove;
        }

        // 3. Nếu lượt đầu (không phải 3x3) → chọn gần trung tâm
        if (boardType != BoardType.Size3x3 && availableMoves.Count >= board.GetBoard().Length - globalConfig.amountMove)
        {
            int centerX = width / 2;
            int centerY = height / 2;
            var goodStarts = new List<Vector2Int>
            {
                new(centerX, centerY),
                new(centerX - 1, centerY),
                new(centerX, centerY - 1),
                new(centerX + 1, centerY),
                new(centerX, centerY + 1),
            };
            foreach (var move in goodStarts)
            {
                if (board.IsInBounds(move.x, move.y) && board.GetBoard()[move.x, move.y] == TileState.Unknown)
                {
                    stopwatch.Stop();
                    UnityEngine.Debug.Log($"⏱️ Time: {stopwatch.ElapsedMilliseconds} ms | Nodes: {nodeCount} (First Move)");
                    return move;
                }
            }
            stopwatch.Stop();
            return availableMoves[Random.Range(0, availableMoves.Count)];
        }

        // 4. Minimax với alpha-beta
        int bestScore = int.MinValue;
        Vector2Int bestMove = new Vector2Int(-1, -1);
        var moves = GetCandidateMoves(board, globalConfig.radiusMove, globalConfig.maxCandidates);

        foreach (var move in moves)
        {
            board.MakeMove(move.x, move.y, aiPlayer);
            int score = Minimax(board, 0, false, int.MinValue, int.MaxValue);
            board.UndoMove(move.x, move.y);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log($"⏱️ Time: {stopwatch.ElapsedMilliseconds} ms | Nodes: {nodeCount}");
        return bestMove;
    }

    private int Minimax(BoardState board, int depth, bool isMaximizing, int alpha, int beta)
    {
        nodeCount++;
        TileState winner = board.CheckWinner(winLength);
        if (depth >= maxDepth || winner != TileState.Unknown || board.IsFull())
        {
            if (winner == aiPlayer) return 100000 - depth;
            if (winner == opponent) return depth - 100000;
            return EvaluateBoard(board);
        }

        var moves = GetCandidateMoves(board, globalConfig.radiusMove, globalConfig.maxCandidates);
        if (isMaximizing)
        {
            int maxEval = int.MinValue;
            foreach (var move in moves)
            {
                board.MakeMove(move.x, move.y, aiPlayer);
                int eval = Minimax(board, depth + 1, false, alpha, beta);
                board.UndoMove(move.x, move.y);
                maxEval = Mathf.Max(maxEval, eval);
                alpha = Mathf.Max(alpha, eval);
                if (beta <= alpha) break;
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (var move in moves)
            {
                board.MakeMove(move.x, move.y, opponent);
                int eval = Minimax(board, depth + 1, true, alpha, beta);
                board.UndoMove(move.x, move.y);
                minEval = Mathf.Min(minEval, eval);
                beta = Mathf.Min(beta, eval);
                if (beta <= alpha) break;
            }
            return minEval;
        }
    }

    private int EvaluateBoard(BoardState board)
    {
        return EvaluateForPlayer(board, aiPlayer) - EvaluateForPlayer(board, opponent);
    }

    private int EvaluateForPlayer(BoardState board, TileState player)
    {
        int score = 0;
        int width = board.GetBoard().GetLength(0);
        int height = board.GetBoard().GetLength(1);

        Vector2Int[] directions = ArrayDirection();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                foreach (var dir in directions)
                {
                    int count = 0;
                    int blanks = 0;
                    for (int i = 0; i < winLength; i++)
                    {
                        int nx = x + dir.x * i;
                        int ny = y + dir.y * i;
                        if (!board.IsInBounds(nx, ny)) break;

                        TileState s = board.GetBoard()[nx, ny];
                        if (s == player) count++;
                        else if (s == TileState.Unknown) blanks++;
                        else { count = -1; break; }
                    }

                    if (count > 0)
                    {
                        if (count == winLength) score += 10000;
                        else if (count == winLength - 1 && blanks > 0) score += 1000;
                        else if (count == winLength - 2 && blanks > 0) score += 100;
                        else score += count * 10;
                    }
                }
            }
        }

        return score;
    }


    private int GetDistanceToCenter(int x, int y, int width, int height)
    {
        int centerX = width / 2;
        int centerY = height / 2;
        return Mathf.Abs(x - centerX) + Mathf.Abs(y - centerY);
    }


    private List<Vector2Int> GetCandidateMoves(BoardState board, int radius, int maxCandidates)
    {
        HashSet<Vector2Int> candidates = new();
        int width = board.GetBoard().GetLength(0);
        int height = board.GetBoard().GetLength(1);

        // Thêm blocking moves ngay đầu
        var blockers = GetBlockingMoves(board, opponent, winLength);
        foreach (var b in blockers)
            candidates.Add(b);

        // Lấy quanh các nước recent
        var focusPoints = board.GetRecentMoves(globalConfig.nearbyMove);
        foreach (var pt in focusPoints)
            for (int dx = -radius; dx <= radius; dx++)
                for (int dy = -radius; dy <= radius; dy++)
                {
                    int nx = pt.x + dx;
                    int ny = pt.y + dy;
                    if ((dx != 0 || dy != 0) && board.IsInBounds(nx, ny) && board.GetBoard()[nx, ny] == TileState.Unknown)
                        candidates.Add(new Vector2Int(nx, ny));
                }

        if (candidates.Count == 0)
            candidates.UnionWith(board.GetAvailableMoves());

        return candidates
            .OrderBy(m => GetDistanceToCenter(m.x, m.y, width, height))
            .Take(maxCandidates)
            .ToList();
    }

    private List<Vector2Int> GetBlockingMoves(BoardState board, TileState threatPlayer, int winLength)
    {
        List<Vector2Int> blockers = new();
        foreach (var move in board.GetAvailableMoves())
        {
            board.MakeMove(move.x, move.y, threatPlayer);
            int maxLen = GetMaxConnectedLength(board, move, threatPlayer);
            board.UndoMove(move.x, move.y);
            if (maxLen >= winLength - 1)
                blockers.Add(move);
        }
        return blockers;
    }

    private int GetMaxConnectedLength(BoardState board, Vector2Int pos, TileState player)
    {
        int maxLength = 1;
        Vector2Int[] directions = ArrayDirection();


        foreach (var dir in directions)
        {
            int count = 1;
            int x = pos.x + dir.x, y = pos.y + dir.y;
            while (board.IsInBounds(x, y) && board.GetBoard()[x, y] == player)
            {
                count++; x += dir.x; y += dir.y;
            }
            x = pos.x - dir.x; y = pos.y - dir.y;
            while (board.IsInBounds(x, y) && board.GetBoard()[x, y] == player)
            {
                count++; x -= dir.x; y -= dir.y;
            }
            maxLength = Mathf.Max(maxLength, count);
        }
        return maxLength;
    }

    private Vector2Int[] ArrayDirection()
    {
        return new Vector2Int[]
        {
            new(1, 0), new(0, 1),
            new(1, 1), new(1, -1),
        };
    }
}
