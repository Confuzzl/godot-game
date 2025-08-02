using Godot;

namespace Matcha.Things;
public partial class GameShopSlot : GameSlot
{
    [Export] public HBoxContainer Container { get; private set; }
    [Export] public Label Label { get; private set; }
}
