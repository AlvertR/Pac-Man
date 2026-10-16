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
            Blinky = new Ghost('b', 100, Board, Color.Red, Direction.Left, 0f, '1', true, false);
            Pinky = new Ghost('r', 100, Board, Color.Pink, Direction.Left, 5f,'2', false, true);
            Inky = new Ghost('i', 100, Board, Color.SkyBlue, Direction.Right, 10f, '3', false, true);
            Clyde = new Ghost('c', 100, Board, Color.Orange, Direction.Right, 15f, '4', false, true);
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
                    MoveGhost();
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
    }
}
