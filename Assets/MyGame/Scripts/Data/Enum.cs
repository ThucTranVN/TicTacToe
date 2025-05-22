public enum ListenType
{
    ANY = 0,
    CHANGED_PLATFORM
}

public enum UIType
{
    Unknown = 0,
    Screen = 1,
    Popup = 2,
    Notify = 3,
    Overlap = 4,
    Hud =5
}

public enum GamePlatform
{
    Unknown = 0,
    Windown,
    MacOS,
    Android,
    iOS,


}
public enum BoardType
{
    Unknown = 0,
    Size3x3 = 3,
    Size6x6 = 6,
    Size9x9 = 9,
    Size11x11 = 11,
}

public enum Direction
{
    Unknown = 0,
    Up,
    Down,
    Left,
    Right
}

public enum GameMode
{
    Unknown = 0,
    PVP,
    PVE
}

public enum TileState
{
    Unknown = 0,
    O,
    X
}

public enum Player
{
    PlayerA,  
    PlayerB   
}
public enum CheckWinDirection
{
    Horizontal,
    Vertical,
    DiagonalRight,
    DiagonalLeft
}