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
            Player = new Player('p', 100, Board);
            Blinky = new Ghost('b', 100, Board, Color.Red, Direction.Left, 0f, true, false);
            Inky = new Ghost('i', 100, Board, Color.SkyBlue, Direction.Right, 10f);
        }
        public int HeightWindow { get; private set; }
        public int WidthWindow { get; private set; }
        public string NameWindow { get; private set; } = string.Empty;
        public int FPS { get; set; }
        public float DeltaTime { get; set; } = 0;
        public GameStatus Status { get; set; } = GameStatus.Start;
        public Board Board { get; set; }
        public Maze Maze { get; set; }
        public Player Player { get; set; }
        public Ghost Blinky { get; set; }
        public Ghost Inky { get; set; }

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
            switch (Status) {
                case GameStatus.Start:
                    Raylib.DrawText("Pack-Man", 0, 0, 24, Color.White);
                    Raylib.DrawText("Presion S para comenzar", 0, 30, 24, Color.White);
                    break;
                case GameStatus.Paused:
                case GameStatus.Playing:
                    Maze.Draw();
                    Raylib.DrawText("Puntuacion: " + Player.Points.ToString(), 0, 0, 24, Color.White);
                    Player.Draw();
                    Blinky.Draw();
                    Inky.Draw();
                    if(Status == GameStatus.Paused)
                        Raylib.DrawText("Pausa", 300, 400, 40, Color.White);
                    break;
                case GameStatus.GameOver: 
                    break;
                case GameStatus.End:
                    break;
            }
            Raylib.EndDrawing();
        }

        public void Update()
        {
            switch (Status) {
                case GameStatus.Playing:
                    MovementPlayer();
                    Blinky.BaseMove(DeltaTime, Maze, MoveBlinky);
                    Inky.BaseMove(DeltaTime, Maze, MoveInky);
                    break;
                case GameStatus.Start:
                case GameStatus.Paused:
                case GameStatus.GameOver:
                case GameStatus.End:
                default:
                    break;
        }
        }

        public void HandleInput()
        {
            switch (Status) {
                case GameStatus.Start:
                    if(Raylib.IsKeyPressed(KeyboardKey.S))
                        Status = GameStatus.Playing;
                    break;
                case GameStatus.Playing:
                    Player.HandleInput();
                    if (Raylib.IsKeyPressed(KeyboardKey.P))
                        Status = GameStatus.Paused;
                    break;
                case GameStatus.Paused:
                    if (Raylib.IsKeyPressed(KeyboardKey.P))
                        Status = GameStatus.Playing;
                    break;
                case GameStatus.GameOver:
                case GameStatus.End:
                default: 
                    break;
            }
        }

        public void MovementPlayer()
        {
            Player.Move(Maze, false, this.DeltaTime);
            Player.UpdateColRow();
            Player.UpdateDirection(Maze, this.DeltaTime);
            Player.UpdatePoints(Maze);
        }

        public void MoveBlinky()
        {
            if (!Blinky.Check(Direction.Left, Maze, true))
            {
                Blinky.SetPositionY(Board.OffsetY + Blinky.MazeRow * Board.Pixel);
                Blinky.Direction = Direction.Right;
            }
            if (!Blinky.Check(Direction.Right, Maze, true))
            {
                Blinky.Direction = Direction.Left;
                Blinky.SetPositionY(Board.OffsetY + Blinky.MazeRow * Board.Pixel);
            }
        }

        public void MoveInky()
        {
            if (!Inky.Check(Direction.Left, Maze, true))
            {
                Inky.SetPositionY(Board.OffsetY + Inky.MazeRow * Board.Pixel);
                Inky.Direction = Direction.Right;
            }
            if (!Inky.Check(Direction.Right, Maze, true))
            {
                Inky.Direction = Direction.Left;
                Inky.SetPositionY(Board.OffsetY + Inky.MazeRow * Board.Pixel);
            }
        }
    }
}
