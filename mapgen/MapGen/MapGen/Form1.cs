using Microsoft.VisualBasic.Devices;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;

namespace MapGen
{
    public partial class Form1: Form {

        Bitmap image = new Bitmap(1, 1);

        public Form1() {
            InitializeComponent();
        }

        private void btn_Generate_Click(object sender, EventArgs e) {

            int seed = (int)num_Seed.Value;
            Random rnd = new Random(seed);
            FastNoiseLite noise = new FastNoiseLite(seed);

            int width = (int)num_SizeX.Value;
            int height = (int)num_SizeY.Value;
            float threshold = (float)num_Threshold.Value;
            int endPoints = (int)num_EndPoints.Value;

            image = new Bitmap(width, height);

            Pathfind pathfind = new Pathfind(width, height, seed, threshold, endPoints);

            DisplayCells(pathfind.cells);

            pbx_Map.Invalidate();

        }

        public void DisplayCells(Cell[,] cells) {
            for (int x = 0; x < cells.GetLength(0); x++) {
                for (int y = 0; y < cells.GetLength(1); y++) {
                    Color color = Color.Black;
                    if (cells[x, y].Type == CellType.Wall)
                        color = Color.LightGray;
                    if (cells[x, y].Status == AStar.Closed)
                        color = Color.Orange;
                    if (cells[x, y].Status == AStar.Open)
                        color = Color.LimeGreen;
                    if (cells[x, y].Direction != Dir.None)
                        color = Color.Purple;
                    if (cells[x, y].Type == CellType.Start)
                        color = Color.Green;
                    if (cells[x, y].Type == CellType.End)
                        color = Color.Red;
                    image.SetPixel(x, y, color);
                }
            }
        }

        // Display stuff
        private void pbx_Map_Paint(object sender, PaintEventArgs e) {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            e.Graphics.DrawImage(
                image,
                new Rectangle(0, 0, pbx_Map.Width, pbx_Map.Height),   // destination (scaled up)
                new Rectangle(0, 0, image.Width, image.Height), // source (original size)
                GraphicsUnit.Pixel
            );
        }
    }

    public enum AStar {
        Unchecked = 0,
        Open = 1,
        Closed = 2
    }

    public enum Dir {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3,
        None = 4
    }

    public enum CellType {
        None = 0,
        Wall = 1,
        Start = 2,
        End = 3
    }

    public class Cell {
        /// <summary>
        /// Whether or not the cell is a wall
        /// </summary>
        public CellType Type;
        /// <summary>
        /// Total cost
        /// </summary>
        public float FCost = float.MaxValue;
        /// <summary>
        /// Cost to start
        /// </summary>
        public float GCost = float.MaxValue;
        /// <summary>
        /// Cost to end
        /// </summary>
        public float HCost = float.MaxValue;
        /// <summary>
        /// A* status of the cell
        /// </summary>
        public AStar Status = AStar.Unchecked;
        /// <summary>
        /// Direction to the next cell
        /// </summary>
        public Dir Direction = Dir.None;

        public Cell(CellType type) {
            Type = type;
        }
    }

    public class Pathfind {
        public Cell[,] cells;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width">Width of the map</param>
        /// <param name="height">Height of the map</param>
        /// <param name="seed">Random seed</param>
        /// <param name="threshold">Noise threshold <para/>-1 means no walls, 1 means 100% walls, -0.5 is a good value</param>
        /// <param name="endPointCount"></param>
        public Pathfind(int width, int height, int seed, float threshold, int endPointCount) {

            Stopwatch sw = Stopwatch.StartNew();

            // Setup
            FastNoiseLite noise = new FastNoiseLite(seed);
            Random rnd = new Random(seed);
            cells = new Cell[width, height];

            // Create walls
            for(int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    if(noise.GetNoise((float)x * 12f, (float)y * 12f) < threshold) {
                        cells[x, y] = new Cell(CellType.Wall);
                    } else {
                        cells[x, y] = new Cell(CellType.None);
                    }
                }
            }

            Debug.WriteLine($"Make noise - {sw.Elapsed.TotalMicroseconds} us");
            sw.Restart();

            // Create starting position
            Point start;
            do {
                start = new Point(rnd.Next(width), rnd.Next(height));
            } while (cells[start.X, start.Y].Type == CellType.Wall);
            cells[start.X, start.Y].Type = CellType.Start;

