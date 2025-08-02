using Godot;
using System;

namespace Matcha;

using Things;
public partial class Shop : Gui.Window
{
	private static void EachLockedSlot<T>(InventoryContainer<T> container, Action<IInventorySlot> func) where T : Things.Thing
	{
		foreach (var slot in container)
		{
			if (slot.Locked) func(slot);
		}
	}
	private static void EachContainer(Action<IInventorySlot> func)
	{
		EachLockedSlot(Game.INSTANCE.Gui.Characters, func);
		EachLockedSlot(Game.INSTANCE.Gui.Items, func);
	}

	[Export] private ShopCharacterContainer Characters;
	[Export] private ShopItemContainer Items;

	public override void _Ready()
	{
		Characters[0].Thing = new Character.Chiikawa() { Price = 0 };
		Items[0].Thing = new Item.Boba();
	}

	public override void OnOpen()
	{
		//EachContainer(static slot => slot.ShowLock());
	}
	public override void OnClose()
	{
		//EachContainer(static slot => slot.HideLock());
	}
}
