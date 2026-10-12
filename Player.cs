using Raylib_cs;
using System.Numerics;

namespace Pac_Man
{
    public class Player : Entity
    {
        public Player(float speed, Board board)
        {
            Board = board;
            var spritePos = Board.GetPlayerInitialPos();
            MazeRow = spritePos[0];
            MazeCol = spritePos[1];
            float x0 = Board.OffsetX + MazeCol * Board.Pixel;
            float y0 = Board.OffsetY + MazeRow * Board.Pixel;
            float x1 = Board.OffsetX + (MazeCol+1) * Board.Pixel;
            float y1 = Board.OffsetY + (MazeRow+1) * Board.Pixel;
            Bounds = new Rectangle(x0, y0, x1 - x0, y1 - y0);
            Speed = speed;
            Points = 0;
            Direction = Direction.Left;
        }
        public Direction DesiredDirection { get; set; } = Direction.None;
        public int Points {  get; set; }

        public override void Draw()
        {
            //Raylib.DrawCircleV(new Vector2(Bounds.X+(Bounds.Width/2), Bounds.Y+(Bounds.Height/2)), (Board.Pixel/2), Color.Yellow);
            if (Direction == Direction.Left)
            {
                Raylib.DrawCircleSector(new Vector2(Bounds.X + (Bounds.Width / 2), Bounds.Y + (Bounds.Height / 2)), (Board.Pixel / 2), 225, 360, 1, Color.Yellow);
                Raylib.DrawCircleSector(new Vector2(Bounds.X + (Bounds.Width / 2), Bounds.Y + (Bounds.Height / 2)), (Board.Pixel / 2), 135, 0, 1, Color.Yellow);
            }
            if(Direction == Direction.Right)
                Raylib.DrawCircleSector(new Vector2(Bounds.X + (Bounds.Width / 2), Bounds.Y + (Bounds.Height / 2)), (Board.Pixel / 2), 45, 315, 1, Color.Yellow);
            if(Direction == Direction.Down)
                Raylib.DrawCircleSector(new Vector2(Bounds.X + (Bounds.Width / 2), Bounds.Y + (Bounds.Height / 2)), (Board.Pixel / 2), 135, 405, 1, Color.Yellow);
            if(Direction == Direction.Up)
                Raylib.DrawCircleSector(new Vector2(Bounds.X + (Bounds.Width / 2), Bounds.Y + (Bounds.Height / 2)), (Board.Pixel / 2), 225, -45, 1, Color.Yellow);
        }

        public void HandleInput()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Up))
                DesiredDirection = Direction.Up;
            if (Raylib.IsKeyPressed(KeyboardKey.Down))
                DesiredDirection = Direction.Down;
            if (Raylib.IsKeyPressed(KeyboardKey.Left))
                DesiredDirection = Direction.Left;
            if (Raylib.IsKeyPressed(KeyboardKey.Right))
                DesiredDirection = Direction.Right;
        }

        public void UpdatePoints(Maze maze)
        {
            if ((MazeCol > -1 && MazeCol < maze.Cells.GetLength(1))
                && (MazeRow > -1 && MazeRow < maze.Cells.GetLength(0)))
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
        }

        public void UpdateDirection(Maze maze)
        {
            bool update = false;
            float step = Speed * DeltaTime;
            bool centerX = MathF.Abs(Bounds.X - (Board.OffsetX + MazeCol * Board.Pixel)) <= step;
            bool centerY = MathF.Abs(Bounds.Y - (Board.OffsetY + MazeRow * Board.Pixel)) <= step;

            bool isReverse = (Direction == Direction.Left && DesiredDirection == Direction.Right)
               || (Direction == Direction.Right && DesiredDirection == Direction.Left)
               || (Direction == Direction.Up && DesiredDirection == Direction.Down)
               || (Direction == Direction.Down && DesiredDirection == Direction.Up);

            if (isReverse)
            {
                Direction = DesiredDirection;
                UpdateColRow();
                return;
            }

            if (Direction == Direction.Up && centerY)
                update = true;
            if (Direction == Direction.Left && centerX)
                update = true;
            if (Direction == Direction.Right && centerX)
                update = true;
            if (Direction == Direction.Down && centerY)
                update = true;


            if (update)
            {
                if (DesiredDirection == Direction.Up && Check(Direction.Up, maze, false))
                {
                    Direction = Direction.Up;
                    SetPositionX(Board.OffsetX + MazeCol * Board.Pixel);
                }
                if (DesiredDirection == Direction.Down && Check(Direction.Down, maze, false))
                {
                    Direction = Direction.Down;
                    SetPositionX(Board.OffsetX + MazeCol * Board.Pixel);
                }
                if (DesiredDirection == Direction.Left && Check(Direction.Left, maze, false))
                {
                    Direction = Direction.Left;
                    SetPositionY(Board.OffsetY + MazeRow * Board.Pixel);
                }
                if (DesiredDirection == Direction.Right && Check(Direction.Right, maze, false))
                {
                    Direction = Direction.Right;
                    SetPositionY(Board.OffsetY + MazeRow * Board.Pixel);
                }
            }
        }
    }
}
