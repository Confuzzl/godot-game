using Godot;
using System;

namespace Matcha;
public partial class ChuteEnd : Area2D
{
	public ChuteEnd(Vector2 size)
	{
		// picked up balls are on layer 2;
		// SetCollisionMaskValue(2, true);
		CollisionMask = Game.BALL_LAYER;

		AddChild(new CollisionShape2D() { Shape = new RectangleShape2D() { Size = size } });
		BodyEntered += (node) =>
		{
			if (node is Capsule cap)
			{
				cap.QueueFree();
			}
		};
	}
}
