using Godot;
using System;

public partial class Particles : GpuParticles2D
{
	//public Particles()
	//{
	//}

	public override void _Ready()
	{
		Amount = 32;
		Lifetime = 10;
		DrawLine();
	}
	private void DrawLine()
	{
		void Emit(Vector2 pos) => EmitParticle(Transform.Translated(pos), new(), Colors.White, Colors.White, (uint)EmitFlags.Position);

		Vector2 start = new(-100, -100), end = new(100, 100);
		Vector2 step = (end - start) / (Amount - 1);

		Emit(start);
		Emit(end);
		for (var i = 1; i < Amount - 1; i++)
		{
			Emit(start + step * i);
		}
	}
	//public override void _Process(double delta)
	//{
	//	void Emit(Vector2 pos) => EmitParticle(Transform.Translated(pos), new(), Colors.White, Colors.White, (uint)EmitFlags.Position);

	//	Vector2 start = new(-100, -100), end = new(100, 100);
	//	//Vector2 step = (end - start) / (Amount - 1);

	//	Emit(start);
	//	Emit(end);
	//}
}
