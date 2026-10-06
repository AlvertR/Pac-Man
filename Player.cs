using Raylib_cs;
using System.Numerics;

namespace Pac_Man
{
    public class Player
    {
        public Player(float speed, float screenWidth, float screenHeight, int cols, int rows)
        {
            Pixel = MathF.Min(screenWidth / cols, screenHeight/rows);
            float offsetY = (screenHeight - Pixel * rows) / 2f;
            int x0 = (int)MathF.Floor(14 * Pixel);
            int y0 = (int)MathF.Floor(offsetY + 23 * Pixel);
            int x1 = (int)MathF.Floor(15 * Pixel);
            int y1 = (int)MathF.Floor(offsetY + 24 * Pixel);
            Bounds = new Rectangle(x0, y0, x1 - x0, y1 - y0);
            Speed = speed;
        }
        public Rectangle Bounds { get; set; }
        public float Pixel { get; set; }
        public float Speed { get; set; }
        public PlayerDirection Direction { get; set; } = PlayerDirection.Lefth;
        public PlayerDirection BloquedDirection { get; set; } = PlayerDirection.None;

        public void Draw()
        {
            //Raylib.DrawRectangleV(new Vector2(Bounds.X, Bounds.Y), new Vector2(Bounds.Width, Bounds.Height), Color.Green);
            Raylib.DrawCircleV(new Vector2(Bounds.X+(Bounds.Width/2), Bounds.Y+(Bounds.Height/2)), (Pixel/2)-2, Color.Red);
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

        public void PlayerUp()
        { 
            Direction = PlayerDirection.Up;
            BloquedDirection = PlayerDirection.None;
        }

        public void PlayerDown()
        { 
            Direction = PlayerDirection.Down;
            BloquedDirection = PlayerDirection.None;
        }

        public void PlayerLefth()
        { 
            Direction = PlayerDirection.Lefth;
            BloquedDirection = PlayerDirection.None;
        }

        public void PlayerRight()
            => Direction = PlayerDirection.Right;

        public void HandleInput()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Up))
                PlayerUp();
            if (Raylib.IsKeyPressed(KeyboardKey.Down))
                PlayerDown();
            if (Raylib.IsKeyPressed(KeyboardKey.Left))
                PlayerLefth();
            if (Raylib.IsKeyPressed(KeyboardKey.Right))
                PlayerRight();
        }
    }
}
