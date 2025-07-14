using Godot;
using System;

namespace Matcha;
public partial class Shop : Gui.Window
{
	private static void EachLockedSlot<T>(ThingContainer<T> container, Action<ThingSlotBase> func) where T : Things.Thing
	{
		foreach (var slot in container)
		{
			if (slot.Locked) func(slot);
		}
	}
	private static void EachContainer(Action<ThingSlotBase> func)
	{
		EachLockedSlot(Game.INSTANCE.Gui.Characters, func);
		EachLockedSlot(Game.INSTANCE.Gui.Items, func);
	}

	public override void OnOpen()
	{
		EachContainer(static slot => slot.ShowLock());
	}
	public override void OnClose()
	{
		EachContainer(static slot => slot.HideLock());
	}
}
