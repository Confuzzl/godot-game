using Godot;
using Matcha.Generator.Attributes;
using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Matcha.Things;


public abstract class Thing(Texture2D texture, Triggers.Base triggerBase)
{
    public Texture2D Texture { get; init; } = texture;
    public ThingSlotBase? Slot { get; set; }
    public bool Active
    {
        get;
        set
        {
            field = value;
            if (value) { triggerBase?.Things.Add(this); }
            else { triggerBase?.Things.Remove(this); }
            //GD.Print($"active {triggerBase?.GetType().Name} {triggerBase?.Things.Count}");
        }
    }
    protected Triggers.Base triggerBase = triggerBase;

    public void Trigger()
    {
        Debug.Assert(Active && Slot is not null, "Thing triggered when inactive or null slot");
        //GD.Print($"activating {Slot?.Index}");
        Slot.Trigger();
        TriggerImpl();
    }
    //protected virtual Vector2? TriggerTarget() => null;
    protected virtual ImmutableArray<Vector2> TriggerTargets() => [];
    protected virtual void TriggerImpl() { }
}

[ThingType]
public partial class Character(string name, Triggers.Base triggerBase) : Thing(Util.GetTexture($"characters/{name}.png"), triggerBase)
{
    [TriggeredBy<Triggers.OnRestock>]
    public partial class Chiikawa { protected override void TriggerImpl() { GD.Print("CHIIKAWA!"); } }

    //[TriggeredBy<Triggers.OnGrab>]
    public partial class Hachiware;

    //[TriggeredBy<Triggers.OnMerge>]
    public partial class Usagi;
}

[ThingType]
public partial class Item(string name, Triggers.Base triggerBase) : Thing(Util.GetTexture($"items/{name}.png"), triggerBase)
{
    //[TriggeredBy<Triggers.OnPassRoundGoal>]
    public partial class Matcha;
    //[TriggeredBy<Triggers.OnRestock>]
    public partial class Boba;
}
