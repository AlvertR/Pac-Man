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
        }
        public int HeightWindow { get; private set; }
        public int WidthWindow { get; private set; }
        public string NameWindow { get; private set; } = string.Empty;
        public int FPS { get; set; }
        public float DeltaTime { get; set; } = 0;

        public Maze Maze { get; set; }
        public Player Player { get; set; }
        public int PlayerMazeCol { get; set; } = 14;
        public int PlayerMazeRow { get; set; } = 23;
        public float ChangePixelPlayer { get; set; } = 0f;

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
            //SetPlayerMazePosition();

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
            //UpdateDirection(); arreglar esto para que actualice a la mitad no inmediatamente
            //if (Player.Bounds.X >= (Player.Pixel / 2) + Player.Bounds.Width)
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
                ChangePixelPlayer += Math.Abs(movement);
                //if(ChangePixelPlayer >= Player.Pixel/3 && ChangePixelPlayer <= (Player.Pixel/3) * 2)
                //if (Player.Bounds.X <= PlayerMazeCol * Player.Pixel - (Player.Pixel / 2))
                //{
                //    //Console.WriteLine("plyX " + Player.Bounds.X + " calculo " + (PlayerMazeCol * Player.Pixel - (Player.Pixel / 2)));
                //    UpdateDirection();
                //}
                if (ChangePixelPlayer >= Player.Pixel)
                {
                    //Maze.Cells[PlayerMazeRow, PlayerMazeCol] = CellType.Path;
                    //PlayerMazeCol--;
                    ChangePixelPlayer -= Player.Pixel;
                }
            }
            if (Player.Direction == PlayerDirection.Right && Check(PlayerDirection.Right))
            {
                movement += Player.Speed * Raylib.GetFrameTime();
                newX = Player.Bounds.X + movement;
                Player.SetPositionX(newX);
                ChangePixelPlayer += Math.Abs(movement);
                //if (Player.Bounds.X + Player.Bounds.Width >= PlayerMazeCol * Player.Pixel + (Player.Pixel / 2))
                //{
                //    Console.WriteLine("plyX " + (Player.Bounds.X + Player.Bounds.Width) + " calculo " + (PlayerMazeCol * Player.Pixel + (Player.Pixel / 2)));
                //    UpdateDirection();
                //}
                if (ChangePixelPlayer >= Player.Pixel)
                {
                    //Maze.Cells[PlayerMazeRow, PlayerMazeCol] = CellType.Path;
                    //PlayerMazeCol++;
                    ChangePixelPlayer -= Player.Pixel;
                }
            }
            if (Player.Direction == PlayerDirection.Up && Check(PlayerDirection.Up))
            {
                movement -= Player.Speed * Raylib.GetFrameTime();
                newY = Player.Bounds.Y + movement;
                Player.SetPositionY(newY);
                ChangePixelPlayer += Math.Abs(movement);
                if (ChangePixelPlayer >= Player.Pixel)
                {
                    Maze.Cells[PlayerMazeRow, PlayerMazeCol] = CellType.Path;
                    //PlayerMazeRow--;
                    ChangePixelPlayer -= Player.Pixel;
                }
            }
            if (Player.Direction == PlayerDirection.Down && Check(PlayerDirection.Down))
            {
                movement += Player.Speed * Raylib.GetFrameTime();
                newY = Player.Bounds.Y + movement;
                Player.SetPositionY(newY);
                ChangePixelPlayer += Math.Abs(movement);
                if (ChangePixelPlayer >= Player.Pixel)
                {
                    Maze.Cells[PlayerMazeRow, PlayerMazeCol] = CellType.Path;
                    //PlayerMazeRow++;
                    ChangePixelPlayer -= Player.Pixel;
                }
            }
        }

        public void UpdateDirection()
        {
            if(Player.DesiredDirection == PlayerDirection.Up && Check(PlayerDirection.Up))
                Player.Direction = PlayerDirection.Up;
            if (Player.DesiredDirection == PlayerDirection.Down && Check(PlayerDirection.Down))
                Player.Direction = PlayerDirection.Down;
            if (Player.DesiredDirection == PlayerDirection.Lefth && Check(PlayerDirection.Lefth))
                Player.Direction = PlayerDirection.Lefth;
            if (Player.DesiredDirection == PlayerDirection.Right && Check(PlayerDirection.Right))
                Player.Direction = PlayerDirection.Right;
        }

        public void UpdateColRow()
        {
            switch (Player.Direction)
            {
                case PlayerDirection.Lefth:
                    PlayerMazeCol = (int)((Player.Bounds.X + Player.Bounds.Width) / Player.Pixel);
                    break;
                case PlayerDirection.Right:
                    PlayerMazeCol = (int)((Player.Bounds.X + Player.Bounds.Width) / Player.Pixel) - 1;
                    break;
                case PlayerDirection.Up:
                    PlayerMazeRow = (int)((Player.Bounds.Y ) / Player.Pixel) - 1;
                    break;
                case PlayerDirection.Down:
                    PlayerMazeRow = (int)((Player.Bounds.Y - Player.Bounds.Height) / Player.Pixel) - 1;
                    break;
            }
            //int col = (int)(Player.Bounds.X / Player.Pixel) -1;
            //int row = (int)(Player.Bounds.Y / Player.Pixel) -1;
            //PlayerMazeCol = col;
            //PlayerMazeRow = row;
        }

        public bool Check(PlayerDirection direction)
        {
            bool canPass = false;
            switch (direction)
            {
                case PlayerDirection.Lefth:
                    //if (Maze.Cells[PlayerMazeRow, PlayerMazeCol - 1] != CellType.Wall)
                    //    canPass = true;
                    canPass = Maze.CanMove(PlayerMazeRow, PlayerMazeCol-1);
                    break;
                case PlayerDirection.Right:
                    //if (Maze.Cells[PlayerMazeRow, PlayerMazeCol + 1] != CellType.Wall)
                    //    canPass = true;
                    canPass = Maze.CanMove(PlayerMazeRow, PlayerMazeCol + 1);
                    break;
                case PlayerDirection.Up:
                    //if (Maze.Cells[PlayerMazeRow -1, PlayerMazeCol] != CellType.Wall)
                    //    canPass = true;
                    canPass = Maze.CanMove(PlayerMazeRow-1, PlayerMazeCol);
                    break;
                case PlayerDirection.Down:
                    //if (Maze.Cells[PlayerMazeRow +1, PlayerMazeCol] != CellType.Wall)
                    //    canPass = true;
                    canPass = Maze.CanMove(PlayerMazeRow+1, PlayerMazeCol);
                    break;
            }
            Console.WriteLine("dir " + direction.ToString() + " move " + canPass.ToString() + " r " + PlayerMazeRow + " c " + PlayerMazeCol);
            return canPass;
        }

        //public void SetPlayerMazePosition()
        //{
        //    for(int r = 0; r < Maze.Cells.GetLength(0); r++)
        //    {
        //        for (int c = 0; c < Maze.Cells.GetLength(1); c++)
        //        {
        //            if (Maze.Cells[r, c] == CellType.Player)
        //            {
        //                PlayerMazeCol = c;
        //                PlayerMazeRow = r;
        //                break;
        //            }
        //        }
        //    }
        //}
    }
}
