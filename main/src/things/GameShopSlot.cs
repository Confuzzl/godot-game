using Godot;

namespace Matcha.Things;
public partial class GameShopSlot : GameSlot
{
    [Export] public HBoxContainer Container { get; private set; }
    [Export] public Label Label { get; private set; }

    public override void OnThingChange(Thing? thing)
    {
        if (Container.Visible = thing is not null)
        {
            Label.Text = $"{thing.Price}";
        }
    }
}
