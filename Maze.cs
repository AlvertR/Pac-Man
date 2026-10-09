using Raylib_cs;
using System.Numerics;

namespace Pac_Man
{
    public class Maze
    {
        public Maze(Sprite sprite)
        {
            Sprite = sprite;
            Cells = new CellType[Sprite.Rows, Sprite.Cols];
            SetCells();
        }

        public CellType[,] Cells {get; set;}
        public Sprite Sprite {get; set;}

        public void Draw()
        {
            int rows = Sprite.Rows;
            int cols = Sprite.Cols;
            
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    char cell = Sprite.Grid[r][c];
                    if (cell == '-' 
                        || cell == '0' 
                        || cell == 'p' 
                        || cell == 'b' 
                        || cell == 'i' 
                        || cell == 'r' 
                        || cell == 'c') continue;

                    if (cell == '#')
                    {
                        int x0 = (int)MathF.Floor(c * Sprite.Pixel);
                        int y0 = (int)MathF.Floor(Sprite.OffsetY + r * Sprite.Pixel);
                        int x1 = (int)MathF.Floor((c +1) * Sprite.Pixel);
                        int y1 = (int)MathF.Floor(Sprite.OffsetY + ( r +1) * Sprite.Pixel);

                        Raylib.DrawRectangleV(new Vector2(x0, y0), new Vector2(x1-x0, y1-y0), Color.Blue);
                    }
                    if (cell == '/')
                    {
                        Color color = Color.Pink;
                        int x0 = (int)MathF.Floor(c * Sprite.Pixel);
                        int y0 = (int)MathF.Floor(Sprite.OffsetY + r * Sprite.Pixel);
                        int x1 = (int)MathF.Floor((c + 1) * Sprite.Pixel);
                        int y1 = (int)MathF.Floor(Sprite.OffsetY + (r + 1) * Sprite.Pixel);

                        Raylib.DrawRectangleV(new Vector2(x0, y0), new Vector2(x1-x0, y1-y0), color);
                    }
                    if (cell == '.')
                    {
                        CellType currentPellet = Cells[r, c];
                        if (currentPellet == CellType.Pellet)
                        {
                            Color color = Color.Yellow;
                            float rad = Sprite.Pixel / 5;
                            float x0 = MathF.Floor(c * Sprite.Pixel) + Sprite.Pixel / 2;
                            float y0 = MathF.Floor(Sprite.OffsetY + r * Sprite.Pixel) + Sprite.Pixel / 2;

                            Raylib.DrawCircleV(new Vector2(x0, y0), rad, color);
                        }
                    }
                    if (cell == 'o')
                    {
                        CellType currentPellet = Cells[r, c];
                        if (currentPellet == CellType.PowerPellet)
                        {
                            Color color = Color.Yellow;
                            float rad = Sprite.Pixel / 3;
                            float x0 = MathF.Floor(c * Sprite.Pixel) + Sprite.Pixel / 2;
                            float y0 = MathF.Floor(Sprite.OffsetY + r * Sprite.Pixel) + Sprite.Pixel / 2;

                            Raylib.DrawCircleV(new Vector2(x0, y0), rad, color);
                        }
                    }
                }
            }
        }

        public void SetCells()
        {
            int rows = Sprite.Rows;
            int cols = Sprite.Cols;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    char cell = Sprite.Grid[r][c];
                    if (cell == '-')
                        Cells[r, c] = CellType.Path;
                    if (cell == '/')
                        Cells[r, c] = CellType.Door;
                    if (cell == '0')
                        Cells[r, c] = CellType.Bridge;
                    if (cell == '#')
                        Cells[r, c] = CellType.Wall;
                    if (cell == '.' || cell == 'b' || cell == 'p')
                        Cells[r, c] = CellType.Pellet;
                    if (cell == 'o')
                        Cells[r, c] = CellType.PowerPellet;
                    if (cell == 'i' || cell == 'r' || cell == 'c')
                        Cells[r, c] = CellType.Path;
                }
            }
        }

        public bool CanMove(int row, int col)
        {
            if(row < 0 || row >= Cells.GetLength(0))
                return false;

            if (col < 0 || col >= Cells.GetLength(1))
                return false;

            return Cells[row, col] != CellType.Wall && Cells[row, col] != CellType.Door;
        }
    }

    public class Sprite
    {
        public Sprite(float screenWidth, float screenHeight) 
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
