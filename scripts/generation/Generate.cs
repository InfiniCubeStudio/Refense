using Godot;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using nVector2 = System.Numerics.Vector2;

namespace Generate
{
	public partial class Generate : Node3D
	{
		Pathfind path = new Pathfind(1000, 1000, (int)GD.Randi(), -0.3f, 1);

		public override void _Ready()
		{
			StaticBody3D staticBody = new StaticBody3D();
			AddChild(staticBody);

			// Your visible mesh
			MeshInstance3D meshInstance = new MeshInstance3D();
			meshInstance.Mesh = GenerateMesh(path.cells, 0.1f, 1f);
			staticBody.AddChild(meshInstance);

			// Optimized collision
			GenerateCollision(staticBody, path.cells, 0.1f, 1f);
		}

		
		void GenerateCollision(StaticBody3D staticBody, Cell[,] cells, float cellSize, float wallHeight){
			int width = cells.GetLength(0);
			int height = cells.GetLength(1);
			bool[,] handled = new bool[width, height];

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (handled[x, y] || cells[x, y].Type != CellType.Wall)
						continue;

					// Expand in X
					int endX = x;
					while (endX + 1 < width && !handled[endX + 1, y] && cells[endX + 1, y].Type == CellType.Wall)
						endX++;

					// Expand in Y
					int endY = y;
					bool canExpandY;
					do
					{
						canExpandY = true;
						for (int xi = x; xi <= endX; xi++)
						{
							if (endY + 1 >= height || handled[xi, endY + 1] || cells[xi, endY + 1].Type != CellType.Wall)
							{
								canExpandY = false;
								break;
							}
						}
						if (canExpandY) endY++;
					} while (canExpandY);

					// Create BoxShape
					var box = new BoxShape3D();
					float boxWidth = (endX - x + 1) * cellSize;
					float boxDepth = (endY - y + 1) * cellSize;
					box.Size = new Godot.Vector3(boxWidth, wallHeight, boxDepth);

					var collisionShape = new CollisionShape3D();
					collisionShape.Shape = box;

					// Position center of box
					float centerX = (x + endX + 1) * 0.5f * cellSize;
					float centerY = wallHeight * 0.5f;
					float centerZ = (y + endY + 1) * 0.5f * cellSize;
					collisionShape.Position = new Godot.Vector3(centerX, centerY, centerZ);

					staticBody.AddChild(collisionShape);

					// Mark handled
					for (int xi = x; xi <= endX; xi++)
						for (int yi = y; yi <= endY; yi++)
							handled[xi, yi] = true;
				}
			}
		}

		
		void AddQuad(SurfaceTool st, Godot.Vector3 v0, Godot.Vector3 v1, Godot.Vector3 v2, Godot.Vector3 v3, bool r = false){
			if (!r){
				st.AddVertex(v0);
				st.AddVertex(v1);
				st.AddVertex(v2);

				st.AddVertex(v0);
				st.AddVertex(v2);
				st.AddVertex(v3);
			} else{
				st.AddVertex(v2);
				st.AddVertex(v1);
				st.AddVertex(v0);

				st.AddVertex(v3);
				st.AddVertex(v2);
				st.AddVertex(v0);
			}
		}


		 Mesh GenerateMesh(Cell[,] cells, float cellSize, float wallHeight)
		{
			int width = cells.GetLength(0);
			int height = cells.GetLength(1);

			List<Godot.Vector3> vertices = new List<Godot.Vector3>();
			List<int> indices = new List<int>();
			List<Godot.Vector3> normals = new List<Godot.Vector3>();

			int indexOffset = 0;

			void AddQuad(Godot.Vector3 v0, Godot.Vector3 v1, Godot.Vector3 v2, Godot.Vector3 v3, Godot.Vector3 normal)
			{
				vertices.Add(v0);
				vertices.Add(v1);
				vertices.Add(v2);
				vertices.Add(v3);

				normals.Add(normal);
				normals.Add(normal);
				normals.Add(normal);
				normals.Add(normal);

				// Two triangles
				indices.Add(indexOffset + 0);
				indices.Add(indexOffset + 1);
				indices.Add(indexOffset + 2);

				indices.Add(indexOffset + 2);
				indices.Add(indexOffset + 3);
				indices.Add(indexOffset + 0);

				indexOffset += 4;
			}

			// Loop cells
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (cells[x, y].Type != CellType.Wall)
						continue;

					Godot.Vector3 pos = new Godot.Vector3(x * cellSize, 0, y * cellSize);

					bool left = x == 0 || cells[x - 1, y].Type != CellType.Wall;
					bool right = x == width - 1 || cells[x + 1, y].Type != CellType.Wall;
					bool front = y == 0 || cells[x, y - 1].Type != CellType.Wall;
					bool back = y == height - 1 || cells[x, y + 1].Type != CellType.Wall;

					// Top
					AddQuad(
						pos + new Godot.Vector3(0, wallHeight, 0),
						pos + new Godot.Vector3(cellSize, wallHeight, 0),
						pos + new Godot.Vector3(cellSize, wallHeight, cellSize),
						pos + new Godot.Vector3(0, wallHeight, cellSize),
						new Godot.Vector3(0, 1, 0)
					);

					if (front)
						AddQuad(
							pos + new Godot.Vector3(0, 0, 0),
							pos + new Godot.Vector3(cellSize, 0, 0),
							pos + new Godot.Vector3(cellSize, wallHeight, 0),
							pos + new Godot.Vector3(0, wallHeight, 0),
							new Godot.Vector3(0, 0, -1)
						);

					if (back)
						AddQuad(
							pos + new Godot.Vector3(cellSize, 0, cellSize),
							pos + new Godot.Vector3(0, 0, cellSize),
							pos + new Godot.Vector3(0, wallHeight, cellSize),
							pos + new Godot.Vector3(cellSize, wallHeight, cellSize),
							new Godot.Vector3(0, 0, 1)
						);

					if (left)
						AddQuad(
							pos + new Godot.Vector3(0, 0, cellSize),
							pos + new Godot.Vector3(0, 0, 0),
							pos + new Godot.Vector3(0, wallHeight, 0),
							pos + new Godot.Vector3(0, wallHeight, cellSize),
							new Godot.Vector3(-1, 0, 0)
						);

					if (right)
						AddQuad(
							pos + new Godot.Vector3(cellSize, 0, 0),
							pos + new Godot.Vector3(cellSize, 0, cellSize),
							pos + new Godot.Vector3(cellSize, wallHeight, cellSize),
							pos + new Godot.Vector3(cellSize, wallHeight, 0),
							new Godot.Vector3(1, 0, 0)
						);
				}
			}

			// Pack into ArrayMesh
			var arrays = new Godot.Collections.Array();
			arrays.Resize((int)Mesh.ArrayType.Max);

			arrays[(int)Mesh.ArrayType.Vertex] = vertices.ToArray();
			arrays[(int)Mesh.ArrayType.Index] = indices.ToArray();
			arrays[(int)Mesh.ArrayType.Normal] = normals.ToArray();

			ArrayMesh mesh = new ArrayMesh();
			mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

			return mesh;
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
			FastNoiseLite noise = new FastNoiseLite();
			noise.Seed = seed;
			Random rnd = new Random(seed);
			cells = new Cell[width, height];

			// Create walls
			for(int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if(noise.GetNoise2D((float)x * 12f, (float)y * 12f) < threshold) {
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
					cells[start.X, start.Y].HCost = nVector2.Distance(new nVector2(start.X, start.Y), new nVector2(ends[e].X, ends[e].Y));
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
								cells[c.X - 1, c.Y].HCost = nVector2.Distance(new nVector2(c.X - 1, c.Y), new nVector2(ends[e].X, ends[e].Y));
								cells[c.X - 1, c.Y].FCost = cells[c.X - 1, c.Y].GCost + cells[c.X - 1, c.Y].HCost;
							}
							if (c.X + 1 < cells.GetLength(0) && cells[c.X + 1, c.Y].Type != CellType.Wall) {
								AStar status = (AStar)Math.Max((int)cells[c.X + 1, c.Y].Status, (int)AStar.Open);
								cells[c.X + 1, c.Y].Status = status;
								if (status == AStar.Open) {
									OpenCells.Add((c.X + 1, c.Y, cells[c.X + 1, c.Y]));
								}
								cells[c.X + 1, c.Y].GCost = Math.Min(cells[c.X + 1, c.Y].GCost, cells[c.X, c.Y].GCost + 1);
								cells[c.X + 1, c.Y].HCost = nVector2.Distance(new nVector2(c.X + 1, c.Y), new nVector2(ends[e].X, ends[e].Y));
								cells[c.X + 1, c.Y].FCost = cells[c.X + 1, c.Y].GCost + cells[c.X + 1, c.Y].HCost;
							}
							if (c.Y - 1 >= 0 && cells[c.X, c.Y - 1].Type != CellType.Wall) {
								AStar status = (AStar)Math.Max((int)cells[c.X, c.Y - 1].Status, (int)AStar.Open);
								cells[c.X, c.Y - 1].Status = status;
								if (status == AStar.Open) {
									OpenCells.Add((c.X, c.Y - 1, cells[c.X, c.Y - 1]));
								}
								cells[c.X, c.Y - 1].GCost = Math.Min(cells[c.X, c.Y - 1].GCost, cells[c.X, c.Y].GCost + 1);
								cells[c.X, c.Y - 1].HCost = nVector2.Distance(new nVector2(c.X, c.Y - 1), new nVector2(ends[e].X, ends[e].Y));
								cells[c.X, c.Y - 1].FCost = cells[c.X, c.Y - 1].GCost + cells[c.X, c.Y - 1].HCost;
							}
							if (c.Y + 1 < cells.GetLength(1) && cells[c.X, c.Y + 1].Type != CellType.Wall) {
								AStar status = (AStar)Math.Max((int)cells[c.X, c.Y + 1].Status, (int)AStar.Open);
								cells[c.X, c.Y + 1].Status = status;
								if (status == AStar.Open) {
									OpenCells.Add((c.X, c.Y + 1, cells[c.X, c.Y + 1]));
								}
								cells[c.X, c.Y + 1].GCost = Math.Min(cells[c.X, c.Y + 1].GCost, cells[c.X, c.Y].GCost + 1);
								cells[c.X, c.Y + 1].HCost = nVector2.Distance(new nVector2(c.X, c.Y + 1), new nVector2(ends[e].X, ends[e].Y));
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