            Debug.WriteLine($"Start pos - {sw.Elapsed.TotalMicroseconds} us");
            sw.Restart();

            // Create End positions
            List<Point> ends = new List<Point>();
            int iterations = 0;
            for (int p = 0; p < endPointCount; p++) {
                ends.Add(new Point());
                int distance;
                iterations = 0;
                do {
                    iterations++;
                    ends[ends.Count - 1] = new Point(rnd.Next(width), rnd.Next(height));
                    distance = Math.Abs(ends[ends.Count - 1].X - start.X) + Math.Abs(ends[ends.Count - 1].Y - start.Y);
                    if (iterations == 5000) {
                        break;
                    }
                } while (cells[ends[ends.Count - 1].X, ends[ends.Count - 1].Y].Type == CellType.Wall || distance < (width + height) * 0.3f);
            }

            Debug.WriteLine($"End pos - {sw.Elapsed.TotalMicroseconds} us");
            sw.Restart();

            if (iterations != 5000) {

                foreach (Point p in ends) {
                    cells[p.X, p.Y].Type = CellType.End;
                }
                bool pathExists = false;
                // Pathfind to end points
                for (int e = 0; e < endPointCount; e++) {
                    
                    cells[start.X, start.Y].Status = AStar.Open;
                    cells[start.X, start.Y].GCost = 0;
                    cells[start.X, start.Y].HCost = Vector2.Distance(new Vector2(start.X, start.Y), new Vector2(ends[e].X, ends[e].Y));
                    cells[start.X, start.Y].FCost = cells[start.X, start.Y].GCost + cells[start.X, start.Y].HCost;
                    bool solved = false;
                    HashSet<(int X, int Y, Cell cell)> OpenCells = new HashSet<(int X, int Y, Cell cell)>();
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            if (cells[x, y].Status == AStar.Open) {
                                OpenCells.Add((x, y, cells[x, y]));
                            }
                        }
                    }
                    while (!solved) {
                        
                        var temp = OpenCells.OrderBy(x => x.cell.FCost).ThenBy(x => x.cell.HCost);
                        if(temp.Count() == 0)
                            goto BreakOut;

                        var c = temp.First();
                        if (c.cell.Type == CellType.End) {
                            solved = true;
                        } else {
                            c.cell.Status = AStar.Closed;
                            OpenCells.Remove(c);
                            if (c.X - 1 >= 0 && cells[c.X - 1, c.Y].Type != CellType.Wall) {
                                AStar status = (AStar)Math.Max((int)cells[c.X - 1, c.Y].Status, (int)AStar.Open);
                                cells[c.X - 1, c.Y].Status = status;
                                if(status == AStar.Open) {
                                    OpenCells.Add((c.X - 1, c.Y, cells[c.X - 1, c.Y]));
                                }
                                cells[c.X - 1, c.Y].GCost = Math.Min(cells[c.X - 1, c.Y].GCost, cells[c.X, c.Y].GCost + 1);
                                cells[c.X - 1, c.Y].HCost = Vector2.Distance(new Vector2(c.X - 1, c.Y), new Vector2(ends[e].X, ends[e].Y));
                                cells[c.X - 1, c.Y].FCost = cells[c.X - 1, c.Y].GCost + cells[c.X - 1, c.Y].HCost;
                            }
                            if (c.X + 1 < cells.GetLength(0) && cells[c.X + 1, c.Y].Type != CellType.Wall) {
                                AStar status = (AStar)Math.Max((int)cells[c.X + 1, c.Y].Status, (int)AStar.Open);
                                cells[c.X + 1, c.Y].Status = status;
                                if (status == AStar.Open) {
                                    OpenCells.Add((c.X + 1, c.Y, cells[c.X + 1, c.Y]));
                                }
                                cells[c.X + 1, c.Y].GCost = Math.Min(cells[c.X + 1, c.Y].GCost, cells[c.X, c.Y].GCost + 1);
                                cells[c.X + 1, c.Y].HCost = Vector2.Distance(new Vector2(c.X + 1, c.Y), new Vector2(ends[e].X, ends[e].Y));
                                cells[c.X + 1, c.Y].FCost = cells[c.X + 1, c.Y].GCost + cells[c.X + 1, c.Y].HCost;
                            }
                            if (c.Y - 1 >= 0 && cells[c.X, c.Y - 1].Type != CellType.Wall) {
                                AStar status = (AStar)Math.Max((int)cells[c.X, c.Y - 1].Status, (int)AStar.Open);
                                cells[c.X, c.Y - 1].Status = status;
                                if (status == AStar.Open) {
                                    OpenCells.Add((c.X, c.Y - 1, cells[c.X, c.Y - 1]));
                                }
                                cells[c.X, c.Y - 1].GCost = Math.Min(cells[c.X, c.Y - 1].GCost, cells[c.X, c.Y].GCost + 1);
                                cells[c.X, c.Y - 1].HCost = Vector2.Distance(new Vector2(c.X, c.Y - 1), new Vector2(ends[e].X, ends[e].Y));
                                cells[c.X, c.Y - 1].FCost = cells[c.X, c.Y - 1].GCost + cells[c.X, c.Y - 1].HCost;
                            }
                            if (c.Y + 1 < cells.GetLength(1) && cells[c.X, c.Y + 1].Type != CellType.Wall) {
                                AStar status = (AStar)Math.Max((int)cells[c.X, c.Y + 1].Status, (int)AStar.Open);
                                cells[c.X, c.Y + 1].Status = status;
                                if (status == AStar.Open) {
                                    OpenCells.Add((c.X, c.Y + 1, cells[c.X, c.Y + 1]));
                                }
                                cells[c.X, c.Y + 1].GCost = Math.Min(cells[c.X, c.Y + 1].GCost, cells[c.X, c.Y].GCost + 1);
                                cells[c.X, c.Y + 1].HCost = Vector2.Distance(new Vector2(c.X, c.Y + 1), new Vector2(ends[e].X, ends[e].Y));
                                cells[c.X, c.Y + 1].FCost = cells[c.X, c.Y + 1].GCost + cells[c.X, c.Y + 1].HCost;
                            }
                        }
                    }
                    
