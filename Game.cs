using Raylib_cs;
using System.Numerics;

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
            TimeInDead = 5f;
            TimeToLife = 5f;
            Board = new Board(WidthWindow, HeightWindow);
            Maze = new Maze(Board);
            Player = new Player('p', 100, Board);
            Blinky = new Ghost('b', 80, Board, Color.Red, Direction.Left, 0f, '1', true, false);
            Pinky = new Ghost('r', 80, Board, Color.Pink, Direction.Left, 5f,'2', false, true);
            Inky = new Ghost('i', 80, Board, Color.SkyBlue, Direction.Right, 10f, '3', false, true);
            Clyde = new Ghost('c', 80, Board, Color.Orange, Direction.Right, 15f, '4', false, true);
        }
        public int HeightWindow { get; private set; }
        public int WidthWindow { get; private set; }
        public string NameWindow { get; private set; } = string.Empty;
        public int FPS { get; set; }
        public float DeltaTime { get; set; } = 0;
        public float TimeInDead { get; set; }
        public float TimeToLife { get; set; }
        public GameStatus Status { get; set; } = GameStatus.Start;
        public Board Board { get; set; }
        public Maze Maze { get; set; }
        public Player Player { get; set; }
        public Ghost Blinky { get; set; }
        public Ghost Pinky { get; set; }
        public Ghost Inky { get; set; }
        public Ghost Clyde { get; set; }

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
                case GameStatus.DeadPlayer:
                    Maze.Draw();
                    Raylib.DrawText("Puntuacion: " + Player.Points.ToString(), 0, 0, 24, Color.White);
                    Player.Draw();
                    Blinky.Draw();
                    Pinky.Draw();
                    Inky.Draw();
                    Clyde.Draw();
                    if(Status == GameStatus.Paused)
                        Raylib.DrawText("Pausa", 300, 400, 40, Color.White);
                    break;
                case GameStatus.GameOver:
                    Raylib.DrawText("Fin del juego", 0, 0, 34, Color.White);
                    Raylib.DrawText("Puntuacion: " + Player.Points.ToString(), 0, 30, 24, Color.White);
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
                    //Console.WriteLine("check " + Player.isPowerPelletCell(Maze));
                    //if(Player.isPowerPelletCell(Maze))
                    //    setScared();
                    MovementPlayer();
                    MoveGhost();
                    CheckCollision();
                    break;
                case GameStatus.DeadPlayer:
                    ResetPositions();
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
                case GameStatus.DeadPlayer:
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
            var scared = Player.UpdatePoints(Maze);
            if (scared)
                setScared();
        }

        public (int row, int col) GetTargetInky()
        {
            var pivote = Inky.GetTarget(Player, 2);
            return ((2 * pivote.row - Blinky.MazeRow), (2 * pivote.col - Blinky.MazeCol));
        }

        public void MoveGhost()
        {
            Blinky.BaseMove(DeltaTime, Maze, Player.MazeRow, Player.MazeCol, 'b');

            var targetPinky = Pinky.GetTarget(Player, 4);
            Pinky.BaseMove(DeltaTime, Maze, targetPinky.row, targetPinky.col, 'r');

            var targetInky = GetTargetInky();
            Inky.BaseMove(DeltaTime, Maze, targetInky.row, targetInky.col, 'i');

            int dx = Clyde.MazeCol - Player.MazeCol;
            int dy = Clyde.MazeRow - Player.MazeRow;
            (int row, int col) targetClyde = dx * dx + dy * dy > 64 ? (Player.MazeRow, Player.MazeCol) : (Clyde.ScatterCorner.Row, Clyde.ScatterCorner.Col);
            Clyde.BaseMove(DeltaTime, Maze, targetClyde.row, targetClyde.col, 'c');
        }

        public void CheckCollision()
        {
            if ((Blinky.Status == GhostStatus.normal && Raylib.CheckCollisionCircles(new Vector2(Player.Bounds.X + (Player.Bounds.Width/2), Player.Bounds.Y + (Player.Bounds.Height/2)), (Board.Pixel / 2), new Vector2(Blinky.Bounds.X + (Blinky.Bounds.Width / 2), Blinky.Bounds.Y + (Blinky.Bounds.Height / 2)), (Board.Pixel / 2)))
                || (Pinky.Status == GhostStatus.normal && Raylib.CheckCollisionCircles(new Vector2(Player.Bounds.X + (Player.Bounds.Width / 2), Player.Bounds.Y + (Player.Bounds.Height / 2)), (Board.Pixel / 2), new Vector2(Pinky.Bounds.X + (Pinky.Bounds.Width / 2), Pinky.Bounds.Y + (Pinky.Bounds.Height / 2)), (Board.Pixel / 2)))
                || (Inky.Status == GhostStatus.normal && Raylib.CheckCollisionCircles(new Vector2(Player.Bounds.X + (Player.Bounds.Width / 2), Player.Bounds.Y + (Player.Bounds.Height / 2)), (Board.Pixel / 2), new Vector2(Inky.Bounds.X + (Inky.Bounds.Width / 2), Inky.Bounds.Y + (Inky.Bounds.Height / 2)), (Board.Pixel / 2)))
                || (Clyde.Status == GhostStatus.normal && Raylib.CheckCollisionCircles(new Vector2(Player.Bounds.X + (Player.Bounds.Width / 2), Player.Bounds.Y + (Player.Bounds.Height / 2)), (Board.Pixel / 2), new Vector2(Clyde.Bounds.X + (Clyde.Bounds.Width / 2), Clyde.Bounds.Y + (Clyde.Bounds.Height / 2)), (Board.Pixel / 2))))
            {
                Status = GameStatus.DeadPlayer;
                Player.Lifes--;
            }
            if ((Blinky.Status == GhostStatus.scared && Raylib.CheckCollisionCircles(new Vector2(Player.Bounds.X + (Player.Bounds.Width / 2), Player.Bounds.Y + (Player.Bounds.Height / 2)), (Board.Pixel / 2), new Vector2(Blinky.Bounds.X + (Blinky.Bounds.Width / 2), Blinky.Bounds.Y + (Blinky.Bounds.Height / 2)), (Board.Pixel / 2))))
            {
                Blinky.Status = GhostStatus.dead;
                Blinky.Speed = Blinky.Speed * 4;
                Blinky.CanUseDoor = false;
                Player.Points += 100;
            }
            if ((Pinky.Status == GhostStatus.scared && Raylib.CheckCollisionCircles(new Vector2(Player.Bounds.X + (Player.Bounds.Width / 2), Player.Bounds.Y + (Player.Bounds.Height / 2)), (Board.Pixel / 2), new Vector2(Pinky.Bounds.X + (Pinky.Bounds.Width / 2), Pinky.Bounds.Y + (Pinky.Bounds.Height / 2)), (Board.Pixel / 2))))
            {
                Pinky.Status = GhostStatus.dead;
                Pinky.Speed = Pinky.Speed * 4;
                Pinky.CanUseDoor = true;
                Player.Points += 100;
            }
            if ((Inky.Status == GhostStatus.scared && Raylib.CheckCollisionCircles(new Vector2(Player.Bounds.X + (Player.Bounds.Width / 2), Player.Bounds.Y + (Player.Bounds.Height / 2)), (Board.Pixel / 2), new Vector2(Inky.Bounds.X + (Inky.Bounds.Width / 2), Inky.Bounds.Y + (Inky.Bounds.Height / 2)), (Board.Pixel / 2))))
            {
                Inky.Status = GhostStatus.dead;
                Inky.Speed = Inky.Speed * 4;
                Inky.CanUseDoor = true;
                Player.Points += 100;
            }
            if ((Clyde.Status == GhostStatus.scared && Raylib.CheckCollisionCircles(new Vector2(Player.Bounds.X + (Player.Bounds.Width / 2), Player.Bounds.Y + (Player.Bounds.Height / 2)), (Board.Pixel / 2), new Vector2(Clyde.Bounds.X + (Clyde.Bounds.Width / 2), Clyde.Bounds.Y + (Clyde.Bounds.Height / 2)), (Board.Pixel / 2))))
            {
                Clyde.Status = GhostStatus.dead;
                Clyde.Speed = Clyde.Speed * 4;
                Clyde.CanUseDoor = true;
                Player.Points += 100;
            }
        }

        public void setScared()
        {
            //Console.WriteLine("scared start");
            if (Blinky.OutRoom)
            {
                Blinky.SetScared();
                Blinky.TimeScared = 10f;
                Blinky.Speed = Blinky.Speed * 0.8f;
            }
            if (Pinky.OutRoom)
            {
                Pinky.SetScared();
                Pinky.TimeScared = 10f;
                Pinky.Speed = Pinky.Speed * 0.8f;
            }
            if (Inky.OutRoom)
            {
                Inky.SetScared();
                Inky.TimeScared = 10f;
                Inky.Speed = Inky.Speed * 0.8f;
            }
            if (Clyde.OutRoom)
            {
                Clyde.SetScared();
                Clyde.TimeScared = 10f;
                Clyde.Speed = Clyde.Speed * 0.8f;
            }
        }

        public void ResetPositions()
        {
            if (TimeInDead <= 0)
            {
                if (Player.Lifes <= 0)
                    Status = GameStatus.GameOver;
                else
                {
                    Player.ResetPosition('p');
                    Player.Direction = Direction.Left;
                    Player.DesiredDirection = Direction.Left;
                    Blinky.Reset('b', 0f, true, false);
                    Pinky.Reset('r', 5f, false, true);
                    Inky.Reset('i', 10f, false, true);
                    Clyde.Reset('c', 15f, false, true);
                    if (TimeToLife <= 0)
                    {
                        Status = GameStatus.Playing;
                        TimeToLife = 5f;
                        TimeInDead = 5f;
                    }
                    else
                        TimeToLife -= DeltaTime;
                }
            }
            else
                TimeInDead -= DeltaTime;
        }
    }
}
