using Godot;
using System;

namespace Matcha;

using Things;
public partial class Shop : Gui.Window
{
	private static void EachSlot<T>(InventoryContainer<T> container, Action<IInventorySlot> func) where T : Thing
	{
		foreach (var slot in container)
		{
			func(slot);
		}
	}
	private static void EachContainer(Action<IInventorySlot> func)
	{
		EachSlot(Game.INSTANCE.Gui.Characters, func);
		EachSlot(Game.INSTANCE.Gui.Items, func);
	}

	[Export] public ShopCharacterContainer Characters { get; private set; }
	[Export] public ShopItemContainer Items { get; private set; }

	[Export] public VBoxContainer Upgrades { get; private set; }

	public override void _Ready()
	{
		Characters[0].Thing = new Character.Chiikawa();
		Items[0].Thing = new Item.Boba();

		Upgrades.AddChild(
			UpgradeItem.New(
				"Stronger Claw",
				() => $"{Game.INSTANCE.Machine.Claw.DropChance:F1}%",
				"decreases the chance of claw releasing",
				[5, 10, 25, 50, 100],
				() =>
				{
					Game.INSTANCE.Machine.Claw.DropChance -= 0.1f;
				}
			)
		);
		Upgrades.AddChild(
			UpgradeItem.New(
				"Faster Tickets",
				() => $"{Game.INSTANCE.TicketMultiplier}",
				"number of tickets gained per restock",
				[10, 20, 40, 80],
				() =>
				{
					Game.INSTANCE.TicketMultiplier += 1;
				}
			)
		);
		Upgrades.AddChild(
			UpgradeItem.New(
				"Valuable Restocks",
				() => $"{Game.INSTANCE.BallValueMultiplier:F1}",
				"multiplier for each restock ball",
				[10, 25, 50, 100, 250, 500],
				() =>
				{
					Game.INSTANCE.BallValueMultiplier += 0.5f;
				}
			)
		);
	}

	public override void OnOpen()
	{
		TicketUpdate();
		//EachContainer(static slot => slot.OnShopEnter());
	}
	public override void OnClose()
	{
		GameInventorySlot.FOCUSED = null;
		//EachContainer(static slot => slot.OnShopExit());
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouse && mouse.Pressed &&
			(!GameInventorySlot.FOCUSED?.Proxy.Hovered ?? false))
		{
			GameInventorySlot.FOCUSED = null;
		}
	}

	public void TicketUpdate()
	{
		foreach (var slot in Characters)
		{
			if (slot.Thing is null) continue;
			slot.GameNode.TicketUpdate();
		}

		foreach (var i in Upgrades.GetChildren())
		{
			var item = (UpgradeItem)i;
			item.TicketUpdate();
		}
	}
}
