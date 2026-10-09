using Raylib_cs;
using System.Numerics;

namespace Pac_Man
{
    public class Entity
    {
        public Rectangle Bounds { get; set; }
        public float Speed { get; set; }
        public PlayerDirection Direction { get; set; }
        public int MazeCol { get; set; }
        public int MazeRow { get; set; }
        public float DeltaTime { get; set; } = 0f;
        public Sprite Sprite { get; set; }

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
            float x = Bounds.X / Sprite.Pixel;
            float y = (Bounds.Y - Sprite.OffsetY) / Sprite.Pixel;
            const float eps = 0.001f;
            switch (Direction)
            {
                case PlayerDirection.Left:
                    MazeCol = (int)MathF.Ceiling(x - eps);
                    break;
                case PlayerDirection.Right:
                    MazeCol = (int)MathF.Floor(x + eps);
                    break;
                case PlayerDirection.Up:
                    MazeRow = (int)MathF.Ceiling(y - eps);
                    break;
                case PlayerDirection.Down:
                    MazeRow = (int)MathF.Floor(y + eps);
                    break;
            }
        }

        public bool Check(PlayerDirection direction, Maze maze)
        {
            bool canPass = false;
            switch (direction)
            {
                case PlayerDirection.Left:
                    canPass = maze.CanMove(MazeRow, MazeCol - 1);
                    break;
                case PlayerDirection.Right:
                    canPass = maze.CanMove(MazeRow, MazeCol + 1);
                    break;
                case PlayerDirection.Up:
                    canPass = maze.CanMove(MazeRow - 1, MazeCol);
                    break;
                case PlayerDirection.Down:
                    canPass = maze.CanMove(MazeRow + 1, MazeCol);
                    break;
            }
            return canPass;
        }

        public void Move(Maze maze)
        {
            if (Direction == PlayerDirection.Left)
            {
                if (Check(PlayerDirection.Left, maze))
                    SetPositionX(Bounds.X - Speed * DeltaTime);
                else
                    SetPositionX(MazeCol * Sprite.Pixel);
            }
            if (Direction == PlayerDirection.Right)
            {
                if (Check(PlayerDirection.Right, maze))
                    SetPositionX(Bounds.X + Speed * DeltaTime);
                else
                    SetPositionX(MazeCol * Sprite.Pixel);
            }
            if (Direction == PlayerDirection.Up)
            {
                if (Check(PlayerDirection.Up, maze))
                    SetPositionY(Bounds.Y - Speed * DeltaTime);
                else
                    SetPositionY(Sprite.OffsetY + MazeRow * Sprite.Pixel);
            }
            if (Direction == PlayerDirection.Down)
            {
                if (Check(PlayerDirection.Down, maze))
                    SetPositionY(Bounds.Y + Speed * DeltaTime);
                else
                    SetPositionY(Sprite.OffsetY + MazeRow * Sprite.Pixel);
            }
        }
    }
}
