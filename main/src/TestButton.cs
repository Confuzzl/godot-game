using Godot;
using System;

public partial class TestButton : Button
{
    public override void _Ready()
    {
        Pressed += () => GD.Print("123");
        Pressed += () => GD.Print("456");
    }
}