                    Debug.WriteLine($"Pathfind to end - {sw.Elapsed.TotalMicroseconds} us");
                    sw.Restart();

                    // Trace path backwards
                    bool pathFinished = false;
                    Point p = ends[e];
                    while (!pathFinished) {
                        List<(int X, int Y, Cell cell)> temp = new List<(int X, int Y, Cell cell)>();
                        if (p.X - 1 >= 0 && cells[p.X - 1, p.Y].Type != CellType.Wall) {
                            temp.Add((p.X - 1, p.Y, cells[p.X - 1, p.Y]));
                        }
                        if (p.X + 1 <= cells.GetLength(0) && cells[p.X + 1, p.Y].Type != CellType.Wall) {
                            temp.Add((p.X + 1, p.Y, cells[p.X + 1, p.Y]));
                        }
                        if (p.Y - 1 >= 0 && cells[p.X, p.Y - 1].Type != CellType.Wall) {
                            temp.Add((p.X, p.Y - 1, cells[p.X, p.Y - 1]));
                        }
                        if (p.Y + 1 <= cells.GetLength(1) && cells[p.X, p.Y + 1].Type != CellType.Wall) {
                            temp.Add((p.X, p.Y + 1, cells[p.X, p.Y + 1]));
                        }
                        var next = temp.OrderBy(x => x.cell.GCost).First();

                        if (next.X == p.X - 1)
                            next.cell.Direction = Dir.Right;
                        if (next.X == p.X + 1)
                            next.cell.Direction = Dir.Left;
                        if (next.Y == p.Y - 1)
                            next.cell.Direction = Dir.Up;
                        if (next.Y == p.Y + 1)
                            next.cell.Direction = Dir.Down;

                        p = new Point(next.X, next.Y);

                        if (p == start)
                            pathFinished = true;
                    }

                }
                pathExists = true;
                BreakOut:;
                if (pathExists == false) {
                    // There is no path to the end point
                    Debug.WriteLine("No path exists, lower the threshold");
                }
                
            } else {
                // Cannot generate End points far enough
                Debug.WriteLine("Cannot generate end points far enough away");
            }

            Debug.WriteLine($"Trace path back - {sw.Elapsed.TotalMicroseconds} us");
            sw.Stop();

        }

    }

}