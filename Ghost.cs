using Raylib_cs;
using System.Numerics;

namespace Pac_Man
{
    public class Ghost : Entity
    {
        public Ghost(char key, float speed, Board board, Color color, Direction dir, float timeToStartMove, bool outOfRoom = false, bool canUseDoor = false)
            : base(key, speed, board)
        {
            Color = color;
            Direction = dir;
            InitialDirection = dir;
            TimeToStartMove = timeToStartMove;
            OutRoom = outOfRoom;
            CanUseDoor = canUseDoor;
        }
        public Color Color { get; set; }
        public bool CanMove { get; set; } = false;
        public bool OutRoom { get; set; }
        public bool CanUseDoor { get; set; }
        public float TimeToStartMove { get; set; }
        public Direction InitialDirection { get; set; }

        public void Draw()
        {
            Raylib.DrawCircleSector(new Vector2(Bounds.X + (Bounds.Width / 2), Bounds.Y + (Bounds.Height / 2)), (Board.Pixel / 2), 180, 360, 1, Color);
            Raylib.DrawRectangleV(new Vector2(Bounds.X, Bounds.Y + (Bounds.Height / 2)), new Vector2(Bounds.Width, Bounds.Height / 2), Color);
        }

        public void StartMoveCounter(float deltaTime)
        {
            if (TimeToStartMove <= 0)
                CanMove = true;
            else
                TimeToStartMove -= deltaTime;
        }

        public void LeaveRoom()
        {
            var doorPosition = Board.GetInitialPosition('/');
            int doorCol = doorPosition[1];
            int doorRow = doorPosition[0];

            if (MazeCol < doorCol)
                Direction = Direction.Right;
            if (MazeCol == doorCol && MazeRow > doorRow - 1)
                Direction = Direction.Up;
            if (MazeCol == doorCol && MazeRow < doorRow)
            {
                OutRoom = true;
                Direction = InitialDirection;
            }

        }

        public void BaseMove(float deltaTime, Maze maze, Action GostLogicMove)
        {
            StartMoveCounter(deltaTime);
            if (CanMove)
            {
                if (!OutRoom)
                    LeaveRoom();
                else
                    GostLogicMove();

                Move(maze, true, deltaTime);
                UpdateColRow();
            }
        }
    }
}
