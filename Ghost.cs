using Raylib_cs;
using System.Numerics;

namespace Pac_Man
{
    public class Ghost : Entity
    {
        const float NormalSpeed = 80f;
        const float FrightSpeed = 50f;
        const float EyeSpeed = 200f;
        public Ghost(char key, Board board, Color color, Direction dir, float timeToStartMove, char cornerChar, bool outOfRoom = false, bool canUseDoor = false)
            : base(key, NormalSpeed, board)
        {
            Color = color;
            Direction = dir;
            InitialDirection = dir;
            TimeToStartMove = timeToStartMove;
            InitialTimeToStartMove = timeToStartMove;
            OutRoom = outOfRoom;
            CanUseDoor = canUseDoor;
            LastCell[0] = MazeRow;
            LastCell[1] = MazeCol;
            var doorPos = Board.GetInitialPosition('/');
            DoorPosition[0] = doorPos[0];
            DoorPosition[1] = doorPos[1];
            ScatterCorner = new ScatterCorner(Board.GetInitialPosition(cornerChar)[0], Board.GetInitialPosition(cornerChar)[1]);
            Status = GhostStatus.normal;
            TimeScared = 10f;
            TimeInScatter = 5f;
            TimeToScatter = 10f;
            SwitchSprite = false;
            IntervalScared = 0f;
        }
        public Color Color { get; set; }
        public bool CanMove { get; set; } = false;
        public bool OutRoom { get; set; }
        public bool CanUseDoor { get; set; }
        public float TimeToStartMove { get; set; }
        public float InitialTimeToStartMove { get; set; }
        public float TimeScared { get; set; }
        public float TimeInScatter { get; set; }
        public float TimeToScatter { get; set; }
        public float IntervalScared { get; set; }
        public Direction InitialDirection { get; set; }
        public int[] LastCell {  get; set; } = new int[2];
        public int[] DoorPosition {  get; set; } = new int[2];
        public ScatterCorner ScatterCorner {  get; set; }
        public GhostStatus Status { get; set; }
        public bool SwitchSprite { get; set; }

        public void Draw()
        {
            if (Status == GhostStatus.normal || Status == GhostStatus.scatter)
            {
                Raylib.DrawCircleSector(new Vector2(Bounds.X + (Bounds.Width / 2), Bounds.Y + (Bounds.Height / 2)), (Board.Pixel / 2), 180, 360, 1, Color);
                Raylib.DrawRectangleV(new Vector2(Bounds.X, Bounds.Y + (Bounds.Height / 2)), new Vector2(Bounds.Width, Bounds.Height / 2), Color);

                Raylib.DrawRectangleV(new Vector2(Bounds.X + (Bounds.Width / 6), Bounds.Y + (Bounds.Height / 4)), new Vector2(Bounds.Width / 8, Bounds.Height / 8), Color.Black);
                Raylib.DrawRectangleV(new Vector2(Bounds.X + (Bounds.Width / 2), Bounds.Y + (Bounds.Height / 4)), new Vector2(Bounds.Width / 8, Bounds.Height / 8), Color.Black);

                Raylib.DrawLineV(new Vector2(Bounds.X + (Bounds.Width / 6), Bounds.Y + (Bounds.Height / 5) * 3), new Vector2(Bounds.X + (Bounds.Width / 3) * 2, Bounds.Y + (Bounds.Height / 5) * 3), Color.Black);
            }
            else if (Status == GhostStatus.scared)
            {
                if (!SwitchSprite)
                {
                    Raylib.DrawCircleSector(new Vector2(Bounds.X + (Bounds.Width / 2), Bounds.Y + (Bounds.Height / 2)), (Board.Pixel / 2), 180, 360, 1, Color.DarkBlue);
                    Raylib.DrawRectangleV(new Vector2(Bounds.X, Bounds.Y + (Bounds.Height / 2)), new Vector2(Bounds.Width, Bounds.Height / 2), Color.DarkBlue);

                    Raylib.DrawRectangleV(new Vector2(Bounds.X + (Bounds.Width / 6), Bounds.Y + (Bounds.Height / 4)), new Vector2(Bounds.Width / 8, Bounds.Height / 8), Color.Yellow);
                    Raylib.DrawRectangleV(new Vector2(Bounds.X + (Bounds.Width / 2), Bounds.Y + (Bounds.Height / 4)), new Vector2(Bounds.Width / 8, Bounds.Height / 8), Color.Yellow);

                    Raylib.DrawLineV(new Vector2(Bounds.X + (Bounds.Width / 6), Bounds.Y + (Bounds.Height / 5) * 3), new Vector2(Bounds.X + (Bounds.Width / 3) * 2, Bounds.Y + (Bounds.Height / 5) * 3), Color.Yellow);
                }
                else
                {
                    Raylib.DrawCircleSector(new Vector2(Bounds.X + (Bounds.Width / 2), Bounds.Y + (Bounds.Height / 2)), (Board.Pixel / 2), 180, 360, 1, Color.White);
                    Raylib.DrawRectangleV(new Vector2(Bounds.X, Bounds.Y + (Bounds.Height / 2)), new Vector2(Bounds.Width, Bounds.Height / 2), Color.White);

                    Raylib.DrawRectangleV(new Vector2(Bounds.X + (Bounds.Width / 6), Bounds.Y + (Bounds.Height / 4)), new Vector2(Bounds.Width / 8, Bounds.Height / 8), Color.Black);
                    Raylib.DrawRectangleV(new Vector2(Bounds.X + (Bounds.Width / 2), Bounds.Y + (Bounds.Height / 4)), new Vector2(Bounds.Width / 8, Bounds.Height / 8), Color.Black);

                    Raylib.DrawLineV(new Vector2(Bounds.X + (Bounds.Width / 6), Bounds.Y + (Bounds.Height / 5) * 3), new Vector2(Bounds.X + (Bounds.Width / 3) * 2, Bounds.Y + (Bounds.Height / 5) * 3), Color.Black);
                }
            }
            else
            {
                Raylib.DrawRectangleV(new Vector2(Bounds.X + (Bounds.Width / 6), Bounds.Y + (Bounds.Height / 4)), new Vector2(Bounds.Width / 8, Bounds.Height / 8), Color.White);
                Raylib.DrawRectangleV(new Vector2(Bounds.X + (Bounds.Width / 2), Bounds.Y + (Bounds.Height / 4)), new Vector2(Bounds.Width / 8, Bounds.Height / 8), Color.White);
            }
        }

