using Godot;
using System;
using System.Numerics;

namespace Matcha;
public partial class Machine : Node2D
{
	public enum State { NONE, RESTOCKING, IN_GAME, OUT_OF_TOKENS, TOTAL_CLEAR }
	public State MyState
	{
		get;
		private set
		{
			if (field != value)
			{
				switch (value)
				{
					case State.NONE:
						break;
					case State.RESTOCKING:
						Restock();
						break;
					case State.IN_GAME:
						InGame();
						break;
					case State.OUT_OF_TOKENS:
						OutOfTokens();
						RoundEnd();
						break;
					case State.TOTAL_CLEAR:
						TotalClear();
						RoundEnd();
						break;
				}
			}
			field = value;
		}
	} = State.NONE;

	[Signal]
	public delegate void OnRestockEventHandler();
	[Signal]
	public delegate void OnInGameEventHandler();
	[Signal]
	public delegate void OnRoundEndEventHandler();
	[Signal]
	public delegate void OnOutOfTokensEventHandler();
	[Signal]
	public delegate void OnTotalClearEventHandler();


	private Timer restockToGameTimer, endToRestockTimer;

	partial void ReadyState()
	{
		restocker.OnRestockEnd += () =>
		{
			restockToGameTimer.Start();
		};

		restockToGameTimer = new()
		{

			WaitTime = 1,
			OneShot = true
		};
		restockToGameTimer.Timeout += () =>
		{
			MyState = State.IN_GAME;
		};
		AddChild(restockToGameTimer);

		endToRestockTimer = new()
		{
			WaitTime = 1,
			OneShot = true
		};
		endToRestockTimer.Timeout += () =>
		{
			MyState = State.RESTOCKING;
		};
		AddChild(endToRestockTimer);

		Claw.OnDepositFinish += () =>
		{
			if (BallCount == 0)
			{
				MyState = State.TOTAL_CLEAR;
			}
			else if (Game.INSTANCE.Tokens == 0)
			{
				MyState = State.OUT_OF_TOKENS;
			}
		};
	}

	private void Restock()
	{
		EmitSignal(SignalName.OnRestock);

		chuteEnd.ProcessMode = ProcessModeEnum.Disabled;
		chuteCheck.ProcessMode = ProcessModeEnum.Disabled;
		mergeCheck.ProcessMode = ProcessModeEnum.Always;

		foreach (var cap in capsules.Values)
			cap.GravityScale = Restocker.GRAVITY_SCALE;

		restocker.Start();
	}
	private void InGame()
	{
		EmitSignal(SignalName.OnInGame);

		chuteEnd.ProcessMode = ProcessModeEnum.Always;
		chuteCheck.ProcessMode = ProcessModeEnum.Always;
		mergeCheck.ProcessMode = ProcessModeEnum.Disabled;
		Claw.Awake = true;

		foreach (var cap in capsules.Values)
		{
			cap.GravityScale = 1;
		}
	}
	private void RoundEnd()
	{
		EmitSignal(SignalName.OnRoundEnd);

		Claw.Awake = false;
		endToRestockTimer.Start();
	}
	private void OutOfTokens()
	{
		EmitSignal(SignalName.OnOutOfTokens);
	}
	private void TotalClear()
	{
		EmitSignal(SignalName.OnTotalClear);
	}
}
