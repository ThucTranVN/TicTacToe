
public class SetTileCommand : ICommand
{
    private readonly Tile tile;
    private readonly TileState previousState;
    private readonly TileState newState;


    public SetTileCommand(Tile tile, TileState newState)
    {
        this.tile = tile;
        this.newState = newState;
        this.previousState = TileState.Unknown;
    }

    public void Execute()
    {
        tile.SetState(newState);
    }

    public void Undo()
    {
        tile.SetState(previousState);
    }
}
