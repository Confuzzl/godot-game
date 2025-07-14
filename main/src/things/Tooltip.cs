using Godot;
using System;

namespace Matcha;

public partial class Tooltip : ColorRect
{
    public override void _Process(double delta)
    {
        //base._Process(delta);
        Position = Game.INSTANCE.GetLocalMousePosition();
    }

    public void Set(string name, string description, string trigger)
    {

    }
}
