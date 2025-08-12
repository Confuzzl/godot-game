using Godot;
using System;

namespace Matcha;
public partial class ScoreBar : TextureProgressBar
{
	private static readonly Color[] COLORS =
	[
		new("#44ce1b"),
		new("#bbdb44"),
		new("#f2a134"),
		new("#e51f1f"),
	];

	private static readonly uint RANDOM_OFFSET = (uint)Random.Shared.Next();

	private static Color RandomColor(uint n)
	{
		Random COLOR_GEN = new((int)(RANDOM_OFFSET + n));
		return new(COLOR_GEN.NextSingle(), COLOR_GEN.NextSingle(), COLOR_GEN.NextSingle());
	}

	private static Color FullColor(uint n)
	{
		if (n == uint.MaxValue)
			return Colors.White;
		if (n < COLORS.Length)
			return COLORS[n];
		return RandomColor(n);
	}

	private const uint MAX_MULTIPLIER = 9999;
	public uint Multiplier { get; private set; } = 0;
	[Export] public Label MultiplierLabel { get; private set; }
	[Export] public Label ScoreLabel { get; private set; }
	[Export] public Label GoalLabel { get; private set; }



	public void Set(double proportion)
	{
		var currFull = (uint)proportion;
		var frac = proportion % 1;

		Value = frac;

		TintUnder = FullColor(unchecked(currFull - 1));
		TintProgress = FullColor(currFull);

		Multiplier = Math.Min(MAX_MULTIPLIER, currFull);

		MultiplierLabel.Text = Multiplier == 0 ? "" : $"{Multiplier}x";
	}
}
