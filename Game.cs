using Raylib_cs;

namespace Pac_Man
{
    public class Game
    {
        public Game(int width, int height, string name, int fps)
        {
            WidthWindow = width;
            HeightWindow = height;
            NameWindow = name;
            FPS = fps;
            Board = new Board(WidthWindow, HeightWindow);
            Maze = new Maze(Board);
            Player = new Player(100, Board);
            Blinky = new Blinky(100, Board);
        }
        public int HeightWindow { get; private set; }
        public int WidthWindow { get; private set; }
        public string NameWindow { get; private set; } = string.Empty;
        public int FPS { get; set; }
        public float DeltaTime { get; set; } = 0;
        public Board Board { get; set; }
        public Maze Maze { get; set; }
        public Player Player { get; set; }
        public Blinky Blinky { get; set; }

        public void Run()
        {
            Raylib.InitWindow(this.WidthWindow, this.HeightWindow, this.NameWindow);

            string BasePath = AppDomain.CurrentDomain.BaseDirectory;
            string FulPathIcon = Path.Combine(BasePath, "Resources", "icon.png");
            Image Icon = Raylib.LoadImage(FulPathIcon);
            Raylib.ImageFormat(ref Icon, PixelFormat.UncompressedR8G8B8A8);
            Raylib.SetWindowIcon(Icon);
            Raylib.UnloadImage(Icon);

            Raylib.SetTargetFPS(this.FPS);

            while (!Raylib.WindowShouldClose())
            {
                this.DeltaTime = MathF.Min(Raylib.GetFrameTime(), 1f/30f);
                HandleInput();
                Update();
                Draw();
            }
            Raylib.CloseWindow();
        }

        public void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            Maze.Draw();
            Raylib.DrawText("Puntuacion: " + Player.Points.ToString(), 0, 0, 24, Color.White);
            Player.Draw();
            Blinky.Draw();
            Raylib.EndDrawing();
        }

        public void Update()
        {
            Player.SetDeltaTime(this.DeltaTime);
            MovementPlayer();
            Blinky.SetDeltaTime(this.DeltaTime);
            MoveBlinky();
        }

        public void HandleInput()
        {
            Player.HandleInput();
        }

        public void MovementPlayer()
        {
            Player.Move(Maze, false);
            Player.UpdateColRow();
            Player.UpdateDirection(Maze);
            Player.UpdatePoints(Maze);
        }

        public void MoveBlinky()
        {
            Blinky.Move(Maze, true);
            Blinky.UpdateColRow();
            Blinky.UpdateDirection(Maze);
        }
    }
}
