using Godot;
using System;

namespace Matcha;
public partial class Claw : Node2D
{
	private const float ARM_OFFSET = 30;
	private const float MIN_ANGLE = 40;
	private const float MAX_ANGLE = 120;

	private const float DEFAULT_ANGLE = 60;

	private float angle;
	private float Angle
	{
		get => angle;
		set
		{
			angle = value;
			pivotL.RotationDegrees = +angle;
			pivotR.RotationDegrees = -angle;
			// var rect = Box();
			// area.Position = new(0, rect.Size.Y / 2);
			// bounds.Size = rect.Size;
		}
	}
	private Control pivotL, pivotR;
	private TextureRect armL, armR;
	private Area2D area;
	private RectangleShape2D bounds;

	private enum State { CLOSING, OPENING }
	private State state = State.OPENING;

	private RayCast2D ray;

	private enum Direction { LEFT, RIGHT }
	private Direction direction;

	public Claw()
	{

	}

	public override void _Ready()
	{
		armL = GetNode<TextureRect>("%ArmL");
		pivotL = GetNode<Control>("%PivotL");
		pivotL.Position = new(-ARM_OFFSET, 0);

		armR = GetNode<TextureRect>("%ArmR");
		pivotR = GetNode<Control>("%PivotR");
		pivotR.Position = new(+ARM_OFFSET, 0);

		area = new Area2D();
		bounds = new RectangleShape2D();
		area.AddChild(new CollisionShape2D { Shape = bounds });
		AddChild(area);

		ray = new RayCast2D
		{
			Position = new(),
			TargetPosition = new(0, 100),
			// CollisionMask = 0
		};
		AddChild(ray);
		// ray.SetCollisionMaskValue(2, true);

		Angle = DEFAULT_ANGLE;
	}

	// public Rect2 Box()
	// {
	// 	// random good enough
	// 	(var sin, var cos) = Math.SinCos(Angle / 180 * Math.PI / 2);
	// 	return new(new(), new(200 * (float)sin, 20 + 50 * (float)cos));
	// }

	public override void _Draw()
	{
		// base._Draw();
		// DrawLine(new(), new(0, 100), Colors.Red);
	}


	public override void _Process(double delta)
	{
		base._Process(delta);

		// Position = GetGlobalMousePosition() - GetParent<Machine>().Position;
		// ray.Position = Position;
		// ray.TargetPosition = Position + new Vector2(0, 1);

		// var collider = ray.GetCollider();
		// var label = GetTree().Root.GetChild<Game>(0).moneyLabel;
		// // label.Text = Position.ToString();
		// if (collider != null && collider is Capsule cap)
		// {
		// 	label.Text = cap.Position.ToString();
		// }
		// else
		// {
		// 	label.Text = "None";
		// }
		// label.QueueRedraw();


		// Angle += 5 * (float)delta;
		// switch (state)
		// {
		// 	case State.CLOSING:
		// 		if (Angle <= MIN_ANGLE)
		// 			state = State.OPENING;
		// 		else
		// 			Angle -= 1;
		// 		break;
		// 	case State.OPENING:
		// 		if (Angle >= MAX_ANGLE)
		// 			state = State.CLOSING;
		// 		else
		// 			Angle += 1;
		// 		break;
		// }

		// GetNode<TextureRect>("").GetRect().Merge();

		// if (angle <= MIN_ANGLE)
		// {

		// }
		// else if (angle >= MAX_ANGLE) { }
	}
}
