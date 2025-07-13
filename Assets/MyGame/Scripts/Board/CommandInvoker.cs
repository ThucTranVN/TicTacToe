using System;
using System.Collections.Generic;

public class CommandInvoker
{
    private readonly Stack<ICommand> commandHistory = new();

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        commandHistory.Push(command);
    }

    public void UndoLastCommand(int amountUndoMove, Action onComplete = null)
    {
        if (commandHistory.Count >= amountUndoMove)
        {
            for (int i = 0; i < amountUndoMove; i++)
            {
                ICommand lastCommand = commandHistory.Pop();
                lastCommand.Undo();
            }
            onComplete?.Invoke();
        }
    }
}
