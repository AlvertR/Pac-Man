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

        public void Run()
        {
            Raylib.InitWindow(this.WidthWindow, this.HeightWindow, this.NameWindow);
            Raylib.SetTargetFPS(this.FPS);
            while (!Raylib.WindowShouldClose())
            {
                this.DeltaTime = Raylib.GetFrameTime();
                Raylib.BeginDrawing();
                Raylib.DrawText(NameWindow, 10, 5, 34, Color.White);
                Raylib.ClearBackground(Color.Black);
                Raylib.EndDrawing();
                //HandleInput();
                //Update();
                //Draw();
            }
            Raylib.CloseWindow();
        }
    }
}
