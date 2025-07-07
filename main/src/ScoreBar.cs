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

	private static readonly int RANDOM_OFFSET = Random.Shared.Next();

	private static Color RandomColor(int n)
	{
		Random COLOR_GEN = new(RANDOM_OFFSET + n);
		return new(COLOR_GEN.NextSingle(), COLOR_GEN.NextSingle(), COLOR_GEN.NextSingle());
	}

	private static Color FullColor(int n)
	{
		if (n < 0)
			return Colors.White;
		if (n < COLORS.Length)
			return COLORS[n];
		return RandomColor(n);
	}

	private const uint MAX_MULTIPLIER = 999;
	public uint Multiplier { get; private set; } = 0;
	private Label multiplierLabel;

	public override void _Ready()
	{
		multiplierLabel = GetNode<Label>("%Multiplier");
	}

	public void Set(double proportion)
	{
		var currFull = (int)proportion;
		var frac = proportion % 1;

		Value = frac;

		TintUnder = FullColor(currFull - 1);
		TintProgress = FullColor(currFull);

		Multiplier = (uint)Math.Min(MAX_MULTIPLIER, currFull);

		multiplierLabel.Text = Multiplier == 0 ? "" : $"{Multiplier}x";
	}
}
