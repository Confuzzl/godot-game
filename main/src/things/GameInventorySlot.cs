using Godot;
using System;
using System.Diagnostics;

namespace Matcha.Things;
public partial class GameInventorySlot : GameSlot
{
	private new IInventorySlot Proxy => (IInventorySlot)base.Proxy;
	[Export] private Control shopOverlay;
	[Export] private Label priceLabel;
	[Export] private Button shopButton;

	public static GameInventorySlot? FOCUSED
	{
		get;
		set
		{
			FOCUSED?.OffFocus();
			field = value;
			FOCUSED?.OnFocus();
		}
	}

	private void OnFocus()
	{
		shopOverlay.Show();
		OnThingChange(base.Proxy.BaseThing);
	}
	private void OffFocus()
	{
		shopOverlay.Hide();
	}

	protected override void OnProxySet()
	{
		shopButton.Pressed += () =>
		{
			if (Proxy.Locked)
			{
				Proxy.BuyUnlock();
			}
			else
			{
				if (base.Proxy.BaseThing is Thing thing)
				{
					Proxy.Sell();
				}
			}
			FOCUSED = null;
		};
	}

	public override void _Ready()
	{
		shopOverlay.Hide();
	}

	public override void OnThingChange(Thing? thing)
	{
		Debug.Assert(thing == base.Proxy.BaseThing);
		priceLabel.SelfModulate = Colors.White;
		if (Proxy.Locked)
		{
			var price = Proxy.BuyPrice;
			if (shopButton.Disabled = (Game.INSTANCE.Tickets < price))
			{
				priceLabel.SelfModulate = Colors.Red;
			}
			priceLabel.Text = $"{price}";
			shopButton.Text = "BUY";
		}
		else
		{
			if (thing is null)
			{
				shopButton.Text = "?";
				shopButton.Disabled = true;
			}
			else
			{
				priceLabel.Text = $"{thing.SellPrice}";
				shopButton.Text = "SELL";
				shopButton.Disabled = false;
			}
		}
	}
}
