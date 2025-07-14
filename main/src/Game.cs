using Godot;
using Matcha.Things;
using System;
using System.Diagnostics;
using System.Numerics;


namespace Matcha;



#region Util
public static class Util
{
	public static string ToCompactString(dynamic n)
	{
		const uint a = 97;

		string str = n.ToString();
		if (n < 1000)
			return str;

		var exp10 = str.Length - 1;
		var thousands = exp10 / 3;

		var suffix = (char)(a + thousands - 1);

		return (exp10 % 3) switch
		{
			0 => $"{str[0]}.{str[1]}{str[2]}",
			1 => $"{str[0]}{str[1]}.{str[2]}",
			2 => $"{str[0]}{str[1]}{str[2]}",
			_ => throw new UnreachableException()
		} + suffix;
	}

	// https://stackoverflow.com/questions/62503389/c-sharp-unity-multiply-biginteger-by-float
	public static BigInteger MultiplyBigInteger(BigInteger n, double m)
	{
		const uint precision = 1_000_000_000;
		return n * (BigInteger)(m * precision) / (BigInteger)precision;
	}
	// https://stackoverflow.com/questions/11859111/biginteger-division-in-c-sharp
	public static double DivideBigInteger(BigInteger a, BigInteger b)
	{
		const uint precision = 1_000;
		return (double)(precision * a / b) / precision;
	}

	public static Texture2D GetTexture(string name) => ResourceLoader.Load<Texture2D>($"res://assets/{name}");
}
#endregion
public partial class Game : Node2D
{
	public const float UNIT = 60;

	private static Game instance;
	public static Game INSTANCE { get => instance; }

	#region Collision Flags
	public const uint WORLD_LAYER = 1 << 0;
	public const uint BALL_LAYER = 1 << 1;
	public const uint CLAW_LAYER = 1 << 2;
	#endregion

	#region Ball
	public uint NumWaves { get; private set; } = 2;
	public uint NumBallsInWave { get; private set; } = 7;
	public uint NumBallsPerSubWave { get; private set; } = 7;
	public BigInteger BallValue = 1;
	#endregion

	#region Score
	private const double TOTAL_CLEAR_MULTIPLIER = 3;
	public BigInteger TotalScore { get; private set; } = 0;
	public BigInteger Score
	{
		get;
		set
		{
			field = value;
			scoreBar.GetNode<Label>("%RoundScore").Text = Util.ToCompactString(value);
			scoreBar.Set((Goal is null) ? 0 : Util.DivideBigInteger(value, Goal.Value));
		}
	}
	public BigInteger? Goal
	{
		get;
		private set
		{
			field = value;
			scoreBar.GetNode<Label>("%GoalScore").Text = (value is null) ? "-" : Util.ToCompactString(value);
		}
	}
	public BigInteger TotalRoundValue { get; private set; } = 0;
	private ScoreBar scoreBar;
	#endregion

	public uint Round
	{
		get;
		private set
		{
			field = value;
			GetNode<Label>("%GUI/%Screen/%RoundCount").Text = $"{value}";
		}
	}

	public const uint MAX_TOKENS = 5;
	public uint Tokens
	{
		get;
		private set
		{
			field = value;
			GetNode<Label>("%GUI/%Screen/%TokenCount").Text = $"{value}";
		}
	}

	public const uint MAX_TICKETS = 999;
	public uint Tickets
	{
		get;
		private set
		{
			field = Math.Min(MAX_TICKETS, value);
			GetNode<Label>("%GUI/%TicketBox/%TicketCount").Text = $"{value}";
		}
	}

	[Export] public Machine Machine { get; private set; }
	[Export] public Gui Gui { get; private set; }
	[Export] public AudioStreamPlayer Music { get; private set; }


	public Game()
	{
		instance = this;
		GD.Print("Game Start");

		// GD.Print(DisplayServer.ScreenGetSize());

		// GD.Print(BigInteger.Parse("1,000", NumberStyles.AllowThousands));
	}

	//public void ConsumeCapsule(Capsule cap) { Score += cap.Value; }
	public Godot.Vector2 MousePosition() => GetViewport().GetMousePosition();

	private void ResetRound()
	{
		TotalScore += Score;
		BallValue = Round;
		Score = 0;
		TotalRoundValue = 0;
		Goal = null;
		Tokens = MAX_TOKENS;
	}

	private double CalculateGoalMultiplier()
	{
		const double min = 0.5;
		const uint cutoff = 30;
		return Round < cutoff ?
		(min + (1 - min) * (Round - 1) / (cutoff - 1)) :
		Math.Pow(1.05, Round - cutoff);
	}
	private BigInteger CalculateGoal()
	{
		const uint precision = 1_000_000;
		var averageBig = (Machine.CalculateRoundValue() * precision) / (Machine.BallCount * precision);
		return Util.MultiplyBigInteger(averageBig, CalculateGoalMultiplier() * MAX_TOKENS * (1 - Machine.Claw.DropChance));
	}

	public override void _Ready()
	{
		scoreBar = Gui.GetNode<ScoreBar>("%ScoreBar");

		Gui.Settings.Music.Value = 0;


		Round = 0;
		Tickets = 0;

		ResetRound();

		Machine.OnRestock += () =>
		{
			Round++;
			ResetRound();
		};
		Machine.OnInGame += () =>
		{
			TotalRoundValue = Machine.CalculateRoundValue();
			Goal = BigInteger.Max(Machine.MinCapsuleValue, CalculateGoal());
		};
		Machine.OnRoundEnd += () =>
		{
			NumWaves = scoreBar.Multiplier;
			Tickets += scoreBar.Multiplier;
		};
		Machine.OnOutOfTokens += () =>
		{
		};
		Machine.OnTotalClear += () =>
		{
			Score = Util.MultiplyBigInteger(Score, TOTAL_CLEAR_MULTIPLIER);
		};
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton me)
		{

			if (me.Pressed && me.ButtonIndex == MouseButton.WheelUp)
			{
				Score += 1;
			}
		}
		if (@event is InputEventKey ke)
		{
			if (!ke.Pressed) return;
			switch (ke.Keycode)
			{
				case Key.Escape:
					if (Gui.ActiveWindow is null)
						GetTree().Quit();
					else
					{
						GetTree().Paused = false;
						Gui.CloseWindow();
					}
					break;
				case Key.Space:
					if (Tokens > 0 && Machine.SendIt()) Tokens -= 1;
					break;
			}
		}
	}
}
