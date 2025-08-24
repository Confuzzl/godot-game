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
			label.Text = $"{field}";
			label.SelfModulate = BigInteger.IsNegative(field) ? Colors.Red : Colors.White;
			label.QueueRedraw();
			color = RandomColor((uint)BigInteger.Abs(field));
			QueueRedraw();
		}
	} = 0;
	public float Radius
	{
		get;
		set
		{
			field = value;
			((CircleShape2D)GetNode<CollisionShape2D>("%Shape").Shape).Radius = value;
		}
	}
	private Color color;

	[Export] private Label label;

	private static readonly uint RANDOM_OFFSET = (uint)Random.Shared.Next();
	private static Color RandomColor(uint n)
	{
		Random COLOR_GEN = new((int)(RANDOM_OFFSET + n));
		return new(COLOR_GEN.NextSingle(), COLOR_GEN.NextSingle(), COLOR_GEN.NextSingle());
	}

	public Capsule()
	{
		ID = COUNT++;

		InputPickable = true;
		FreezeMode = FreezeModeEnum.Static;
		CollisionLayer = Game.BALL_LAYER;
		CollisionMask = Game.WORLD_LAYER | Game.BALL_LAYER | Game.CLAW_LAYER;
	}

	public override int GetHashCode() => ID;

	public override void _Draw()
	{
		DrawCircle(Godot.Vector2.Zero, Radius, color, antialiased: true);
	}
}
