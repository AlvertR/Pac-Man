using Raylib_cs;
using System.Numerics;

namespace Pac_Man
{
    public class Game
    {
        private static readonly Random Rnd = new Random();
        public Game(int width, int height, string name, int fps)
        {
            HeightWindow = height;
            WidthWindow = width;
            NameWindow = name;
            FPS = fps;
            TimeInDead = 2f;
            TimeToLife = 2f;
            TimeToNext = 2f;
            Board = new Board(WidthWindow, HeightWindow);
            Maze = new Maze(Board);
            Player = new Player('p', 100, Board);
            Ghost Blinky = new Ghost('b', Board, Color.Red, Direction.Left, 0f, '1', false, true);
            Ghost Pinky = new Ghost('r', Board, Color.Pink, Direction.Left, 5f,'2', false, true);
            Ghost Inky = new Ghost('i', Board, Color.SkyBlue, Direction.Right, 10f, '3', false, true);
            Ghost Clyde = new Ghost('c', Board, Color.Orange, Direction.Right, 15f, '4', false, true);
            Ghosts = new List<Ghost> { Blinky, Pinky, Inky, Clyde };
        }
        public int HeightWindow { get; private set; }
        public int WidthWindow { get; private set; }
        public string NameWindow { get; private set; } = string.Empty;
        public int FPS { get; set; }
        public float DeltaTime { get; set; } = 0;
        public float TimeInDead { get; set; }
        public float TimeToLife { get; set; }
        public float TimeToNext { get; set; }
        public GameStatus Status { get; set; } = GameStatus.Start;
        public Board Board { get; set; }
        public Maze Maze { get; set; }
        public Player Player { get; set; }
        public List<Ghost> Ghosts { get; set; }

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
                case GameStatus.Ready:
                case GameStatus.NextLevel:
                    Maze.Draw();
                    Raylib.DrawText("Puntuacion: " + Player.Points.ToString(), 0, 0, 24, Color.White);
                    Player.Draw();
                    foreach (var g in Ghosts)
                        g.Draw();
                    if (Status == GameStatus.Paused)
                        Raylib.DrawText("Pausa", 300, 400, 40, Color.White);
                    if (Status == GameStatus.Ready)
                        Raylib.DrawText("Listo!", 300, 400, 40, Color.White);
                    if (Status == GameStatus.NextLevel)
                        Raylib.DrawText("Nivel Completado", 300, 400, 40, Color.White);
                    break;
                case GameStatus.GameOver:
                    Raylib.DrawText("Fin del juego", 0, 0, 34, Color.White);
                    Raylib.DrawText("Puntuacion: " + Player.Points.ToString(), 0, 30, 24, Color.White);
                    Raylib.DrawText("Presion R para volver a jugar", 0, 60, 24, Color.White);
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
                    CheckEndLevel();
                    MoveGhost();
                    CheckCollision();
                    break;
                case GameStatus.DeadPlayer:
                    ResetPositions();
                    break;
                case GameStatus.Ready:
                    if (TimeToLife > 0)
                        TimeToLife -= DeltaTime;
                    else
                    {
                        Status = GameStatus.Playing;
                        TimeToLife = 2f;
                    }
                    break;
                case GameStatus.NextLevel:
                    if (TimeToNext > 0)
                        TimeToNext -= DeltaTime;
                    else
                    {
                        NextLevel();
                        TimeToNext = 2f;
                    }
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
                        Status = GameStatus.Ready;
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
                    if (Raylib.IsKeyPressed(KeyboardKey.R))
                        ResetGame();
                    break;
                case GameStatus.NextLevel:
                case GameStatus.DeadPlayer:
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

        public void MoveGhost()
        {
            var blinky = Ghosts.FirstOrDefault(g => g.Key == 'b');
            foreach (var g in Ghosts) {
                switch (g.Key)
                {
                    case 'b':
                        g.BaseMove(DeltaTime, Maze, Player.MazeRow, Player.MazeCol, Rnd);
                        break;
                    case 'r':
                        var targetPinky = g.GetTarget(Player, 4);
                        g.BaseMove(DeltaTime, Maze, targetPinky.row, targetPinky.col, Rnd);
                        break;
                    case 'i':
                        var pivote = g.GetTarget(Player, 2);
                        (int row, int col) targetInky = ((2 * pivote.row - blinky.MazeRow), (2 * pivote.col - blinky.MazeCol));
                        g.BaseMove(DeltaTime, Maze, targetInky.row, targetInky.col, Rnd);
                        break;
                    case 'c':
                        int dx = g.MazeCol - Player.MazeCol;
                        int dy = g.MazeRow - Player.MazeRow;
                        (int row, int col) targetClyde = dx * dx + dy * dy > 64 ? (Player.MazeRow, Player.MazeCol) : (g.ScatterCorner.Row, g.ScatterCorner.Col);
                        g.BaseMove(DeltaTime, Maze, targetClyde.row, targetClyde.col, Rnd);
                        break;
                }
            }
        }

        public bool Collides(Entity a, Entity b)
        {
            return Raylib.CheckCollisionCircles(new Vector2(a.Bounds.X + (a.Bounds.Width / 2), a.Bounds.Y + (a.Bounds.Height / 2)), (Board.Pixel / 2), 
                new Vector2(b.Bounds.X + (b.Bounds.Width / 2), b.Bounds.Y + (b.Bounds.Height / 2)), (Board.Pixel / 2));
        }

        public void KillPlayer()
        {
            Status = GameStatus.DeadPlayer;
            Player.Lifes--;
        }

        public void CheckCollision()
        {
            foreach(var g in Ghosts)
            {
                if(!Collides(Player, g)) continue;
                if(g.Status == GhostStatus.normal)
                {
                    KillPlayer();
                    return;
                }
                if (g.Status == GhostStatus.scared)
                {
                    Player.EatedGhosts++;
                    var points = 100 * Math.Pow(2, Player.EatedGhosts);
                    g.SetDead();
                    Player.Points += (int)points;
                }
            }
        }
        public void CheckEndLevel()
        {
            if(Maze.TotalPellets <= 0)
                Status = GameStatus.NextLevel;
        }

        public void NextLevel()
        {
            Player.ResetPosition();
            Player.Direction = Direction.Left;
            Player.DesiredDirection = Direction.Left;
            foreach (var g in Ghosts)
                g.Reset();
            Maze.SetCells();
            TimeInDead = 5f;
            Status = GameStatus.Ready;
        }
        public void setScared()
        {
            Player.EatedGhosts = 0;
            foreach (var g in Ghosts)
            {
                if(g.Status != GhostStatus.dead)
                    g.SetScared();
            }
        }

        public void ResetPositions()
        {
            if(TimeInDead > 0)
                TimeInDead -= DeltaTime;
            else
            {
                if (Player.Lifes == 0)
                    Status = GameStatus.GameOver;
                else
                {
                    Player.ResetPosition();
                    Player.Direction = Direction.Left;
                    Player.DesiredDirection = Direction.Left;
                    foreach (var g in Ghosts)
                        g.Reset();
                    TimeInDead = 5f;
                    Status = GameStatus.Ready;
                }
            }
        }

        public void ResetGame()
        {
            Player.ResetPosition();
            Player.Direction = Direction.Left;
            Player.DesiredDirection = Direction.Left;
            Player.Points = 0;
            Player.Lifes = 3;
            foreach (var g in Ghosts)
                g.Reset();
            Maze.SetCells();
            TimeInDead = 5f;
            Status = GameStatus.Ready;
        }
    }
}
