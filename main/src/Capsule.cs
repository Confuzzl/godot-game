using Godot;
using System;
using System.Numerics;

namespace Matcha;
public partial class Capsule : RigidBody2D
{
	private static int COUNT = 0;

	public int ID { get; init; }
	public BigInteger Value
	{
		get;
		set
		{
			field = value;
			var label = GetNode<Label>("%Shape/%Label");
			label.Text = $"{value}";
			label.QueueRedraw();
		}
	} = 0;

	public Capsule()
	{
		ID = COUNT++;

		InputPickable = true;
		FreezeMode = FreezeModeEnum.Static;
		CollisionLayer = Game.BALL_LAYER;
		CollisionMask = Game.WORLD_LAYER | Game.BALL_LAYER | Game.CLAW_LAYER;
	}

	public override int GetHashCode() => ID;

	public Ball GetBall() => GetNode<Ball>("%Shape");
}
