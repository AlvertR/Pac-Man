using Raylib_cs;
using System.Numerics;

namespace Pac_Man
{
    public class Blinky : Entity
    {
        public Blinky(float speed, Board board)
        {
            var spritePos = board.GetInitialPosition('b');
            MazeCol = spritePos[1];
            MazeRow = spritePos[0];
            Board = board;
            float x0 = Board.OffsetX + MazeCol * Board.Pixel;
            float y0 = Board.OffsetY + MazeRow * Board.Pixel;
            float x1 = Board.OffsetX + (MazeCol + 1) * Board.Pixel;
            float y1 = Board.OffsetY + (MazeRow + 1) * Board.Pixel;
            Bounds = new Rectangle(x0, y0, x1 - x0, y1 - y0);
            Speed = speed;
            Color = Color.Red;
            Direction = Direction.Right;
        }
        public Color Color { get; set; }

        public override void Draw()
        {
            Raylib.DrawCircleSector(new Vector2(Bounds.X + (Bounds.Width / 2), Bounds.Y + (Bounds.Height / 2)), (Board.Pixel / 2), 180, 360, 1, Color);
            Raylib.DrawRectangleV(new Vector2(Bounds.X, Bounds.Y + (Bounds.Height / 2)), new Vector2(Bounds.Width, Bounds.Height / 2), Color);
        }

        public void UpdateDirection(Maze maze)
        {
            if (!Check(Direction.Left, maze, true))
            {
                SetPositionY(Board.OffsetY + MazeRow * Board.Pixel);
                Direction = Direction.Right;
            }
            if (!Check(Direction.Right, maze, true))
            {
                Direction = Direction.Left;
                SetPositionY(Board.OffsetY + MazeRow * Board.Pixel);
            }
        }
    }
}
