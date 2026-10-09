using Raylib_cs;
using System.Numerics;

namespace Pac_Man
{
    public class Player : Entity
    {
        public Player(float speed, Sprite sprite)
        {
            Sprite = sprite;
            var spritePos = Sprite.GetPlayerInitialPos();
            MazeRow = spritePos[0];
            MazeCol = spritePos[1];
            float x0 = MathF.Floor(MazeCol * Sprite.Pixel);
            float y0 = MathF.Floor(Sprite.OffsetY + MazeRow * Sprite.Pixel);
            float x1 = MathF.Floor((MazeCol+1) * Sprite.Pixel);
            float y1 = MathF.Floor(Sprite.OffsetY + (MazeRow+1) * Sprite.Pixel);
            Bounds = new Rectangle(x0, y0, x1 - x0, y1 - y0);
            Speed = speed;
            Points = 0f;
            Direction = PlayerDirection.Left;
        }
        public PlayerDirection DesiredDirection { get; set; } = PlayerDirection.None;
        public float Points {  get; set; }

        public override void Draw()
        {
            Raylib.DrawCircleV(new Vector2(Bounds.X+(Bounds.Width/2), Bounds.Y+(Bounds.Height/2)), (Sprite.Pixel/2), Color.Red);
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

        public void UpdatePoints(Maze maze)
        {
            var currentCell = maze.Cells[MazeRow, MazeCol];
            if (currentCell == CellType.Pellet)
            {
                maze.Cells[MazeRow, MazeCol] = CellType.Path;
                Points += 10;
            }
            if (currentCell == CellType.PowerPellet)
            {
                maze.Cells[MazeRow, MazeCol] = CellType.Path;
                Points += 50;
            }
        }

        public void UpdateDirection(Maze maze)
        {
            bool update = false;
            float step = Speed * DeltaTime;
            bool centerX = MathF.Abs(Bounds.X - MazeCol * Sprite.Pixel) <= step;
            bool centerY = MathF.Abs(Bounds.Y - (Sprite.OffsetY + MazeRow * Sprite.Pixel)) <= step;

            bool isReverse = (Direction == PlayerDirection.Left && DesiredDirection == PlayerDirection.Right)
               || (Direction == PlayerDirection.Right && DesiredDirection == PlayerDirection.Left)
               || (Direction == PlayerDirection.Up && DesiredDirection == PlayerDirection.Down)
               || (Direction == PlayerDirection.Down && DesiredDirection == PlayerDirection.Up);

            if (isReverse)
            {
                Direction = DesiredDirection;
                UpdateColRow();
                return;
            }

            if (Direction == PlayerDirection.Up && centerY)
                update = true;
            if (Direction == PlayerDirection.Left && centerX)
                update = true;
            if (Direction == PlayerDirection.Right && centerX)
                update = true;
            if (Direction == PlayerDirection.Down && centerY)
                update = true;


            if (update)
            {
                if (DesiredDirection == PlayerDirection.Up && Check(PlayerDirection.Up, maze))
                {
                    Direction = PlayerDirection.Up;
                    SetPositionX(MazeCol * Sprite.Pixel);
                }
                if (DesiredDirection == PlayerDirection.Down && Check(PlayerDirection.Down, maze))
                {
                    Direction = PlayerDirection.Down;
                    SetPositionX(MazeCol * Sprite.Pixel);
                }
                if (DesiredDirection == PlayerDirection.Left && Check(PlayerDirection.Left, maze))
                {
                    Direction = PlayerDirection.Left;
                    SetPositionY(Sprite.OffsetY + MazeRow * Sprite.Pixel);
                }
                if (DesiredDirection == PlayerDirection.Right && Check(PlayerDirection.Right, maze))
                {
                    Direction = PlayerDirection.Right;
                    SetPositionY(Sprite.OffsetY + MazeRow * Sprite.Pixel);
                }
            }
        }
    }
}
