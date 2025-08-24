using Godot;
using Matcha;
using Matcha.Things;
using System;

public partial class TestButton : Button
{
    public override void _Ready()
    {
        Pressed += () =>
        {
            foreach (var slot in Game.INSTANCE.Gui.Shop.Characters)
            {
                slot.Thing =
                Thing.CHARACTER_CONSTRUCTORS[
                Random.Shared.Next(0, Thing.CHARACTER_CONSTRUCTORS.Count)
                ]();
            }
            foreach (var slot in Game.INSTANCE.Gui.Shop.Items)
            {
                slot.Thing =
                Thing.ITEM_CONSTRUCTORS[
                Random.Shared.Next(0, Thing.ITEM_CONSTRUCTORS.Count)
                ]();
            }
        };
    }
}
