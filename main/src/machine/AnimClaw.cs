using Godot;
using System;
using System.ComponentModel.DataAnnotations;

namespace Matcha;
public partial class AnimClaw : Node2D
{
	private const float ARM_OFFSET = 30;
	private const float MIN_ANGLE = -22.5f;
	private const float MAX_ANGLE = 90.0f;
	private const float DEFAULT_ANGLE = 0;

	private float _angle;
	private float Angle
	{
		get => _angle;
		set
		{
			_angle = value;
			armL.RotationDegrees = +value;
			armR.RotationDegrees = -value;
		}
	}

	private AnimatableBody2D @base;
	private Node2D pivotL, pivotR;
	private AnimatableBody2D armL, armR;

	private enum State { CLOSING, OPENING }
	private State state = State.OPENING;

	public AnimClaw()
	{

	}

	public override void _Ready()
	{
		@base = GetNode<AnimatableBody2D>("%Base");
		pivotL = GetNode<Node2D>("%PivotL");
		pivotR = GetNode<Node2D>("%PivotR");
		armL = GetNode<AnimatableBody2D>("%ArmL");
		armR = GetNode<AnimatableBody2D>("%ArmR");


		pivotL.Position = new(-ARM_OFFSET, 0);
		pivotR.Position = new(+ARM_OFFSET, 0);



		Angle = DEFAULT_ANGLE;
	}

	public override void _Process(double delta)
	{
		// switch (state)
		// {
		//     case State.CLOSING:
		//         if (Angle <= 0)
		//             state = State.OPENING;
		//         else
		//             Angle -= 1;
		//         break;
		//     case State.OPENING:
		//         if (Angle >= MAX_ANGLE)
		//             state = State.CLOSING;
		//         else
		//             Angle += 1;
		//         break;
		// }
	}

}
