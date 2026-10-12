using Raylib_cs;
using System.Numerics;

namespace Pac_Man
{
    public class Entity
    {
        public Rectangle Bounds { get; set; }
        public float Speed { get; set; }
        public Direction Direction { get; set; }
        public int MazeCol { get; set; }
        public int MazeRow { get; set; }
        public float DeltaTime { get; set; } = 0f;
        public Board Board { get; set; }

        public void SetDeltaTime(float deltaTime)
            => DeltaTime = deltaTime;
        public virtual void Draw()
        {
            Raylib.DrawRectangleV(new Vector2(Bounds.X, Bounds.Y), new Vector2(Bounds.Width, Bounds.Height), Color.Black);
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

        public void UpdateColRow()
        {
            float x = (Bounds.X - Board.OffsetX) / Board.Pixel;
            float y = (Bounds.Y - Board.OffsetY) / Board.Pixel;
            const float eps = 0.001f;
            switch (Direction)
            {
                case Direction.Left:
                    MazeCol = (int)MathF.Ceiling(x - eps);
                    break;
                case Direction.Right:
                    MazeCol = (int)MathF.Floor(x + eps);
                    break;
                case Direction.Up:
                    MazeRow = (int)MathF.Ceiling(y - eps);
                    break;
                case Direction.Down:
                    MazeRow = (int)MathF.Floor(y + eps);
                    break;
            }
        }

        public bool Check(Direction direction, Maze maze, bool canUseDoor)
        {
            bool canPass = false;
            switch (direction)
            {
                case Direction.Left:
                    canPass = maze.CanMove(MazeRow, MazeCol - 1, canUseDoor);
                    break;
                case Direction.Right:
                    canPass = maze.CanMove(MazeRow, MazeCol + 1, canUseDoor);
                    break;
                case Direction.Up:
                    canPass = maze.CanMove(MazeRow - 1, MazeCol, canUseDoor);
                    break;
                case Direction.Down:
                    canPass = maze.CanMove(MazeRow + 1, MazeCol, canUseDoor);
                    break;
            }
            return canPass;
        }

        public void Move(Maze maze, bool canUseDoor)
        {
            if (Direction == Direction.Left)
            {
                if (maze.Cells[MazeRow, MazeCol] == CellType.Bridge && MazeCol <=0)
                    SetPositionX(Board.OffsetX + (maze.Cells.GetLength(1)-1) * Board.Pixel);
                else if (Check(Direction.Left, maze, canUseDoor))
                    SetPositionX(Bounds.X - Speed * DeltaTime);
                else
                    SetPositionX(Board.OffsetX + MazeCol * Board.Pixel);
            }
            if (Direction == Direction.Right)
            {
                if (maze.Cells[MazeRow, MazeCol] == CellType.Bridge && MazeCol >= maze.Cells.GetLength(1)-1)
                    SetPositionX(Board.OffsetX);
                else if (Check(Direction.Right, maze, canUseDoor))
                    SetPositionX(Bounds.X + Speed * DeltaTime);
                else
                    SetPositionX(Board.OffsetX + MazeCol * Board.Pixel);
            }
            if (Direction == Direction.Up)
            {
                if (Check(Direction.Up, maze, canUseDoor))
                    SetPositionY(Bounds.Y - Speed * DeltaTime);
                else
                    SetPositionY(Board.OffsetY + MazeRow * Board.Pixel);
            }
            if (Direction == Direction.Down)
            {
                if (Check(Direction.Down, maze, canUseDoor))
                    SetPositionY(Bounds.Y + Speed * DeltaTime);
                else
                    SetPositionY(Board.OffsetY + MazeRow * Board.Pixel);
            }
        }
    }
}
