using Godot;
using System;

namespace Matcha;

public partial class Tooltip : PanelContainer
{
	private static readonly Vector2 OFFSET = new(5, 5);

	[Export] private Label top;
	[Export] private Label text;

	public Tooltip()
	{
		ZIndex = 1;

		VisibilityChanged += () => ProcessMode = Visible ? ProcessModeEnum.Always : ProcessModeEnum.Disabled;
	}

	public override void _Process(double delta)
	{
		//base._Process(delta);
		Position = Game.INSTANCE.GetLocalMousePosition() + OFFSET;
	}

	public void Set(string name, string description, string trigger)
	{
		top.Text = name;
		text.Text = "";
		if (trigger != "")
			text.Text += $"* {trigger}\n";
		if (description != "")
			text.Text += description;
		Size = Vector2.Zero;
		Show();
	}
}
