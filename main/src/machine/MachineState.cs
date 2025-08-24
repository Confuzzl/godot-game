using Godot;
using System;
using System.Numerics;

namespace Matcha;
public partial class Machine : Node2D
{
	public enum StateEnum { LIMBO, RESTOCKING, IN_ROUND, OUT_OF_TOKENS, TOTAL_CLEAR }
	public StateEnum State
	{
		get;
		private set
		{
			if (field != value)
			{
				// limbo to any other gamestate
				if (field == StateEnum.LIMBO)
				{
					Game.INSTANCE.Gui.ShopButton.Disabled = true;
					startButton.Disabled = true;
				}

				switch (value)
				{
					case StateEnum.LIMBO:
						Game.INSTANCE.Gui.ShopButton.Disabled = false;
						startButton.Disabled = false;
						break;
					case StateEnum.RESTOCKING:
						Restocking();
						break;
					case StateEnum.IN_ROUND:
						InRound();
						break;
					case StateEnum.OUT_OF_TOKENS:
						OutOfTokens();
						RoundEnd();
						break;
					case StateEnum.TOTAL_CLEAR:
						TotalClear();
						RoundEnd();
						break;
				}
			}
			field = value;
		}
	} = StateEnum.LIMBO;


	[Signal] public delegate void OnRestockEventHandler();
	[Signal] public delegate void OnInRoundEventHandler();
	[Signal] public delegate void OnRoundEndEventHandler();
	[Signal] public delegate void OnOutOfTokensEventHandler();
	[Signal] public delegate void OnTotalClearEventHandler();


	private Timer restockToGameTimer, endToLimboTimer;

	private void ReadyState()
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
			State = StateEnum.IN_ROUND;
		};
		AddChild(restockToGameTimer);

		endToLimboTimer = new()
		{
			WaitTime = 1,
			OneShot = true
		};
		endToLimboTimer.Timeout += () =>
		{
			//State = StateEnum.RESTOCKING;
			State = StateEnum.LIMBO;
		};
		AddChild(endToLimboTimer);

		Claw.OnDepositFinish += () =>
		{
			if (BallCount == 0)
			{
				State = StateEnum.TOTAL_CLEAR;
			}
			else if (Game.INSTANCE.Tokens == 0)
			{
				State = StateEnum.OUT_OF_TOKENS;
			}
		};
	}

	private void Restocking()
	{
		EmitSignal(SignalName.OnRestock);

		chuteEnd.ProcessMode = ProcessModeEnum.Disabled;
		chuteCheck.ProcessMode = ProcessModeEnum.Disabled;
		mergeCheck.ProcessMode = ProcessModeEnum.Always;

		foreach (var cap in capsules.Values)
			cap.GravityScale = Restocker.GRAVITY_SCALE;

		restocker.Start();
	}
	private void InRound()
	{
		EmitSignal(SignalName.OnInRound);

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

		//Claw.Awake = false;
		endToLimboTimer.Start();
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
