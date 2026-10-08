using Raylib_cs;
using System.Numerics;

namespace Pac_Man
{
    public class Player
    {
        public Player(float speed, float screenWidth, float screenHeight, int cols, int rows)
        {
            MazeCol = 14;
            MazeRow = 23;
            Pixel = MathF.Min(screenWidth / cols, screenHeight/rows);
            OffsetY = (screenHeight - Pixel * rows) / 2f;
            float x0 = MathF.Floor(MazeCol * Pixel);
            float y0 = MathF.Floor(OffsetY + MazeRow * Pixel);
            float x1 = MathF.Floor((MazeCol+1) * Pixel);
            float y1 = MathF.Floor(OffsetY + (MazeRow+1) * Pixel);
            Bounds = new Rectangle(x0, y0, x1 - x0, y1 - y0);
            Speed = speed;
        }
        public Rectangle Bounds { get; set; }
        public float Pixel { get; set; }
        public float Speed { get; set; }
        public PlayerDirection Direction { get; set; } = PlayerDirection.Left;
        public PlayerDirection DesiredDirection { get; set; } = PlayerDirection.None;
        public int MazeCol {  get; set; }
        public int MazeRow {  get; set; }
        public float OffsetY { get; set; }

        public void Draw()
        {
            Raylib.DrawCircleV(new Vector2(Bounds.X+(Bounds.Width/2), Bounds.Y+(Bounds.Height/2)), (Pixel/2), Color.Red);
        }

        public void SetPositionX(float position)
        {
            Rectangle newRecX = new Rectangle(position, Bounds.Y, Bounds.Width, Bounds.Height);
            this.Bounds = newRecX;
        }

        public void SetPositionY(float position)
        {
            Rectangle newRecY = new Rectangle(Bounds.X, position, Bounds.Width, Bounds.Height);
            this.Bounds = newRecY;
        }

        public void HandleInput()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Up))
                DesiredDirection = PlayerDirection.Up;
            if (Raylib.IsKeyPressed(KeyboardKey.Down))
                DesiredDirection = PlayerDirection.Down;
            if (Raylib.IsKeyPressed(KeyboardKey.Left))
                DesiredDirection = PlayerDirection.Left;
            if (Raylib.IsKeyPressed(KeyboardKey.Right))
                DesiredDirection = PlayerDirection.Right;
        }
    }
}
