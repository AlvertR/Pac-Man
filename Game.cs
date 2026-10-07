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
        public int PlayerMazeCol { get; set; } = -1;
        public int PlayerMazeRow { get; set; } = -1;
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
            Player = new Player(10, WidthWindow, HeightWindow, Maze.Sprite[0].Length, Maze.Sprite.Length);
            SetPlayerMazePosition();

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
            //Raylib.DrawText(NameWindow, 10, 5, 34, Color.White);
            Maze.Draw();
            Player.Draw();
            Raylib.EndDrawing();
        }

        public void Update()
        {
            MovementPlayer();
        }

        public void HandleInput()
        {
            Player.HandleInput();
            //Console.WriteLine(Maze.Sprite);
        }

        public void MovementPlayer()
        {
            float movement = 0;
            float newY = 0;
            float newX = 0;
            //Check();
            if (Player.Direction == PlayerDirection.Lefth && Check(PlayerDirection.Lefth))
            {
                movement -= Player.Speed * Raylib.GetFrameTime();
                newX = Player.Bounds.X + movement;
                Player.SetPositionX(newX);
                ChangePixelPlayer += Math.Abs(movement);
                if (ChangePixelPlayer >= Player.Pixel)
                {
                    Maze.Cells[PlayerMazeRow, PlayerMazeCol] = CellType.Path;
                    Maze.Cells[PlayerMazeRow, PlayerMazeCol - 1] = CellType.Player;
                    string oldP = Maze.Sprite[PlayerMazeRow];
                    string newP = oldP.Substring(0, PlayerMazeCol - 1) + "p-" + oldP.Substring(PlayerMazeCol + 1);
                    //Console.WriteLine("old " + oldP+ " new "+newP);
                    Maze.Sprite[PlayerMazeRow] = newP;
                    PlayerMazeCol--;
                    ChangePixelPlayer = 0;
                }
            }
            if (Player.Direction == PlayerDirection.Right && Check(PlayerDirection.Right))
            {
                movement += Player.Speed * Raylib.GetFrameTime();
                newX = Player.Bounds.X + movement;
                Player.SetPositionX(newX);
                ChangePixelPlayer += Math.Abs(movement);
                if (ChangePixelPlayer >= Player.Pixel)
                {
                    Maze.Cells[PlayerMazeRow, PlayerMazeCol] = CellType.Path;
                    Maze.Cells[PlayerMazeRow, PlayerMazeCol + 1] = CellType.Player;
                    string oldP = Maze.Sprite[PlayerMazeRow];
                    string newP = oldP.Substring(0, PlayerMazeCol) + "-p" + oldP.Substring(PlayerMazeCol + 2);
                    //Console.WriteLine("old " + oldP+ " new "+newP);
                    Maze.Sprite[PlayerMazeRow] = newP;
                    PlayerMazeCol++;
                    ChangePixelPlayer = 0;
                }
            }
            if (Player.Direction == PlayerDirection.Up)//falta provar
            {
                movement -= Player.Speed * Raylib.GetFrameTime();
                newY = Player.Bounds.Y + movement;
                Player.SetPositionY(newY);
                ChangePixelPlayer += Math.Abs(movement);
                if (ChangePixelPlayer >= Player.Pixel)
                {
                    Maze.Cells[PlayerMazeRow, PlayerMazeCol] = CellType.Path;
                    Maze.Cells[PlayerMazeRow-1, PlayerMazeCol] = CellType.Player;
                    string oldP = Maze.Sprite[PlayerMazeRow];
                    string newP = oldP.Substring(0, PlayerMazeCol-1) + "-" + oldP.Substring(PlayerMazeCol);
                    //Console.WriteLine("old " + oldP+ " new "+newP);
                    Maze.Sprite[PlayerMazeRow] = newP;
                    string oldP2 = Maze.Sprite[PlayerMazeRow-1];
                    string newP2 = oldP2.Substring(0, PlayerMazeCol-1) + "p" + oldP2.Substring(PlayerMazeCol);
                    Maze.Sprite[PlayerMazeRow-1] = newP2;
                    PlayerMazeRow--;
                    ChangePixelPlayer = 0;
                }
            }
            if (Player.Direction == PlayerDirection.Down)
            {
                movement += Player.Speed * Raylib.GetFrameTime();
                newY = Player.Bounds.Y + movement;
                Player.SetPositionY(newY);
            }
        }

        public bool Check(PlayerDirection direction)
        {
            bool canPass = false;
            switch (direction)
            {
                case PlayerDirection.Lefth:
                    if (Maze.Cells[PlayerMazeRow, PlayerMazeCol - 1] != CellType.Wall)
                        canPass = true;
                    break;
                case PlayerDirection.Right:
                    if (Maze.Cells[PlayerMazeRow, PlayerMazeCol + 1] != CellType.Wall)
                        canPass = true;
                    break;
                case PlayerDirection.Up:
                    if (Maze.Cells[PlayerMazeRow -1, PlayerMazeCol] != CellType.Wall)
                        canPass = true;
                    break;
                case PlayerDirection.Down:
                    if (Maze.Cells[PlayerMazeRow +1, PlayerMazeCol] != CellType.Wall)
                        canPass = true;
                    break;
            }
            return canPass;
        }

        public void SetPlayerMazePosition()
        {
            //Console.WriteLine("r " + Maze.Cells.GetLength(0) + " c " + Maze.Cells.GetLength(1));
            for(int r = 0; r < Maze.Cells.GetLength(0); r++)
            {
                for (int c = 0; c < Maze.Cells.GetLength(1); c++)
                {
                    if (Maze.Cells[r, c] == CellType.Player)
                    {
                        PlayerMazeCol = c;
                        PlayerMazeRow = r;
                        break;
                    }
                }
            }
        }
    }
}