        public void SetScared()
        { 
            Status = GhostStatus.scared;
            TimeScared = 10f;
            Speed = FrightSpeed;
            Direction = GetReverse(Direction);
            LastCell[0] = -1;
            LastCell[1] = -1;
            IntervalScared = 0f;
            SwitchSprite = false;
        }

        public void SetScatter()
        {
            Status = GhostStatus.scatter;
            Direction = GetReverse(Direction);
            LastCell[0] = -1;
            LastCell[1] = -1;
            TimeInScatter = 5f;
        }
        public void SetNormal()
        { 
            Status = GhostStatus.normal;
            TimeScared = 10f;
            Speed = NormalSpeed;
            IntervalScared = 0f;
            SwitchSprite = false;
            TimeToScatter = 10f;
        }

        public void SetDead()
        {
            Status = GhostStatus.dead;
            Speed = EyeSpeed;
            CanUseDoor = true;
        }

        public void Reset( bool outOfRoom = false, bool canUseDoor = true)
        {
            ResetPosition();
            Revive(InitialTimeToStartMove, false, outOfRoom, canUseDoor);
        }

        public void Revive(float timeToStartMove, bool canMove, bool outOfRoom = false, bool canUseDoor = true)
        {
            SetNormal();
            Direction = InitialDirection;
            TimeToStartMove = timeToStartMove;
            OutRoom = outOfRoom;
            CanUseDoor = canUseDoor;
            LastCell[0] = MazeRow;
            LastCell[1] = MazeCol;
            CanMove = canMove;
        }
        public void StartMoveCounter(float deltaTime)
        {
            if (TimeToStartMove <= 0)
                CanMove = true;
            else
                TimeToStartMove -= deltaTime;
        }

        public void CheckScaredCounter(float deltaTime)
        {
            if (TimeScared <= 0)
            {
                SetNormal();
            }
            else
            {
                var change = TimeScared;
                TimeScared -= deltaTime;
                if (TimeScared < 2)
                {
                    if (IntervalScared >= 0.2f)
                    {
                        SwitchSprite = !SwitchSprite;
                        IntervalScared -= 0.2f;
                    }
                    else
                        IntervalScared += deltaTime;
                }
            }
        }

        public void CheckInScatteCounter(float deltaTime)
        {
            if (Status == GhostStatus.scatter)
            {
                if (TimeInScatter <= 0)
                {
                    SetNormal();
                }
                else
                {
                    TimeInScatter -= deltaTime;
                }
            }
        }

        public void CheckToBeScatteCounter(float deltaTime)
        {
            if (Status == GhostStatus.normal)
            {
                if (TimeToScatter <= 0)
                {
                    SetScatter();
                }
                else
                {
                    TimeToScatter -= deltaTime;
                }
            }
        }

        public bool isInitialCell(int row, int col)
        {
            if(MazeRow == row && MazeCol == col) 
                return true;
            return false;
        }

        public void LeaveRoom()
        {
            if (CanUseDoor)
            {
                int doorCol = DoorPosition[1];
                int doorRow = DoorPosition[0];

                if (MazeCol < doorCol)
                {
                    Direction = Direction.Right;
                    SetPositionY(Board.OffsetY + MazeRow * Board.Pixel);
                }
                else if (MazeCol == doorCol && CenterY && MazeRow > doorRow - 1)
                {
                    Direction = Direction.Up;
                    SetPositionX(Board.OffsetX + MazeCol * Board.Pixel);
                }
                else if (MazeCol == doorCol && CenterX && MazeRow < doorRow)
                {
                    OutRoom = true;
                    CanUseDoor = false;
                    Direction = InitialDirection;
                    SetPositionX(Board.OffsetX + MazeCol * Board.Pixel);
                    SetPositionY(Board.OffsetY + MazeRow * Board.Pixel);
                }
            }
        }

