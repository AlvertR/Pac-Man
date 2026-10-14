namespace Pac_Man
{
    public enum CellType
    {
        Wall,
        Path,
        Pellet,
        PowerPellet,
        Door,
        Bridge,
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        None,
    }

    public enum GameStatus
    {
        Start,
        Playing,
        Paused,
        GameOver,
        End,
    }
}
