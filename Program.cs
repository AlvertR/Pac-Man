using Pac_Man;

public class Program
{
    public Program() { }

    public static void Main(string[] args)
    {
        Game game = new Game(400, 400, "Pac-Man", 60);
        game.Run();
    }
}