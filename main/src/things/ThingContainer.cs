using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Matcha.Things;

public abstract partial class ThingContainerBase : Control
{
	public abstract Vector2 DefaultPositionOfSlot(uint i);
}

public abstract partial class ThingContainer<T, SlotType>(uint numSlots) : ThingContainerBase, IEnumerable<SlotType>
	where T : Thing where SlotType : ThingSlotBase
{
	protected readonly SlotType[] slots = new SlotType[numSlots];

	public SlotType this[uint i] => slots[i];

	public IEnumerator<SlotType> GetEnumerator() => slots.AsEnumerable().GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public abstract partial class InventoryContainer<T> : ThingContainer<T, InventorySlot<T>> where T : Thing
{
	private static readonly Vector2 SLOT_OFFSET = new(9, 8);
	public override Vector2 DefaultPositionOfSlot(uint i) => SLOT_OFFSET +
		new Vector2((i % 3) * (ThingSlotBase.SIZE.X + 6), (i / 3) * (ThingSlotBase.SIZE.Y + 4));

	private const float SEPERATOR_BUFFER = 8;

	public Color BackgroundColor
	{
		get;
		set
		{
			field = value;
			GetNode<ColorRect>("%Background").Color = value;
		}
	}
	private StyleBoxLine
		themeH = new() { GrowBegin = -SEPERATOR_BUFFER, GrowEnd = -SEPERATOR_BUFFER, Thickness = 2, Vertical = false },
		themeV = new() { GrowBegin = -SEPERATOR_BUFFER, GrowEnd = -SEPERATOR_BUFFER, Thickness = 2, Vertical = true };
	public Color SeparatorColor
	{
		get;
		set
		{
			field = value;
			themeH.Color = value;
			themeV.Color = value;

			for (var i = 0; i < 3; i++)
				GetNode<HSeparator>($"Separator/%h{i}").AddThemeStyleboxOverride("separator", themeH);
			for (var i = 0; i < 4; i++)
				GetNode<VSeparator>($"Separator/%v{i}").AddThemeStyleboxOverride("separator", themeV);
		}
	}

	public uint OpenSlots { get; protected set; } = 3;

	public GpuParticles2D TriggerParticles;

	protected InventoryContainer(Color bg, Color sep) : base(6)
	{
		BackgroundColor = bg;
		SeparatorColor = sep;
	}

	public override void _Ready()
	{
		TriggerParticles = GetNode<GpuParticles2D>("%SlotTriggerParticles");
		TriggerParticles.Amount = 1;

		var slotContainer = GetNode<Control>("%Slots");
		for (var i = 0u; i < slots.Length; i++)
		{
			var placeholder = slotContainer.GetNode<GameInventorySlot>($"%s{i}");

			Debug.Assert(placeholder.Size == ThingSlotBase.SIZE, "inconsistent thingslot size");

			var slot = new InventorySlot<T>(i, this, placeholder);

			slots[i] = slot;
		}
	}

	public uint LockedSlotPrice = 5;
}

public abstract partial class ShopContainer<T>() : ThingContainer<T, ShopSlot<T>>(6) where T : Thing
{

	private static readonly Vector2 SLOT_OFFSET = new(0, 0);
	public override Vector2 DefaultPositionOfSlot(uint i) => SLOT_OFFSET +
		new Vector2((i % 3) * (ThingSlotBase.SIZE.X + 5), (i / 3) * (ThingSlotBase.SIZE.Y + 30));

	public override void _Ready()
	{
		for (var i = 0u; i < slots.Length; i++)
		{
			var placeholder = GetNode<GameShopSlot>($"%s{i}");

			Debug.Assert(placeholder.Size == ThingSlotBase.SIZE, "inconsistent thingslot size");
			Debug.Assert(placeholder.Position == DefaultPositionOfSlot(i), "inconsistent thingslot position");

			var slot = new ShopSlot<T>(i, this, placeholder);
			slots[i] = slot;
		}
	}
}
