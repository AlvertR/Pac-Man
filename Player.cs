using Raylib_cs;
using System.Numerics;

namespace Pac_Man
{
    public class Player : Entity
    {
        public Player(char key, float speed, Board board)
            : base(key, speed, board)
        {
            Points = 0;
            Lifes = 3;
            Direction = Direction.Left;
            EatedGhosts = 0;
        }
        public Direction DesiredDirection { get; set; } = Direction.None;
        public int Points {  get; set; }
        public int Lifes {  get; set; }
        public int EatedGhosts {  get; set; }

        public void Draw()
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

            for (int l = 0; l < Lifes; l++)
            {
                Raylib.DrawCircleSector(new Vector2(Board.OffsetX + ((l+1)*Board.Pixel) , Board.OffsetY + (Board.Pixel * (Board.Rows +1))), (Board.Pixel / 2), 45, 315, 1, Color.Yellow);
            }
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

        public bool UpdatePoints(Maze maze)
        {
            if ((MazeCol > -1 && MazeCol < maze.Cells.GetLength(1))
                && (MazeRow > -1 && MazeRow < maze.Cells.GetLength(0)))
            {
                var currentCell = maze.Cells[MazeRow, MazeCol];
                if (currentCell == CellType.Pellet)
                {
                    maze.Cells[MazeRow, MazeCol] = CellType.Path;
                    maze.TotalPellets--;
                    Points += 10;
                }
                if (currentCell == CellType.PowerPellet)
                {
                    maze.Cells[MazeRow, MazeCol] = CellType.Path;
                    maze.TotalPellets--;
                    Points += 50;
                    return true;
                }
            }
            return false;
        }

        public void UpdateDirection(Maze maze, float deltaTime)
        {
            bool update = false;
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

            if (Direction == Direction.Up && CenterY)
                update = true;
            if (Direction == Direction.Left && CenterX)
                update = true;
            if (Direction == Direction.Right && CenterX)
                update = true;
            if (Direction == Direction.Down && CenterY)
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
