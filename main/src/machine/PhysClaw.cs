using Godot;
using Matcha.Things;
using System;

namespace Matcha;
public partial class PhysClaw : Node2D
{
	private const float START_HEIGHT = 7 * Game.UNIT;
	private const float TRACK_OFFSET = 45;
	private const float TRACK_START = -Machine.LEFT_WIDTH + TRACK_OFFSET;
	private const float TRACK_END = +Machine.LEFT_WIDTH - TRACK_OFFSET;

	private const float INNER_ANGLE = 15;
	private const float OUTER_ANGLE = 45;


	[Export] private AnimatableBody2D @base;
	[Export] private PinJoint2D jointL, jointR;
	[Export] private RigidBody2D armL, armR;

	[Export] private Area2D deepCheck;

	public bool Awake { get; set; }

	public float DropChance { get; set; } = 0.5f;

	private double timeSinceGrab = 0;
	private double dropTime = 0;


	public enum StateEnum { PACING_LEFT, PACING_RIGHT, DOWN, GRABBING, UP_VERTICAL, UP_HORIZONTAL, DEPOSITING }
	public StateEnum MyState
	{
		get;
		private set
		{
			if (field != value)
			{
				switch (value)
				{
					case StateEnum.GRABBING:
						OnStateGrabbing();
						break;
					case StateEnum.UP_VERTICAL:
						OnStateUpVertical();
						break;
					case StateEnum.DEPOSITING:
						OnStateDepositing();
						break;
				}
			}
			field = value;
		}
	} = StateEnum.PACING_RIGHT;

	private float speed = 200;
	[Export] private Timer grabTimer, depositTimer;

	public override void _Ready()
	{
		Position = new(TRACK_START, -START_HEIGHT);

		armL.Mass = 10;
		armR.Mass = 10;

		@base.CollisionLayer = Game.CLAW_LAYER;
		armL.CollisionLayer = Game.CLAW_LAYER;
		armR.CollisionLayer = Game.CLAW_LAYER;
		@base.CollisionMask = 0;
		armL.CollisionMask = Game.CLAW_LAYER;
		armR.CollisionMask = Game.CLAW_LAYER;

		jointL.AngularLimitEnabled = true;
		jointL.AngularLimitLower = Mathf.DegToRad(-INNER_ANGLE);
		jointL.AngularLimitUpper = Mathf.DegToRad(+OUTER_ANGLE);
		jointR.AngularLimitEnabled = true;
		jointR.AngularLimitLower = Mathf.DegToRad(-OUTER_ANGLE);
		jointR.AngularLimitUpper = Mathf.DegToRad(+INNER_ANGLE);

		grabTimer.OneShot = true;
		grabTimer.WaitTime = 1;
		grabTimer.Timeout += () => { MyState = StateEnum.UP_VERTICAL; };

		depositTimer.OneShot = true;
		depositTimer.WaitTime = 1;
		depositTimer.Timeout += () =>
		{
			EmitSignalOnDepositFinish();
			MyState = StateEnum.PACING_LEFT;
			armL.CollisionLayer = Game.CLAW_LAYER;
			armR.CollisionLayer = Game.CLAW_LAYER;
		};
	}

	private bool CapsuleDeepCheck()
	{
		foreach (var node in deepCheck.GetOverlappingBodies())
			if (node is Capsule) return true;
		return false;
	}

	[Signal]
	public delegate void OnSendEventHandler();
	public bool SendIt()
	{
		if (!Awake || (MyState != StateEnum.PACING_LEFT && MyState != StateEnum.PACING_RIGHT))
			return false;
		EmitSignalOnSend();
		MyState = StateEnum.DOWN;

		return true;
	}


	[Signal]
	public delegate void OnGrabbingEventHandler();
	[Signal]
	public delegate void OnDepositingEventHandler();

	[Signal]
	public delegate void OnDepositFinishEventHandler();


	private double CalculateDropTime()
	{
		// const double offset = 0.5;
		if (DropChance == 0)
			return double.PositiveInfinity;
		if (DropChance == 1)
			return 0;
		var travelTime = ((START_HEIGHT + Position.Y) + (TRACK_END - Position.X)) / speed;
		return GD.RandRange(0, travelTime * (1 / DropChance));

	}

	private void OnStateGrabbing()
	{
		EmitSignalOnGrabbing();
		grabTimer.Start();
	}
	private void OnStateUpVertical()
	{
		timeSinceGrab = 0;
		dropTime = CalculateDropTime();
	}
	private void OnStateDepositing()
	{
		EmitSignalOnDepositing();
		armL.CollisionLayer = 0;
		armR.CollisionLayer = 0;
		depositTimer.Start();
	}

	public void BallRadiusChanged(float rad)
	{
		var height = 50 - rad;
		var shape = deepCheck.GetNode<CollisionShape2D>("%Shape");
		shape.Position = shape.Position with { Y = height / 2 };
		var rect = (RectangleShape2D)shape.Shape;
		rect.Size = rect.Size with { Y = height };
	}

	public override void _Process(double delta)
	{
		if (!Awake)
			return;

		const float OPEN = +500_000;
		const float LOOSEN = +100_000;
		const float CLOSE = -500_000;

		(Vector2 offset, StateEnum state, float torque) next = MyState switch
		{
			StateEnum.PACING_LEFT => (new(-speed, 0), (Position.X < TRACK_START) ? StateEnum.PACING_RIGHT : MyState, 0.0f),
			StateEnum.PACING_RIGHT => (new(+speed, 0), (Position.X > TRACK_END) ? StateEnum.PACING_LEFT : MyState, 0.0f),
			StateEnum.DOWN => (new(0, +speed), (CapsuleDeepCheck() || Position.Y > -(Machine.FRONT_HEIGHT + 60)) ? StateEnum.GRABBING : MyState, OPEN),
			StateEnum.GRABBING => (new(), MyState, CLOSE),
			StateEnum.UP_VERTICAL => (new(0, -speed), (Position.Y < -START_HEIGHT) ? StateEnum.UP_HORIZONTAL : MyState, CLOSE),
			StateEnum.UP_HORIZONTAL => (new(+speed, 0), (Position.X > Machine.CHUTE_END_X_OFFSET) ? StateEnum.DEPOSITING : MyState, CLOSE),
			StateEnum.DEPOSITING => (new(), MyState, OPEN),
			_ => throw new NotImplementedException(),
		};

		if (MyState == StateEnum.UP_VERTICAL || MyState == StateEnum.UP_HORIZONTAL)
		{
			timeSinceGrab += delta;
			foreach (var timer in Triggers.TIMERS)
			{
				if ((timer.TimeSinceLastTriggered += delta) > timer.Interval)
				{
					timer.TimeSinceLastTriggered = 0;
					timer.Trigger();
				}
			}


			if (timeSinceGrab >= dropTime)
				next.torque = LOOSEN;
		}

		GetNode<Label>("Label").Text = $"{timeSinceGrab:0.00} {dropTime:0.00}";

		MyState = next.state;
		Position += next.offset * (float)delta;
		armL.ApplyTorque(+next.torque);
		armR.ApplyTorque(-next.torque);
	}
}
