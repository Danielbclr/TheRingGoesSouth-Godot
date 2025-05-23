using Godot;
using System;

public partial class HighlightHexTile : Node2D
{
	[Export] public Color HighlightColor { get; set; } = new Color(1, 1, 0, 0.5f);

	public override void _Draw()
	{
		GD.Print($"Highlighting tile at CurrentPosition: {GlobalPosition}");
		DrawColoredPolygon(new Vector2[]
		{
			new Vector2(0, -10.5f),     // N
			new Vector2(16, -3.2f),     // NE
			new Vector2(16, 6f),        // SE
			new Vector2(0, 14),         // S
			new Vector2(-16, 6.6f),     // SW
			new Vector2(-16, -3f),      // NW
		}, HighlightColor);
	}

	public HighlightHexTile()
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		QueueFree();
	}

}
