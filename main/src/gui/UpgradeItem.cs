using Godot;
using System;

namespace Matcha;

public partial class UpgradeItem : MarginContainer
{
	[Export] public Label UpgradeName { get; private set; }
	[Export] public Label ValueLabel { get; private set; }
	[Export] public ProgressBar Progress { get; private set; }
	[Export] public Label Description { get; private set; }
	[Export] public Button BuyButton { get; private set; }
	[Export] public Label PriceLabel { get; private set; }

	[Export] public Control PriceContainer { get; private set; }
	[Export] public Control MaxedContainer { get; private set; }

	public uint Price
	{
		get;
		private set
		{
			field = value;
			PriceLabel.Text = Util.ToCompactString(field);
		}
	}

	public void TicketUpdate()
	{
		var cantAfford = Game.INSTANCE.Tickets < Price;
		BuyButton.Disabled = cantAfford;
		PriceLabel.SelfModulate = cantAfford ? Colors.Red : Colors.White;
	}

	private UpgradeItem Set(
		string upgradeName,
		Func<string> valueStringFunc,
		string description,
		uint[] prices,
		Action onBuy
	)
	{
		UpgradeName.Text = upgradeName;
		Description.Text = description;
		ValueLabel.Text = valueStringFunc();

		Price = prices[0];

		Progress.Value = 0;
		Progress.MaxValue = prices.Length;

		BuyButton.Pressed += () =>
		{
			onBuy();
			ValueLabel.Text = valueStringFunc();
			Game.INSTANCE.Tickets -= Price;

			Progress.Value++;
			if (Progress.Value == Progress.MaxValue)
			{
				BuyButton.Disabled = true;
				PriceContainer.Hide();
				MaxedContainer.Show();
			}
			Price = prices[(int)Progress.Value];
		};

		return this;
	}
	private static readonly PackedScene SCENE = ResourceLoader.Load<PackedScene>("res://scenes/UpgradeItem.tscn");
	public static UpgradeItem New(
		string upgradeName,
		Func<string> valueStringFunc,
		string description,
		uint[] prices,
		Action onBuy
	) => SCENE.Instantiate<UpgradeItem>().Set(upgradeName, valueStringFunc, description, prices, onBuy);
}
