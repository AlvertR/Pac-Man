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
            Blinky.BaseMove(DeltaTime, Maze, Player.MazeRow, Player.MazeCol);

            var targetPinky = Pinky.GetTarget(Player, 4);
            Pinky.BaseMove(DeltaTime, Maze, targetPinky.row, targetPinky.col);

            var targetInky = GetTargetInky();
            Inky.BaseMove(DeltaTime, Maze, targetInky.row, targetInky.col);

            int dx = Clyde.MazeCol - Player.MazeCol;
            int dy = Clyde.MazeRow - Player.MazeRow;
            (int row, int col) targetClyde = dx * dx + dy * dy > 64 ? (Player.MazeRow, Player.MazeCol) : (Clyde.ScatterCorner.Row, Clyde.ScatterCorner.Col);
            Clyde.BaseMove(DeltaTime, Maze, targetClyde.row, targetClyde.col);
        }

        public void CheckCollision()
        {
            if ((Blinky.Status == GhostStatus.normal && Raylib.CheckCollisionRecs(Player.Bounds, Blinky.Bounds))
                || (Pinky.Status == GhostStatus.normal && Raylib.CheckCollisionRecs(Player.Bounds, Pinky.Bounds))
                || (Inky.Status == GhostStatus.normal && Raylib.CheckCollisionRecs(Player.Bounds, Inky.Bounds))
                || (Clyde.Status == GhostStatus.normal && Raylib.CheckCollisionRecs(Player.Bounds, Clyde.Bounds)))
            {
                Status = GameStatus.DeadPlayer;
                Player.Lifes--;
            }
        }

        public void setScared()
        {
            Console.WriteLine("scared start");
            Blinky.SetScared();
            Blinky.Speed = Blinky.Speed * 0.8f;
            Pinky.SetScared();
            Pinky.Speed = Pinky.Speed * 0.8f;
            Inky.SetScared();
            Inky.Speed = Inky.Speed * 0.8f;
            Clyde.SetScared();
            Clyde.Speed = Clyde.Speed * 0.8f;
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
