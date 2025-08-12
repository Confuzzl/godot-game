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

    public override void _Ready()
    {
        Characters[0].Thing = new Character.Chiikawa();
        Items[0].Thing = new Item.Boba();
    }

    public override void OnOpen()
    {
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
}
