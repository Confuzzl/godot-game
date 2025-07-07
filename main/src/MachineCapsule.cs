using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

using BigInteger = System.Numerics.BigInteger;

namespace Matcha;
public partial class Machine : Node2D
{
	private const float CHUTE_SEPARATOR_X_OFFSET = 3 * Game.UNIT;
	private const float CHUTE_SEPARATOR_HEIGHT = 4 * Game.UNIT;
	public const float CHUTE_END_X_OFFSET = 4 * Game.UNIT;

	private static readonly PackedScene capsuleScene = ResourceLoader.Load<PackedScene>("res://scenes/Capsule.tscn");

	private readonly SortedList<int, Capsule> capsules = [];
	public uint BallCount => (uint)capsules.Count;
	public BigInteger MinCapsuleValue => capsules.Values.MinBy(cap => cap.Value).Value;

	private ChuteEnd chuteEnd;
	private Area2D chuteCheck;
	private Area2D mergeCheck;

	private Restocker restocker;

	partial void ReadyCapsule()
	{
		restocker = GetNode<Restocker>("%Restocker");

		chuteEnd = new ChuteEnd(new(1.75f * Game.UNIT, 0.1f * Game.UNIT)) { Position = new(CHUTE_END_X_OFFSET, -1 * Game.UNIT) };
		AddChild(chuteEnd);

		chuteCheck = GetNode<Area2D>("%ChuteCheck");
		chuteCheck.Position = new(CHUTE_END_X_OFFSET, -FRONT_HEIGHT);
		chuteCheck.BodyEntered += (node) =>
		{
			if (node is Capsule cap)
			{
				Game.INSTANCE.ConsumeCapsule(cap);
				cap.CollisionMask = 0;
				capsules.Remove(cap.ID);
			}
		};

		mergeCheck = GetNode<Area2D>("%MergeCheck");
		mergeCheck.Position = new(mergeCheck.Position.X, -(FRONT_HEIGHT + CHUTE_SEPARATOR_HEIGHT + Game.UNIT));
		mergeCheck.CollisionMask = Game.BALL_LAYER;
		mergeCheck.BodyEntered += (node) => { if (node is Capsule cap) Merge(cap); };
	}

	private Capsule RandomCapsule() => capsules.GetValueAtIndex(Random.Shared.Next(0, capsules.Count));

	private void Merge(Capsule cap)
	{
		if (capsules.Count != 0)
		{
			capsules.Remove(cap.ID);

			var picked = RandomCapsule();
			picked.Value += cap.Value;

			cap.QueueFree();
		}
	}

	public Capsule AddCapsule(BigInteger value, float radius, Vector2 position, Color color)
	{
		var capsule = capsuleScene.Instantiate<Capsule>();
		capsule.Value = value;
		capsule.Position = position;
		var ball = capsule.GetBall();
		ball.Color = color;
		ball.CircleShape().Radius = radius;

		AddChild(capsule);
		capsules.Add(capsule.ID, capsule);

		return capsule;
	}

	public void Shuffle()
	{
		var rand = capsules.Keys.OrderBy(_ => Random.Shared.Next()).ToArray();
		for (var i = 0; i < rand.Length / 2; i += 2)
		{
			Capsule a = capsules[rand[i]], b = capsules[rand[i + 1]];
			Vector2 ap = a.Position, bp = b.Position;
			PhysicsServer2D.BodySetState(b.GetRid(), PhysicsServer2D.BodyState.Transform, Transform.Translated(ap));
			PhysicsServer2D.BodySetState(a.GetRid(), PhysicsServer2D.BodyState.Transform, Transform.Translated(bp));
		}

		foreach (var cap in capsules.Values)
		{
			cap.ApplyCentralImpulse(Vector2.FromAngle((float)GD.RandRange(0, Math.Tau)) * 500);
		}
	}

	public BigInteger CalculateRoundValue() => capsules.Values.Aggregate(BigInteger.Zero, (acc, cap) => acc + cap.Value);
}
