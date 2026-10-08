using Raylib_cs;
using System.Numerics;

namespace Pac_Man
{
    public class Maze
    {
        public string[] Sprite { get; set; } =
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
            "-----#.##----------##.#-----",
            "-----#.##-###//###-##.#-----",
            "######.##-#------#-##.######",
            "0-----.---#------#---.-----0",
            "######.##-#------#-##.######",
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
        public Maze(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public float Width {get; set;}
        public float Height {get; set;}
        public CellType[,]? Cells {get; set;}

        public void Draw()
        {
            int rows = Sprite.Length;
            int cols = Sprite[0].Length;

            float pixel = MathF.Min(Width / cols, Height / rows);

            //float offsetX = (Width - pixel * cols) / 2f;
            float offsetY = (Height - pixel * rows) / 2f;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    char cell = Sprite[r][c];
                    if (cell == '-') continue;
                    if (cell == '0') continue;
                    if (cell == 'p') continue;

                    if (cell == '#')
                    {
                        int x0 = (int)MathF.Floor(c * pixel);
                        int y0 = (int)MathF.Floor(offsetY + r * pixel);
                        int x1 = (int)MathF.Floor((c +1) * pixel);
                        int y1 = (int)MathF.Floor(offsetY + ( r +1) * pixel);

                        Raylib.DrawRectangleV(new Vector2(x0, y0), new Vector2(x1-x0, y1-y0), Color.Blue);
                    }
                    if (cell == '/')
                    {
                        Color color = Color.Pink;
                        int x0 = (int)MathF.Floor(c * pixel);
                        int y0 = (int)MathF.Floor(offsetY + r * pixel);
                        int x1 = (int)MathF.Floor((c + 1) * pixel);
                        int y1 = (int)MathF.Floor(offsetY + (r + 1) * pixel);

                        Raylib.DrawRectangleV(new Vector2(x0, y0), new Vector2(x1-x0, y1-y0), color);
                    }
                    if (cell == '.')
                    {
                        Color color = Color.Yellow;
                        float rad  = pixel / 5;
                        float x0 = MathF.Floor(c * pixel) + pixel/2;
                        float y0 = MathF.Floor(offsetY + r * pixel) + pixel/2;

                        Raylib.DrawCircleV(new Vector2(x0, y0), rad, color);
                    }
                    if (cell == 'o')
                    {
                        Color color = Color.Yellow;
                        float rad = pixel / 3;
                        float x0 = MathF.Floor(c * pixel) + pixel/2;
                        float y0 = MathF.Floor(offsetY + r * pixel) + pixel / 2;

                        Raylib.DrawCircleV(new Vector2(x0, y0), rad, color);
                    }
                }
            }
        }

        public void SetCells()
        {
            int rows = Sprite.Length;
            int cols = Sprite[0].Length;
            this.Cells = new CellType[rows, cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    char cell = Sprite[r][c];
                    if (cell == '-')
                        Cells[r, c] = CellType.Path;
                    if (cell == '/')
                        Cells[r, c] = CellType.Door;
                    if (cell == '0')
                        Cells[r, c] = CellType.Bridge;
                    if (cell == '#')
                        Cells[r, c] = CellType.Wall;
                    if (cell == '.')
                        Cells[r, c] = CellType.Pellet;
                    if (cell == 'o')
                        Cells[r, c] = CellType.PowerPellet;
                    if (cell == 'p')
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
}
