using Godot;

namespace Matcha.Things;
public partial class GameShopSlot : GameSlot
{
    [Export] public Control Container { get; private set; }
    [Export] public Label Label { get; private set; }

    public void TicketUpdate()
    {
        Label.SelfModulate = Game.INSTANCE.Tickets < Proxy.BaseThing.Price ? Colors.Red : Colors.White;
    }

    public override void OnThingChange(Thing? thing)
    {
        if (Container.Visible = thing is not null)
        {
            TicketUpdate();
            Label.Text = $"{thing.Price}";
        }
    }
}
