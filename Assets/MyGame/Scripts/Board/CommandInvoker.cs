using System;
using System.Collections.Generic;

public class CommandInvoker
{
    private readonly Stack<Tile> commandHistory = new();
    public Action OnComplete;

    public void ExecuteCommand(Tile tile, TileState newState)
    {
        if (tile.state != TileState.Unknown)
        {
            // If the tile is already set, we cannot change it
            return;
        }
        // Save the current state before changing it
        commandHistory.Push(tile);
        tile.SetState(newState);
    }

    public void UndoLastCommand()
    {
        if (commandHistory.Count > 0)
        {
            Tile lastTile = commandHistory.Pop();
            lastTile.SetState(TileState.Unknown); // Reset the tile state to Unknown
        }
    }
    public void UndoLastTwoMoves()
    {
        UndoLastCommand();// Undo AI move
        UndoLastCommand();// Undo PLayer move
        OnComplete?.Invoke();
    }
}