        public void BaseMove(float deltaTime, Maze maze, int targetRow, int targetCol, Random rnd)
        {
            StartMoveCounter(deltaTime);
            if (CanMove)
            {
                if (!OutRoom)
                    LeaveRoom();
                else
                {
                    if (Status == GhostStatus.dead)
                    {
                        var basePos = Board.GetInitialPosition(Key);
                        if (isInitialCell(basePos[0], basePos[1]))
                        {
                            Revive(0f, true);
                        }
                        else
                            ChooseDirection(maze, basePos[0], basePos[1]);
                    }
                    else if (Status == GhostStatus.scared)
                    {
                        int row = rnd.Next(maze.Cells.GetLength(0));
                        int col = rnd.Next(maze.Cells.GetLength(1));
                        ChooseDirection(maze, row, col);
                    }
                    else if(Status == GhostStatus.scatter)
                    {
                        ChooseDirection(maze, ScatterCorner.Row, ScatterCorner.Col);
                    }
                    else
                    {
                        CheckToBeScatteCounter(deltaTime);
                        ChooseDirection(maze, targetRow, targetCol);
                    }
                }
                Move(maze, CanUseDoor, deltaTime);
                UpdateColRow();
            }
            if (Status == GhostStatus.scared)
                CheckScaredCounter(deltaTime);
            if (Status == GhostStatus.scatter)
                CheckInScatteCounter(deltaTime);
        }

        public (int row, int col) GetTarget(Entity entity, int n)
        {
            int mazeCol = entity.MazeCol;
            int mazeRow = entity.MazeRow;

            if (entity.Direction == Direction.Up)
                mazeRow = mazeRow - n;
            if (entity.Direction == Direction.Down)
                mazeRow = mazeRow + n;
            if (entity.Direction == Direction.Left)
                mazeCol = mazeCol - n;
            if (entity.Direction == Direction.Right)
                mazeCol = mazeCol + n;

            return (mazeRow, mazeCol);
        }

        public void ChooseDirection(Maze maze, int targetRow, int targetCol)
        {
            bool isLastCell = LastCell[0] == MazeRow && LastCell[1] == MazeCol;
            if (isLastCell || !(CenterX && CenterY))
                return;

            int dUp = int.MaxValue;
            int dDown = int.MaxValue;
            int dLeft = int.MaxValue;
            int dRight = int.MaxValue;
            int dx = 0;
            int dy = 0;
            Direction dirMenor = Direction.None;
            int menor = int.MaxValue;

            if (Check(Direction.Up, maze, CanUseDoor) && Direction != Direction.Down)
            {
                dx = MazeCol - targetCol;
                dy = MazeRow - 1 - targetRow;
                dUp = dx * dx + dy * dy;
            }
            if (Check(Direction.Down, maze, CanUseDoor) && Direction != Direction.Up)
            {
                dx = MazeCol - targetCol;
                dy = MazeRow + 1 - targetRow;
                dDown = dx * dx + dy * dy;
            }
            if (Check(Direction.Left, maze, CanUseDoor) && Direction != Direction.Right)
            {
                dx = MazeCol - 1 - targetCol;
                dy = MazeRow - targetRow;
                dLeft = dx * dx + dy * dy;
            }
            if (Check(Direction.Right, maze, CanUseDoor) && Direction != Direction.Left)
            {
                dx = MazeCol + 1 - targetCol;
                dy = MazeRow - targetRow;
                dRight = dx * dx + dy * dy;
            }

            if (dUp < menor)
            {
                menor = dUp;
                dirMenor = Direction.Up;
            }
            if (dLeft < menor)
            {
                menor = dLeft;
                dirMenor = Direction.Left;
            }
            if (dDown < menor)
            {
                menor = dDown;
                dirMenor = Direction.Down;
            }
            if (dRight < menor)
            {
                menor = dRight;
                dirMenor = Direction.Right;
            }

            if (dirMenor == Direction.None)
                dirMenor = GetReverse(Direction);
            
            switch (dirMenor)
            {
                case Direction.Up:
                    Direction = Direction.Up;
                    SetPositionX(Board.OffsetX + MazeCol * Board.Pixel);
                    break;
                case Direction.Left:
                    Direction = Direction.Left;
                    SetPositionY(Board.OffsetY + MazeRow * Board.Pixel);
                    break;
                case Direction.Right:
                    Direction = Direction.Right;
                    SetPositionY(Board.OffsetY + MazeRow * Board.Pixel);
                    break;
                case Direction.Down:
                    Direction = Direction.Down;
                    SetPositionX(Board.OffsetX + MazeCol * Board.Pixel);
                    break;
                default:
                    break;
            }

            LastCell[0] = MazeRow;
            LastCell[1] = MazeCol;
        }
    }

    public class ScatterCorner
    {
        public ScatterCorner(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public int Row {  get; set; }
        public int Col {  get; set; }
    }
}
