using Raylib_cs;
using System.Numerics;

namespace Pac_Man
{
    public class Maze
    {
        public string[] Sprite =
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
            "------.---#------#---.------",
            "######.##-#------#-##.######",
            "-----#.##-########-##.#-----",
            "-----#.##----------##.#-----",
            "-----#.##-########-##.#-----",
            "######.##-########-##.######",
            "#............##............#",
            "#.####.#####.##.#####.####.#",
            "#.####.#####.##.#####.####.#",
            "#o..##........-.......##..o#",
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
        public List<Cell> Walls {get; set;} = new List<Cell>();
        public List<Cell> Pellets {get; set;}= new List<Cell>();
        public List<Cell> PowerPellets {get; set;}= new List<Cell>();
        public List<Cell> Road {get; set;}= new List<Cell>();

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

                    if (cell == '#')
                    {
                        Color color = Color.Blue;
                        int x0 = (int)MathF.Floor(c * pixel);
                        int y0 = (int)MathF.Floor(offsetY + r * pixel);
                        int x1 = (int)MathF.Floor((c +1) * pixel);
                        int y1 = (int)MathF.Floor(offsetY + ( r +1) * pixel);

                        //Raylib.DrawRectangleV(new Vector2(x0, y0), new Vector2(pixel/2, y1-y0), color);
                        Raylib.DrawRectangleV(new Vector2(x0, y0), new Vector2(x1-x0, y1-y0), color);
                    }
                    if (cell == '/')
                    {
                        Color color = Color.Pink;
                        int x0 = (int)MathF.Floor(c * pixel);
                        int y0 = (int)MathF.Floor(offsetY + r * pixel);
                        int x1 = (int)MathF.Floor((c + 1) * pixel);
                        int y1 = (int)MathF.Floor(offsetY + (r + 1) * pixel);

                        //Raylib.DrawRectangleV(new Vector2(x0, y0), new Vector2(x1-x0, pixel/2), color);
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

            float pixel = MathF.Min(Width / cols, Height / rows);

            //float offsetX = (Width - pixel * cols) / 2f;
            float offsetY = (Height - pixel * rows) / 2f;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    char cell = Sprite[r][c];
                    if (cell == '-')
                    {
                        int x0 = (int)MathF.Floor(c * pixel);
                        int y0 = (int)MathF.Floor(offsetY + r * pixel);
                        int x1 = (int)MathF.Floor((c + 1) * pixel);
                        int y1 = (int)MathF.Floor(offsetY + (r + 1) * pixel);
                        AddRoad(x0, y0, x1 - x0, y1 - y0);
                    }
                    if (cell == '/') continue;

                    if (cell == '#')
                    {
                        int x0 = (int)MathF.Floor(c * pixel);
                        int y0 = (int)MathF.Floor(offsetY + r * pixel);
                        int x1 = (int)MathF.Floor((c + 1) * pixel);
                        int y1 = (int)MathF.Floor(offsetY + (r + 1) * pixel);

                        this.AddWall(x0, y0, x1 - x0, y1 - y0);
                    }
                    if (cell == '.')
                    {
                        float x0 = MathF.Floor(c * pixel) + pixel / 2;
                        float y0 = MathF.Floor(offsetY + r * pixel) + pixel / 2;
                        int x1 = (int)MathF.Floor((c + 1) * pixel);
                        int y1 = (int)MathF.Floor(offsetY + (r + 1) * pixel);

                        this.AddPellet(x0, y0, x1 - x0, y1 - y0);
                        AddRoad(x0, y0, x1 - x0, y1 - y0);
                    }
                    if (cell == 'o')
                    {
                        float x0 = MathF.Floor(c * pixel) + pixel / 2;
                        float y0 = MathF.Floor(offsetY + r * pixel) + pixel / 2;
                        int x1 = (int)MathF.Floor((c + 1) * pixel);
                        int y1 = (int)MathF.Floor(offsetY + (r + 1) * pixel);

                        this.AddPowerPellet(x0, y0, x1 - x0, y1 - y0);
                        AddRoad(x0, y0, x1 - x0, y1 - y0);
                    }
                }
            }
        }

        public void AddWall(float x, float y, float width, float height)
        {
            Cell newWall = new Cell(x, y, width, height, CellType.Wall);
            this.Walls.Add(newWall);
        }

        public void AddRoad(float x, float y, float width, float height)
        {
            Cell newWall = new Cell(x, y, width, height, CellType.Path);
            this.Road.Add(newWall);
        }

        public void AddPellet(float x, float y, float width, float height)
        {
            Cell newPellet = new Cell(x, y, width, height, CellType.Pellet);
            this.Pellets.Add(newPellet);
        }

        public void AddPowerPellet(float x, float y, float width, float height)
        {
            Cell newPellet = new Cell(x, y, width, height, CellType.PowerPellet);
            this.PowerPellets.Add(newPellet);
        }
    }

    public class Cell
    {
        public Cell(float x, float y, float width, float height, CellType type)
        {
            Bounds = new Rectangle(x, y, width, height);
            Type = type;
        }
        public Rectangle Bounds {get; set;}
        public CellType Type {get; set;}

        public void SetPath() 
            => Type = CellType.Path;
    }
}
