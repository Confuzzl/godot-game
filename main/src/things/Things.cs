using Godot;
//using Matcha.Generator.Attributes;
using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Matcha.Things;

using Generator.Attributes.Thing;


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
			if (value) TriggerBase?.Things.Add(this);
			else TriggerBase?.Things.Remove(this);
		}
	}
	public uint Price { get; set; } = 4;
	public bool Upgraded { get; set; }

	public string TooltipName { get; init; }
	public string Description { get; init; }


	public Triggers.Base? TriggerBase { get; } = triggerBase;

	public void Trigger()
	{
		Debug.Assert(Active && Slot is not null, "Thing triggered when inactive or null slot");
		((IInventorySlot)Slot).Trigger();
		TriggerImpl();
	}

	protected virtual ImmutableArray<Vector2> TriggerTargets() => [];
	protected virtual void TriggerImpl() { }
}

[BaseType]
public abstract partial class Character(string name, Triggers.Base triggerBase) : Thing(Util.GetTexture($"characters/{name}.png"), triggerBase)
{
	[
		ResourceName("chiikawa2"),
		TooltipName("evil!"),
		TooltipDescription("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."),
		TriggeredBy<Triggers.OnRestock>
	]
	public partial class Chiikawa { protected override void TriggerImpl() { GD.Print("CHIIKAWA!"); } }

	[TriggeredBy<Triggers.EveryHalfSecond>]
	public partial class Hachiware;

	[TriggeredBy<Triggers.OnMerge>]
	public partial class Usagi;
}

[BaseType]
public partial class Item(string name, Triggers.Base triggerBase) : Thing(Util.GetTexture($"items/{name}.png"), triggerBase)
{
	//[TriggeredBy<Triggers.OnPassRoundGoal>]
	public partial class Matcha;
	//[TriggeredBy<Triggers.OnRestock>]
	public partial class Boba;
}
