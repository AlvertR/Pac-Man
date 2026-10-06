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
            //Raylib.DrawText(NameWindow, 10, 5, 34, Color.White);
            Maze.Draw();
            Player.Draw();
            Raylib.ClearBackground(Color.Black);
            Raylib.EndDrawing();
        }

        public void Update()
        {
            MovementPlayer();
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
            Check();
            if (Player.Direction == PlayerDirection.Lefth && Player.BloquedDirection != PlayerDirection.Lefth)
            {
                movement -= Player.Speed * Raylib.GetFrameTime();
                newX = Player.Bounds.X + movement;
                Player.SetPositionX(newX);
            }
            if (Player.Direction == PlayerDirection.Right && Player.BloquedDirection != PlayerDirection.Right)
            {
                movement += Player.Speed * Raylib.GetFrameTime();
                newX = Player.Bounds.X + movement;
                Player.SetPositionX(newX);
            }
            if (Player.Direction == PlayerDirection.Up && Player.BloquedDirection != PlayerDirection.Up)
            {
                movement -= Player.Speed * Raylib.GetFrameTime();
                newY = Player.Bounds.Y + movement;
                Player.SetPositionY(newY);
            }
            if (Player.Direction == PlayerDirection.Down && Player.BloquedDirection != PlayerDirection.Down)
            {
                movement += Player.Speed * Raylib.GetFrameTime();
                newY = Player.Bounds.Y + movement;
                Player.SetPositionY(newY);
            }
        }

        public void Check()
        {
            foreach (var wall in Maze.Walls)
            {
                if (Raylib.CheckCollisionRecs(Player.Bounds, wall.Bounds))
                {
                    // Calcular cuánto se solapan en cada eje
                    float overlapX = Math.Min(Player.Bounds.X + Player.Bounds.Width, wall.Bounds.X + wall.Bounds.Width) - Math.Max(Player.Bounds.X, wall.Bounds.X);
                    float overlapY = Math.Min(Player.Bounds.Y + Player.Bounds.Height, wall.Bounds.Y + wall.Bounds.Height) - Math.Max(Player.Bounds.Y, wall.Bounds.Y);

                    // El lado de la colisión es aquel donde el solapamiento sea MENOR
                    if (overlapX < overlapY)
                    {
                        // Colisión Horizontal (Izquierda o Derecha)
                        if (Player.Bounds.X + Player.Bounds.Width / 2 < wall.Bounds.X + wall.Bounds.Width / 2)
                        {
                            //ladoColision = "IZQUIERDA";
                            Player.BloquedDirection = PlayerDirection.Right;
                            Player.SetPositionX(wall.Bounds.X - Player.Bounds.Width); // Empujar a la izquierda
                        }
                        else
                        {
                            //ladoColision = "DERECHA";
                            Player.BloquedDirection = PlayerDirection.Lefth;
                            Player.SetPositionX(wall.Bounds.X + wall.Bounds.Width); // Empujar a la derecha
                        }
                    }
                    else
                    {
                        // Colisión Vertical (Arriba o Abajo)
                        if (Player.Bounds.Y + Player.Bounds.Height / 2 < wall.Bounds.Y + wall.Bounds.Height / 2)
                        {
                            //ladoColision = "ARRIBA del obstáculo";
                            Player.BloquedDirection = PlayerDirection.Down;
                            Player.SetPositionY(wall.Bounds.Y - Player.Bounds.Height); // Empujar hacia arriba
                        }
                        else
                        {
                            //ladoColision = "ABAJO del obstáculo";
                            Player.BloquedDirection = PlayerDirection.Up;
                            Player.SetPositionY(wall.Bounds.Y + wall.Bounds.Height); // Empujar hacia abajo
                        }
                    }
                }
            }
        }
    }
}
