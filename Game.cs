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
            Sprite = new Sprite(WidthWindow, HeightWindow);
            Maze = new Maze(Sprite);
            Player = new Player(100, Sprite);
            Ink = new Ghost(100, Color.SkyBlue, 14,11, Sprite);
        }
        public int HeightWindow { get; private set; }
        public int WidthWindow { get; private set; }
        public string NameWindow { get; private set; } = string.Empty;
        public int FPS { get; set; }
        public float DeltaTime { get; set; } = 0;
        public Sprite Sprite { get; set; }
        public Maze Maze { get; set; }
        public Player Player { get; set; }
        public Ghost Ink { get; set; }

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
            Raylib.DrawText("Puntuacion: " + Player.Points.ToString(), 0, 0, 24, Color.White);
            Maze.Draw();
            Player.Draw();
            Ink.Draw();
            Raylib.EndDrawing();
        }

        public void Update()
        {
            Player.SetDeltaTime(this.DeltaTime);
            MovementPlayer();
            Ink.SetDeltaTime(this.DeltaTime);
            MoveInky();
        }

        public void HandleInput()
        {
            Player.HandleInput();
        }

        public void MovementPlayer()
        {
            Player.Move(Maze);
            Player.UpdateColRow();
            Player.UpdateDirection(Maze);
            Player.UpdatePoints(Maze);
        }

        public void MoveInky()
        {
            Ink.Move(Maze);
            Ink.UpdateColRow();
            Ink.UpdateDirection(Maze);
        }
    }
}
