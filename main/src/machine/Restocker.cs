using Godot;
using Matcha.Things;
using System;
using System.Numerics;

namespace Matcha;
public partial class Restocker() : Node
{
	public const float GRAVITY_SCALE = 8;

	private double start = -1;
	private double current = 0;
	private double interval;
	private bool Running { get => start >= 0; }

	private uint remaining;

	private uint currSubWave = 0;
	private uint numSubWaves = 0;

	public float BallRadius
	{
		get;
		set
		{
			field = value;
			Game.INSTANCE.Machine.Claw.BallRadiusChanged(value);
		}
	}

	private static void AddWaveCapsule(BigInteger val, float rad)
	{
		Game.INSTANCE.Machine.AddCapsule(val, rad,
			new((float)GD.RandRange(-Machine.LEFT_WIDTH + rad, +Machine.LEFT_WIDTH - rad), -Machine.FRONT_HEIGHT),
			new(GD.Randf(), GD.Randf(), GD.Randf())).GravityScale = GRAVITY_SCALE;
	}

	public void Start()
	{
		start = Time.GetTicksMsec() / 1000.0;
		current = start;

		interval = Math.Clamp(2.0 / numSubWaves, 0.02, 0.5);

		remaining = Game.INSTANCE.NumWaves * Game.INSTANCE.NumBallsInWave;

		currSubWave = 0;
		numSubWaves = (uint)Math.Ceiling((double)remaining / Game.INSTANCE.NumBallsPerSubWave);

		Restock();
	}

	private void Restock()
	{
		Triggers.ON_RESTOCK.Trigger();
		var n = Math.Min(remaining, Game.INSTANCE.NumBallsPerSubWave);
		remaining -= n;
		for (var i = 0u; i < n; i++)
			AddWaveCapsule(Game.INSTANCE.BallValue, BallRadius);
		currSubWave++;
	}

	[Signal]
	public delegate void OnRestockEndEventHandler();
	private void End()
	{
		start = -1;
		EmitSignal(SignalName.OnRestockEnd);
	}

	public override void _Process(double delta)
	{
		if (!Running) return;
		if (currSubWave >= numSubWaves)
		{
			End();
			return;
		}

		current += delta;

		var target = start + interval * currSubWave;

		if (current >= target) Restock();
	}
}
