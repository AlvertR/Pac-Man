using Raylib_cs;
using System.Numerics;

namespace Pac_Man
{
    public class Ghost : Entity
    {
        public Ghost(float speed, Color color, int mazeCol, int mazeRow, Sprite sprite)
        {
            MazeCol = mazeCol;
            MazeRow = mazeRow;
            Sprite = sprite;
            float x0 = MathF.Floor(MazeCol * Sprite.Pixel);
            float y0 = MathF.Floor(Sprite.OffsetY + MazeRow * Sprite.Pixel);
            float x1 = MathF.Floor((MazeCol + 1) * Sprite.Pixel);
            float y1 = MathF.Floor(Sprite.OffsetY + (MazeRow + 1) * Sprite.Pixel);
            Bounds = new Rectangle(x0, y0, x1 - x0, y1 - y0);
            Speed = speed;
            Color = color;
            Direction = PlayerDirection.Right;
        }
        public Color Color { get; set; }

        public override void Draw()
        {
            Raylib.DrawCircleSector(new Vector2(Bounds.X + (Bounds.Width / 2), Bounds.Y + (Bounds.Height / 2)), (Sprite.Pixel / 2), 180, 360, 1, Color);
            Raylib.DrawRectangleV(new Vector2(Bounds.X, Bounds.Y + (Bounds.Height / 2)), new Vector2(Bounds.Width, Bounds.Height / 2), Color);
        }

        public void UpdateDirection(Maze maze)
        {
            if (!Check(PlayerDirection.Left, maze))
            {
                SetPositionY(Sprite.OffsetY + MazeRow * Sprite.Pixel);
                Direction = PlayerDirection.Right;
            }
            if (!Check(PlayerDirection.Right, maze))
            {
                Direction = PlayerDirection.Left;
                SetPositionY(Sprite.OffsetY + MazeRow * Sprite.Pixel);
            }
        }
    }
}
