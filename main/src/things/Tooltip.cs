using Godot;
using System;

namespace Matcha;

public partial class Tooltip : ColorRect
{
	private static readonly Vector2 OFFSET = new(5, 5);

	[Export] private Label top;
	[Export] private Label text;

	public override void _Process(double delta)
	{
		//base._Process(delta);
		Position = Game.INSTANCE.GetLocalMousePosition() + OFFSET;
	}

	public void Set(string name, string description, string trigger)
	{
		top.Text = name;
		text.Text = $"""
			* {trigger}
			{description}
			""";
		//top.
	}
}
