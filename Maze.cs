using Raylib_cs;
using System.Numerics;

namespace Pac_Man
{
    public class Maze
    {
        public Maze(Board board)
        {
            Board = board;
            Cells = new CellType[Board.Rows, Board.Cols];
            SetCells();
        }

        public CellType[,] Cells {get; set;}
        public Board Board {get; set;}


        public void Draw()
        {
            int rows = Cells.GetLength(0);
            int cols = Cells.GetLength(1);
            
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    CellType cell = Cells[r,c];
                    switch (cell)
                    {
                        case CellType.Wall:
                            int x0 = (int)MathF.Floor(Board.OffsetX + c * Board.Pixel);
                            int y0 = (int)MathF.Floor(Board.OffsetY + r * Board.Pixel);
                            int x1 = (int)MathF.Floor(Board.OffsetX + (c + 1) * Board.Pixel);
                            int y1 = (int)MathF.Floor(Board.OffsetY + (r + 1) * Board.Pixel);
                            Raylib.DrawRectangleV(new Vector2(x0, y0), new Vector2(x1 - x0, y1 - y0), Color.Blue);
                            break;
                        case CellType.Pellet:
                                float radP = Board.Pixel / 5;
                                float x0P = MathF.Floor(Board.OffsetX + c * Board.Pixel) + Board.Pixel / 2;
                                float y0P = MathF.Floor(Board.OffsetY + r * Board.Pixel) + Board.Pixel / 2;
                                Raylib.DrawCircleV(new Vector2(x0P, y0P), radP, Color.Yellow);
                            break;
                        case CellType.PowerPellet:
                            float radPp = Board.Pixel / 3;
                            float x0Pp = MathF.Floor(Board.OffsetX + c * Board.Pixel) + Board.Pixel / 2;
                            float y0Pp = MathF.Floor(Board.OffsetY + r * Board.Pixel) + Board.Pixel / 2;
                            Raylib.DrawCircleV(new Vector2(x0Pp, y0Pp), radPp, Color.Yellow);
                            break;
                        case CellType.Door:
                        case CellType.Path:
                        case CellType.Bridge:
                        default:
                            continue;
                    }
                }
            }
        }

        public void SetCells()
        {
            int rows = Board.Rows;
            int cols = Board.Cols;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    char cell = Board.Grid[r][c];
                    switch (cell)
                    {
                        case '-':
                            Cells[r, c] = CellType.Path;
                            break;
                        case '/':
                            Cells[r, c] = CellType.Door;
                            break;
                        case '0':
                            Cells[r, c] = CellType.Bridge;
                            break;
                        case '#':
                            Cells[r, c] = CellType.Wall;
                            break;
                        case '.':
                        case 'p':
                            Cells[r, c] = CellType.Pellet;
                            break;
                        case 'o':
                            Cells[r, c] = CellType.PowerPellet;
                            break;
                        case 'b':
                        case 'i':
                        case 'r':
                        case 'c':
                        default:
                            Cells[r, c] = CellType.Path;
                            break;
                    }
                }
            }
        }

        public bool CanMove(int row, int col, bool canUseDoor)
        {
            if (row < 0 || row >= Cells.GetLength(0))
                return false;
            if (col < 0 || col >= Cells.GetLength(1))
                return false;

            if (!canUseDoor)
                return Cells[row, col] != CellType.Wall && Cells[row, col] != CellType.Door;
            else
                return Cells[row, col] != CellType.Wall;
        }
    }

    public class Board
    {
        public Board(float screenWidth, float screenHeight) 
        {
            Rows = Grid.Length;
            Cols = Grid[0].Length;
            Pixel = MathF.Min(screenWidth / Cols, screenHeight / Rows);
            OffsetY = (screenHeight - Pixel * Rows) / 2f;
            OffsetX = (screenWidth - Pixel * Cols) / 2f;
        }
        public string[] Grid { get; set; } =
        {
            "############################",
            "#............##............#",
            "#.####.#####.##.#####.####.#",
            "#o#--#.#---#.##.#---#.#--#o#",
            "#.####.#####.##.#####.####.#",
            "#..........................#",
            "#.####.##.########.##.####.#",
            "#.####.##.########.##.####.#",
            "#......##....##....##......#",
            "######.#####-##-#####.######",
            "-----#.#####-##-#####.#-----",
            "-----#.##-----b----##.#-----",
            "-----#.##-###//###-##.#-----",
            "######.##-#--i---#-##.######",
            "0-----.---#--r---#---.-----0",
            "######.##-#--c---#-##.######",
            "-----#.##-########-##.#-----",
            "-----#.##----------##.#-----",
            "-----#.##-########-##.#-----",
            "######.##-########-##.######",
            "#............##............#",
            "#.####.#####.##.#####.####.#",
            "#.####.#####.##.#####.####.#",
            "#o..##........p.......##..o#",
            "###.##.##.########.##.##.###",
            "###.##.##.########.##.##.###",
            "#......##....##....##......#",
            "#.##########.##.##########.#",
            "#.##########.##.##########.#",
            "#..........................#",
            "############################",
        };

        public float Pixel {  get; set; }
        public int Cols {  get; set; }
        public int Rows {  get; set; }
        public float OffsetY { get; set; }
        public float OffsetX { get; set; }

        public int[] GetPlayerInitialPos()
            => GetInitialPosition('p');

        public int[] GetInitialPosition(char key)
        {
            int[] pos = new int[2];
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    char cell = Grid[r][c];
                    if (cell == key)
                    {
                        pos[0] = r;
                        pos[1] = c;
                        return pos;
                    }
                }
            }
            return pos;
        }
    }
}
