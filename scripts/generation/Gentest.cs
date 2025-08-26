using Godot;

public partial class Gentest : Control
{
	[Export] public int Rows = 20;
	[Export] public int Cols = 20;
	[Export] public int CellSize = 16;

	public override void _Ready()
	{
		GenerateGrid();
	}

	private void GenerateGrid()
	{
		for (int row = 0; row < Rows; row++)
		{
			for (int col = 0; col < Cols; col++)
			{
				ColorRect cell = new ColorRect();

				// Size
				cell.CustomMinimumSize = new Vector2(CellSize, CellSize);
				cell.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
				cell.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;

				// Position
				cell.Position = new Vector2(col * CellSize, row * CellSize);

				// Random color
				cell.Color = new Color(
					(float)GD.RandRange(0, 1),
					(float)GD.RandRange(0, 1),
					(float)GD.RandRange(0, 1)
				);

				AddChild(cell);
			}
		}
	}
}
