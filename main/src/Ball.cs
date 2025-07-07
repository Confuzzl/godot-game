using Godot;
using System;

namespace Matcha;
public partial class Ball : CollisionShape2D
{
	public Color Color
	{
		get;
		set
		{
			field = value;
			QueueRedraw();
		}
	}

	// private static readonly Texture2D texture = ResourceLoader.Load<Texture2D>("res://assets/SweetBabyHachiware2.webp");

	public CircleShape2D CircleShape() => (CircleShape2D)Shape;

	public override void _Draw()
	{
		// const float scale = 1.2f;
		var r = CircleShape().Radius;
		DrawCircle(Position, r, Color, antialiased: true);
		// var img = texture.GetImage();
		// img.Resize((int)(2 * r * scale), (int)(2 * r * scale));
		// DrawTexture(ImageTexture.CreateFromImage(img), new(-scale * r, -scale * r));
	}

}
