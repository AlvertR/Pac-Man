using Raylib_cs;

namespace Pac_Man
{
    public abstract class Entity
    {
        protected Entity(char key, float speed, Board board)
        {
            var spritePos = board.GetInitialPosition(key);
            MazeCol = spritePos[1];
            MazeRow = spritePos[0];
            Board = board;
            float x0 = Board.OffsetX + MazeCol * Board.Pixel;
            float y0 = Board.OffsetY + MazeRow * Board.Pixel;
            float x1 = Board.OffsetX + (MazeCol + 1) * Board.Pixel;
            float y1 = Board.OffsetY + (MazeRow + 1) * Board.Pixel;
            Bounds = new Rectangle(x0, y0, x1 - x0, y1 - y0);
            Speed = speed;
        }
        public Rectangle Bounds { get; set; }
        public float Speed { get; set; }
        public Direction Direction { get; set; }
        public int MazeCol { get; set; }
        public int MazeRow { get; set; }
        public Board Board { get; set; }

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

        public void Move(Maze maze, bool canUseDoor, float deltaTime)
        {
            if (Direction == Direction.Left)
            {
                if (maze.Cells[MazeRow, MazeCol] == CellType.Bridge && MazeCol <=0)
                    SetPositionX(Board.OffsetX + (maze.Cells.GetLength(1)-1) * Board.Pixel);
                else if (Check(Direction.Left, maze, canUseDoor))
                    SetPositionX(Bounds.X - Speed * deltaTime);
                else
                    SetPositionX(Board.OffsetX + MazeCol * Board.Pixel);
            }
            if (Direction == Direction.Right)
            {
                if (maze.Cells[MazeRow, MazeCol] == CellType.Bridge && MazeCol >= maze.Cells.GetLength(1)-1)
                    SetPositionX(Board.OffsetX);
                else if (Check(Direction.Right, maze, canUseDoor))
                    SetPositionX(Bounds.X + Speed * deltaTime);
                else
                    SetPositionX(Board.OffsetX + MazeCol * Board.Pixel);
            }
            if (Direction == Direction.Up)
            {
                if (Check(Direction.Up, maze, canUseDoor))
                    SetPositionY(Bounds.Y - Speed * deltaTime);
                else
                    SetPositionY(Board.OffsetY + MazeRow * Board.Pixel);
            }
            if (Direction == Direction.Down)
            {
                if (Check(Direction.Down, maze, canUseDoor))
                    SetPositionY(Bounds.Y + Speed * deltaTime);
                else
                    SetPositionY(Board.OffsetY + MazeRow * Board.Pixel);
            }
        }
    }
}
