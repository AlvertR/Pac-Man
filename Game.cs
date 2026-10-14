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
            Inky = new Ghost('i', 100, Board, Color.SkyBlue, Direction.Right, 10f, false, true);
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
            if (Blinky.CenterX && Blinky.CenterY)//se detiene, tal vez no queda centrado
            {
                Console.WriteLine("update dir");
                bool up = Blinky.Check(Direction.Up, Maze, Blinky.CanUseDoor);
                bool down = Blinky.Check(Direction.Down, Maze, Blinky.CanUseDoor);
                bool left = Blinky.Check(Direction.Left, Maze, Blinky.CanUseDoor);
                bool right = Blinky.Check(Direction.Right, Maze, Blinky.CanUseDoor);
                int dUp = 30*30 + 28*28;
                int dDown = 30 * 30 + 28 * 28;
                int dLeft = 30 * 30 + 28 * 28;
                int dRight = 30 * 30 + 28 * 28;
                int dx = 0;
                int dy = 0;
                string dirMenor = string.Empty;
                int menor = 0;
                
                if(Blinky.Direction == Direction.Up)
                    down = false;
                if (Blinky.Direction == Direction.Down)
                    up = true;
                if (Blinky.Direction == Direction.Left)
                    right = true;
                if (Blinky.Direction == Direction.Right)
                    left = true;

                if (Blinky.Direction == Direction.Up && !left && !right && !up)
                    down = true;
                if (Blinky.Direction == Direction.Down && !left && !right && !down)
                    up = true;
                if (Blinky.Direction == Direction.Left && !up && !down && !left)
                    right = true;
                if (Blinky.Direction == Direction.Right && !up && !down && !right)
                    left = true;

                if (up)
                {
                    dx = Blinky.MazeCol - Player.MazeCol;
                    dy = Blinky.MazeRow - 1 - Player.MazeRow;
                    dUp = dx * dx + dy * dy;
                }
                if (down)
                {
                    dx = Blinky.MazeCol - Player.MazeCol;
                    dy = Blinky.MazeRow + 1 - Player.MazeRow;
                    dDown = dx * dx + dy * dy;
                }
                if (left)
                {
                    dx = Blinky.MazeCol - 1 - Player.MazeCol;
                    dy = Blinky.MazeRow - Player.MazeRow;
                    dLeft = dx * dx + dy * dy;
                }
                if (right)
                {
                    dx = Blinky.MazeCol + 1 - Player.MazeCol;
                    dy = Blinky.MazeRow - Player.MazeRow;
                    dRight = dx * dx + dy * dy;
                }
                menor = dUp;
                dirMenor = Direction.Up.ToString();
                if(dLeft < menor)
                {
                    menor = dLeft;
                    dirMenor = Direction.Left.ToString();
                }
                if(dRight < menor)
                {
                    menor = dRight;
                    dirMenor = Direction.Right.ToString();
                }
                if(dDown < menor)
                {
                    menor=dDown;
                    dirMenor=Direction.Down.ToString();
                }
                switch (dirMenor)
                {
                    case "Up":
                        Blinky.Direction = Direction.Up;
                        break;
                    case "Left":
                        Blinky.Direction = Direction.Left;
                        break;
                    case "Right":
                        Blinky.Direction = Direction.Right;
                        break;
                    case "Down":
                        Blinky.Direction = Direction.Down;
                        break;
                    default:
                        break;
                }
                
                Blinky.LastCell = new int[2] { Blinky.MazeRow, Blinky.MazeCol };
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
