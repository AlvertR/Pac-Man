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
            Inky = new Ghost('i', 100, Board, Color.SkyBlue, Direction.Left, 10f, false, true);
            Pinky = new Ghost('r', 100, Board, Color.Pink, Direction.Right, 15f, false, true);
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
        public Ghost Pinky { get; set; }
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
                    Pinky.Draw();
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
                    Pinky.BaseMove(DeltaTime, Maze, MovePinky);
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
            bool isLast = Blinky.LastCell[0] == Blinky.MazeRow && Blinky.LastCell[1] == Blinky.MazeCol;
            if (Blinky.CenterX && Blinky.CenterY && !isLast)
            {
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
                    up = false;
                if (Blinky.Direction == Direction.Left)
                    right = false;
                if (Blinky.Direction == Direction.Right)
                    left = false;

                if (Blinky.Direction == Direction.Up && !left && !right && !up)
                {
                    Blinky.Direction = Direction.Down;
                    Blinky.SetPositionX(Board.OffsetX + Blinky.MazeCol * Board.Pixel);
                    Blinky.UpdateColRow();
                    return;
                }
                if (Blinky.Direction == Direction.Down && !left && !right && !down)
                {
                    Blinky.Direction = Direction.Up;
                    Blinky.SetPositionX(Board.OffsetX + Blinky.MazeCol * Board.Pixel);
                    Blinky.UpdateColRow();
                    return;
                }
                if (Blinky.Direction == Direction.Left && !up && !down && !left)
                {
                    Blinky.Direction = Direction.Right;
                    Blinky.SetPositionY(Board.OffsetY + Blinky.MazeRow * Board.Pixel);
                    Blinky.UpdateColRow();
                    return;
                }
                if (Blinky.Direction == Direction.Right && !up && !down && !right)
                {
                    Blinky.Direction = Direction.Left;
                    Blinky.SetPositionY(Board.OffsetY + Blinky.MazeRow * Board.Pixel);
                    Blinky.UpdateColRow();
                    return;
                }


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
                if(dDown < menor)
                {
                    menor=dDown;
                    dirMenor=Direction.Down.ToString();
                }
                if(dRight < menor)
                {
                    menor = dRight;
                    dirMenor = Direction.Right.ToString();
                }
                
                switch (dirMenor)
                {
                    case "Up":
                        Blinky.Direction = Direction.Up;
                        Blinky.SetPositionX(Board.OffsetX + Blinky.MazeCol * Board.Pixel);
                        break;
                    case "Left":
                        Blinky.Direction = Direction.Left;
                        Blinky.SetPositionY(Board.OffsetY + Blinky.MazeRow * Board.Pixel);
                        break;
                    case "Right":
                        Blinky.Direction = Direction.Right;
                        Blinky.SetPositionY(Board.OffsetY + Blinky.MazeRow * Board.Pixel);
                        break;
                    case "Down":
                        Blinky.Direction = Direction.Down;
                        Blinky.SetPositionX(Board.OffsetX + Blinky.MazeCol * Board.Pixel);
                        break;
                    default:
                        break;
                }

                Blinky.LastCell[0] = Blinky.MazeRow;
                Blinky.LastCell[1] = Blinky.MazeCol;
            }
        }

        public void MovePinky()
        {
            bool isLast = Pinky.LastCell[0] == Pinky.MazeRow && Pinky.LastCell[1] == Pinky.MazeCol;
            if (Pinky.CenterX && Pinky.CenterY && !isLast)
            {
                bool up = Pinky.Check(Direction.Up, Maze, Pinky.CanUseDoor);
                bool down = Pinky.Check(Direction.Down, Maze, Pinky.CanUseDoor);
                bool left = Pinky.Check(Direction.Left, Maze, Pinky.CanUseDoor);
                bool right = Pinky.Check(Direction.Right, Maze, Pinky.CanUseDoor);
                int dUp = 30 * 30 + 28 * 28;
                int dDown = 30 * 30 + 28 * 28;
                int dLeft = 30 * 30 + 28 * 28;
                int dRight = 30 * 30 + 28 * 28;
                int dx = 0;
                int dy = 0;
                int mazeCol = Pinky.MazeCol + 4;
                int mazeRow = Pinky.MazeRow + 4;
                string dirMenor = string.Empty;
                int menor = 0;

                if (Pinky.Direction == Direction.Up)
                    down = false;
                if (Pinky.Direction == Direction.Down)
                    up = false;
                if (Pinky.Direction == Direction.Left)
                    right = false;
                if (Pinky.Direction == Direction.Right)
                    left = false;

                if (Pinky.Direction == Direction.Up && !left && !right && !up)
                {
                    Pinky.Direction = Direction.Down;
                    Pinky.SetPositionX(Board.OffsetX + Pinky.MazeCol * Board.Pixel);
                    Pinky.UpdateColRow();
                    return;
                }
                if (Pinky.Direction == Direction.Down && !left && !right && !down)
                {
                    Pinky.Direction = Direction.Up;
                    Pinky.SetPositionX(Board.OffsetX + Pinky.MazeCol * Board.Pixel);
                    Pinky.UpdateColRow();
                    return;
                }
                if (Pinky.Direction == Direction.Left && !up && !down && !left)
                {
                    Pinky.Direction = Direction.Right;
                    Pinky.SetPositionY(Board.OffsetY + Pinky.MazeRow * Board.Pixel);
                    Pinky.UpdateColRow();
                    return;
                }
                if (Pinky.Direction == Direction.Right && !up && !down && !right)
                {
                    Pinky.Direction = Direction.Left;
                    Pinky.SetPositionY(Board.OffsetY + Pinky.MazeRow * Board.Pixel);
                    Pinky.UpdateColRow();
                    return;
                }


                if (up)
                {
                    dx = mazeCol - Player.MazeCol;
                    dy = mazeRow - 1 - Player.MazeRow;
                    dUp = dx * dx + dy * dy;
                }
                if (down)
                {
                    dx = mazeCol - Player.MazeCol;
                    dy = mazeRow + 1 - Player.MazeRow;
                    dDown = dx * dx + dy * dy;
                }
                if (left)
                {
                    dx = mazeCol - 1 - Player.MazeCol;
                    dy = mazeRow - Player.MazeRow;
                    dLeft = dx * dx + dy * dy;
                }
                if (right)
                {
                    dx = mazeCol + 1 - Player.MazeCol;
                    dy = mazeRow - Player.MazeRow;
                    dRight = dx * dx + dy * dy;
                }
                menor = dUp;
                dirMenor = Direction.Up.ToString();
                if (dLeft < menor)
                {
                    menor = dLeft;
                    dirMenor = Direction.Left.ToString();
                }
                if (dDown < menor)
                {
                    menor = dDown;
                    dirMenor = Direction.Down.ToString();
                }
                if (dRight < menor)
                {
                    menor = dRight;
                    dirMenor = Direction.Right.ToString();
                }

                switch (dirMenor)
                {
                    case "Up":
                        Pinky.Direction = Direction.Up;
                        Pinky.SetPositionX(Board.OffsetX + Pinky.MazeCol * Board.Pixel);
                        break;
                    case "Left":
                        Pinky.Direction = Direction.Left;
                        Pinky.SetPositionY(Board.OffsetY + Pinky.MazeRow * Board.Pixel);
                        break;
                    case "Right":
                        Pinky.Direction = Direction.Right;
                        Pinky.SetPositionY(Board.OffsetY + Pinky.MazeRow * Board.Pixel);
                        break;
                    case "Down":
                        Pinky.Direction = Direction.Down;
                        Pinky.SetPositionX(Board.OffsetX + Pinky.MazeCol * Board.Pixel);
                        break;
                    default:
                        break;
                }

                Pinky.LastCell[0] = Pinky.MazeRow;
                Pinky.LastCell[1] = Pinky.MazeCol;
            }
        }

        public void MoveInky()
        {
            bool isLast = Inky.LastCell[0] == Inky.MazeRow && Inky.LastCell[1] == Inky.MazeCol;
            if (Inky.CenterX && Inky.CenterY && !isLast)
            {
                bool up = Inky.Check(Direction.Up, Maze, Inky.CanUseDoor);
                bool down = Inky.Check(Direction.Down, Maze, Inky.CanUseDoor);
                bool left = Inky.Check(Direction.Left, Maze, Inky.CanUseDoor);
                bool right = Inky.Check(Direction.Right, Maze, Inky.CanUseDoor);
                int dUp = 30 * 30 + 28 * 28;
                int dDown = 30 * 30 + 28 * 28;
                int dLeft = 30 * 30 + 28 * 28;
                int dRight = 30 * 30 + 28 * 28;
                int dx = 0;
                int dy = 0;
                string dirMenor = string.Empty;
                int menor = 0;

                if (Inky.Direction == Direction.Up)
                    down = false;
                if (Inky.Direction == Direction.Down)
                    up = false;
                if (Inky.Direction == Direction.Left)
                    right = false;
                if (Inky.Direction == Direction.Right)
                    left = false;

                if (Inky.Direction == Direction.Up && !left && !right && !up)
                {
                    Inky.Direction = Direction.Down;
                    Inky.SetPositionX(Board.OffsetX + Inky.MazeCol * Board.Pixel);
                    Inky.UpdateColRow();
                    return;
                }
                if (Inky.Direction == Direction.Down && !left && !right && !down)
                {
                    Inky.Direction = Direction.Up;
                    Inky.SetPositionX(Board.OffsetX + Inky.MazeCol * Board.Pixel);
                    Inky.UpdateColRow();
                    return;
                }
                if (Inky.Direction == Direction.Left && !up && !down && !left)
                {
                    Inky.Direction = Direction.Right;
                    Inky.SetPositionY(Board.OffsetY + Inky.MazeRow * Board.Pixel);
                    Inky.UpdateColRow();
                    return;
                }
                if (Inky.Direction == Direction.Right && !up && !down && !right)
                {
                    Inky.Direction = Direction.Left;
                    Inky.SetPositionY(Board.OffsetY + Inky.MazeRow * Board.Pixel);
                    Inky.UpdateColRow();
                    return;
                }


                if (up)
                {
                    dx = Inky.MazeCol - Blinky.MazeCol;
                    dy = Inky.MazeRow - 1 - Blinky.MazeRow;
                    dUp = dx * dx + dy * dy;
                }
                if (down)
                {
                    dx = Inky.MazeCol - Blinky.MazeCol;
                    dy = Inky.MazeRow + 1 - Blinky.MazeRow;
                    dDown = dx * dx + dy * dy;
                }
                if (left)
                {
                    dx = Inky.MazeCol - 1 - Blinky.MazeCol;
                    dy = Inky.MazeRow - Blinky.MazeRow;
                    dLeft = dx * dx + dy * dy;
                }
                if (right)
                {
                    dx = Inky.MazeCol + 1 - Blinky.MazeCol;
                    dy = Inky.MazeRow - Blinky.MazeRow;
                    dRight = dx * dx + dy * dy;
                }
                menor = dUp;
                dirMenor = Direction.Up.ToString();
                if (dLeft < menor)
                {
                    menor = dLeft;
                    dirMenor = Direction.Left.ToString();
                }
                if (dDown < menor)
                {
                    menor = dDown;
                    dirMenor = Direction.Down.ToString();
                }
                if (dRight < menor)
                {
                    menor = dRight;
                    dirMenor = Direction.Right.ToString();
                }

                switch (dirMenor)
                {
                    case "Up":
                        Inky.Direction = Direction.Up;
                        Inky.SetPositionX(Board.OffsetX + Inky.MazeCol * Board.Pixel);
                        break;
                    case "Left":
                        Inky.Direction = Direction.Left;
                        Inky.SetPositionY(Board.OffsetY + Inky.MazeRow * Board.Pixel);
                        break;
                    case "Right":
                        Inky.Direction = Direction.Right;
                        Inky.SetPositionY(Board.OffsetY + Inky.MazeRow * Board.Pixel);
                        break;
                    case "Down":
                        Inky.Direction = Direction.Down;
                        Inky.SetPositionX(Board.OffsetX + Inky.MazeCol * Board.Pixel);
                        break;
                    default:
                        break;
                }

                Inky.LastCell[0] = Inky.MazeRow;
                Inky.LastCell[1] = Inky.MazeCol;
            }
        }
    }
}
