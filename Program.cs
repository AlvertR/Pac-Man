using Pac_Man;

public class Program
{
    public Program() { }

    public static void Main(string[] args)
    {
        Game game = new Game(600, 800, "Pac-Man", 60);
        game.Run();
    }
}