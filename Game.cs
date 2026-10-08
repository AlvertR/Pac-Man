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
        }
        public int HeightWindow { get; private set; }
        public int WidthWindow { get; private set; }
        public string NameWindow { get; private set; } = string.Empty;
        public int FPS { get; set; }
        public float DeltaTime { get; set; } = 0;

        public Maze Maze { get; set; }
        public Player Player { get; set; }

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

            Maze = new Maze(WidthWindow, HeightWindow);
            Maze.SetCells();
            Player = new Player(100, WidthWindow, HeightWindow, Maze.Sprite[0].Length, Maze.Sprite.Length);

            while (!Raylib.WindowShouldClose())
            {
                this.DeltaTime = Raylib.GetFrameTime();
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
            Player.Draw();
            Raylib.EndDrawing();
        }

        public void Update()
        {
            MovementPlayer();
            UpdateColRow();
            UpdateDirection();
        }

        public void HandleInput()
        {
            Player.HandleInput();
        }

        public void MovementPlayer()
        {
            float movement = 0;
            float newY = 0;
            float newX = 0;
            if (Player.Direction == PlayerDirection.Lefth && Check(PlayerDirection.Lefth))
            {
                movement -= Player.Speed * Raylib.GetFrameTime();
                newX = Player.Bounds.X + movement;
                Player.SetPositionX(newX);
            }
            if (Player.Direction == PlayerDirection.Right && Check(PlayerDirection.Right))
            {
                movement += Player.Speed * Raylib.GetFrameTime();
                newX = Player.Bounds.X + movement;
                Player.SetPositionX(newX);
            }
            if (Player.Direction == PlayerDirection.Up && Check(PlayerDirection.Up))
            {
                movement -= Player.Speed * Raylib.GetFrameTime();
                newY = Player.Bounds.Y + movement;
                Player.SetPositionY(newY);
            }
            if (Player.Direction == PlayerDirection.Down && Check(PlayerDirection.Down))
            {
                movement += Player.Speed * Raylib.GetFrameTime();
                newY = Player.Bounds.Y + movement;
                Player.SetPositionY(newY);
            }
        }

        public void UpdateDirection()
        {
            bool update = false;
            if (Player.Direction == PlayerDirection.Up && (Player.Bounds.Y + Player.Bounds.Height <= ((Player.MazeRow + 2) * Player.Pixel) + Player.Pixel))
                update = true;
            if(Player.Direction == PlayerDirection.Lefth && (Player.Bounds.X + Player.Bounds.Width <= (Player.MazeCol * Player.Pixel) + Player.Pixel))
                update = true;
            if(Player.Direction == PlayerDirection.Right && (Player.Bounds.X >= Player.MazeCol * Player.Pixel))
                update = true;
            if (Player.Direction == PlayerDirection.Down && (Player.Bounds.Y >= Player.MazeRow * Player.Pixel ))
                update = true;
            if(Player.Direction == PlayerDirection.Lefth && !Check(PlayerDirection.Lefth))
                update = true;
            if (Player.Direction == PlayerDirection.Right&& !Check(PlayerDirection.Right))
                update = true;
            if (Player.Direction == PlayerDirection.Up && !Check(PlayerDirection.Up))
                update = true;
            if (Player.Direction == PlayerDirection.Down && !Check(PlayerDirection.Down))
                update = true;

            if (update)
            {
                if (Player.DesiredDirection == PlayerDirection.Up && Check(PlayerDirection.Up))
                    Player.Direction = PlayerDirection.Up;
                if (Player.DesiredDirection == PlayerDirection.Down && Check(PlayerDirection.Down))
                    Player.Direction = PlayerDirection.Down;
                if (Player.DesiredDirection == PlayerDirection.Lefth && Check(PlayerDirection.Lefth))
                    Player.Direction = PlayerDirection.Lefth;
                if (Player.DesiredDirection == PlayerDirection.Right && Check(PlayerDirection.Right))
                    Player.Direction = PlayerDirection.Right;
            }
        }

        public void UpdateColRow()
        {
            switch (Player.Direction)
            {
                case PlayerDirection.Lefth:
                    Player.MazeCol = (int)((Player.Bounds.X + Player.Bounds.Width) / Player.Pixel);
                    break;
                case PlayerDirection.Right:
                    Player.MazeCol = (int)((Player.Bounds.X + Player.Bounds.Width) / Player.Pixel) - 1;
                    break;
                case PlayerDirection.Up:
                    Player.MazeRow = (int)((Player.Bounds.Y ) / Player.Pixel) - 1;
                    break;
                case PlayerDirection.Down:
                    Player.MazeRow = (int)((Player.Bounds.Y - Player.Bounds.Height) / Player.Pixel) - 1;
                    break;
            }
        }

        public bool Check(PlayerDirection direction)
        {
            bool canPass = false;
            switch (direction)
            {
                case PlayerDirection.Lefth:
                    canPass = Maze.CanMove(Player.MazeRow, Player.MazeCol-1);
                    break;
                case PlayerDirection.Right:
                    canPass = Maze.CanMove(Player.MazeRow, Player.MazeCol + 1);
                    break;
                case PlayerDirection.Up:
                    canPass = Maze.CanMove(Player.MazeRow-1, Player.MazeCol);
                    break;
                case PlayerDirection.Down:
                    canPass = Maze.CanMove(Player.MazeRow+1, Player.MazeCol);
                    break;
            }
            return canPass;
        }
    }
}
