using Godot;

namespace Matcha.Things;
public partial class GameSlot : TextureRect
{
    public ThingSlotBase Proxy;

    public GameSlot()
    {
        PivotOffset = Size / 2;
        StretchMode = StretchModeEnum.KeepCentered;
    }

    public override void _Process(double delta) => Proxy.ProcessCallback(delta);
    public override void _Input(InputEvent @event) => Proxy.InputCallback(@event);
}
