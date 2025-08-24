using Godot;
using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Matcha.Things;

using Generator.Attributes.Thing;
using System.Collections.Generic;


[Base]
public abstract partial class Thing(Texture2D texture, Triggers.Base triggerBase)
{
	public static readonly List<Func<Character>> CHARACTER_CONSTRUCTORS = [];
	public static readonly List<Func<Item>> ITEM_CONSTRUCTORS = [];

	public Texture2D Texture { get; init; } = texture;
	public ThingSlotBase? Slot { get; set; }
	public bool Active
	{
		get;
		set
		{
			if (field = value) TriggerBase?.Things.Add(this);
			else TriggerBase?.Things.Remove(this);
		}
	}
	public uint Price { get; set; }
	public const double SELL_MULTIPLIER = 0.5;
	public uint SellPrice => (uint)Math.Max(1, Price * SELL_MULTIPLIER);
	public bool Upgraded { get; set; }

	public string TooltipName { get; init; }
	public string Description { get; init; }


	public Triggers.Base? TriggerBase => triggerBase;

	public void Trigger()
	{
		Debug.Assert(Active, "Thing isn't active");
		Debug.Assert(Slot is not null, "Slot is null");
		Debug.Assert(Slot is IInventorySlot, "Slot isn't an IInventorySlot");
		((IInventorySlot)Slot).Trigger();
		TriggerImpl();
	}

	protected virtual ImmutableArray<Vector2> TriggerTargets => [];
	protected virtual void TriggerImpl() { }
}

[CharacterBase]
public abstract partial class Character(string name, Triggers.Base triggerBase) : Thing(Util.GetTexture($"characters/{name}.png"), triggerBase)
{
	[
		ResourceName("chiikawa2"),
		TooltipName("evil!"),
		TooltipDescription("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."),
		TriggeredBy<Triggers.OnRestock>,
		Price(4),
	]
	public partial class Chiikawa { protected override void TriggerImpl() { GD.Print("CHIIKAWA!"); } }

	[TriggeredBy<Triggers.EveryHalfSecond>]
	public partial class Hachiware;

	[TriggeredBy<Triggers.OnMerge>]
	public partial class Usagi;

	public partial class Kurimanju;

	public partial class Momonga;

	public partial class Rakko;
}

[ItemBase]
public partial class Item(string name, Triggers.Base triggerBase) : Thing(Util.GetTexture($"items/{name}.png"), triggerBase)
{
	public partial class Matcha;

	public partial class Boba;

	public partial class Catnip;

	public partial class String;

	public partial class MouseToy;

	public partial class DinoNugget;

	public partial class ChoccyMilk;
}
