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
            Maze = new Maze(WidthWindow, HeightWindow);
            Player = new Player(100, WidthWindow, HeightWindow, Maze.Sprite[0].Length, Maze.Sprite.Length);
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

            Maze.SetCells();

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
            if (Player.Direction == PlayerDirection.Left)
            {
                if (Check(PlayerDirection.Left))
                    Player.SetPositionX(Player.Bounds.X - Player.Speed * DeltaTime);
                else
                    Player.SetPositionX(Player.MazeCol * Player.Pixel);
            }
            if (Player.Direction == PlayerDirection.Right)
            {
                if(Check(PlayerDirection.Right))
                    Player.SetPositionX(Player.Bounds.X + Player.Speed * DeltaTime);
                else
                    Player.SetPositionX(Player.MazeCol * Player.Pixel);
            }
            if (Player.Direction == PlayerDirection.Up)
            {
                if (Check(PlayerDirection.Up))
                    Player.SetPositionY(Player.Bounds.Y - Player.Speed * DeltaTime);
                else
                    Player.SetPositionY(Player.OffsetY + Player.MazeRow * Player.Pixel);
            }
            if (Player.Direction == PlayerDirection.Down)
            {
                if(Check(PlayerDirection.Down))
                    Player.SetPositionY(Player.Bounds.Y + Player.Speed * DeltaTime);
                else
                    Player.SetPositionY(Player.OffsetY + Player.MazeRow * Player.Pixel);
            }
        }

        public void UpdateDirection()
        {
            bool update = false;
            float step = Player.Speed * DeltaTime;
            bool centerX = MathF.Abs(Player.Bounds.X - Player.MazeCol * Player.Pixel) <= step;
            bool centerY = MathF.Abs(Player.Bounds.Y - (Player.OffsetY + Player.MazeRow * Player.Pixel)) <= step;

            bool isReverse = (Player.Direction == PlayerDirection.Left && Player.DesiredDirection == PlayerDirection.Right)
               || (Player.Direction == PlayerDirection.Right && Player.DesiredDirection == PlayerDirection.Left)
               || (Player.Direction == PlayerDirection.Up && Player.DesiredDirection == PlayerDirection.Down)
               || (Player.Direction == PlayerDirection.Down && Player.DesiredDirection == PlayerDirection.Up);

            if (isReverse)
            {
                Player.Direction = Player.DesiredDirection;
                return;
            }

            if (Player.Direction == PlayerDirection.Up && centerY)
                update = true;
            if(Player.Direction == PlayerDirection.Left && centerX)
                update = true;
            if(Player.Direction == PlayerDirection.Right && centerX)
                update = true;
            if (Player.Direction == PlayerDirection.Down && centerY)
                update = true;
            if(Player.Direction == PlayerDirection.Left && !Check(PlayerDirection.Left))
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
                {
                    Player.Direction = PlayerDirection.Up;
                    Player.SetPositionX(Player.MazeCol * Player.Pixel);
                }
                if (Player.DesiredDirection == PlayerDirection.Down && Check(PlayerDirection.Down)) 
                { 
                    Player.Direction = PlayerDirection.Down;
                    Player.SetPositionX(Player.MazeCol * Player.Pixel);
                }
                if (Player.DesiredDirection == PlayerDirection.Left && Check(PlayerDirection.Left))
                {
                    Player.Direction = PlayerDirection.Left;
                    Player.SetPositionY(Player.OffsetY + Player.MazeRow * Player.Pixel);
                }
                if (Player.DesiredDirection == PlayerDirection.Right && Check(PlayerDirection.Right))
                {
                    Player.Direction = PlayerDirection.Right;
                    Player.SetPositionY(Player.OffsetY + Player.MazeRow * Player.Pixel);
                }
            }
        }

        public void UpdateColRow()
        {
            float x = Player.Bounds.X / Player.Pixel;
            float y = (Player.Bounds.Y - Player.OffsetY) / Player.Pixel;
            switch (Player.Direction)
            {
                case PlayerDirection.Left:
                    Player.MazeCol = (int)Math.Ceiling(x);
                    break;
                case PlayerDirection.Right:
                    Player.MazeCol = (int)Math.Floor(x);
                    break;
                case PlayerDirection.Up:
                    Player.MazeRow = (int)MathF.Ceiling(y);
                    break;
                case PlayerDirection.Down:
                    Player.MazeRow = (int)MathF.Floor(y);
                    break;
            }
        }

        public bool Check(PlayerDirection direction)
        {
            bool canPass = false;
            switch (direction)
            {
                case PlayerDirection.Left:
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
