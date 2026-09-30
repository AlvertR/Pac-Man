using Raylib_cs;
using System.Numerics;

namespace Pac_Man
{
    public class Player
    {
        public Player(float screenWidth, float screenHeight, int cols = 28, int rows = 31)
        {
            Pixel = MathF.Min(screenWidth / cols, screenHeight/rows);
            float offsetY = (screenHeight - Pixel * rows);
            int x0 = (int)MathF.Floor(14 * Pixel);
            int y0 = (int)MathF.Floor(offsetY + 24 * Pixel);
            int x1 = (int)MathF.Floor(15 * Pixel);
            int y1 = (int)MathF.Floor(offsetY + 25 * Pixel);
            Bounds = new Rectangle(x0, y0, x1 - x0, y1 - y0);
        }
        public Rectangle Bounds { get; set; }
        public float Pixel { get; set; }

        public void Draw()
        {
            //revisar posision
            Raylib.DrawCircleV(new Vector2(Bounds.X+(Bounds.Width/2), Bounds.Y+(Bounds.Height/2)), Pixel/2, Color.Red);
        }
    }
}
