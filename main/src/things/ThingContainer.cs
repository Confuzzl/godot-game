using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Matcha;

public abstract partial class ThingContainerBase : Node2D
{
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

	protected ThingContainerBase(Color bg, Color sep)
	{
		BackgroundColor = bg;
		SeparatorColor = sep;
	}

	public override void _Ready()
	{
		TriggerParticles = GetNode<GpuParticles2D>("%SlotTriggerParticles");
		TriggerParticles.Amount = 1;
	}
}
public partial class ThingContainer<T>(Color bg, Color sep) : ThingContainerBase(bg, sep) where T : Things.Thing
{
	public readonly ThingSlot<T>[] slots = new ThingSlot<T>[6];

	public override void _Ready()
	{
		base._Ready();
		var slots = GetNode<Node2D>("%Slots");
		for (var i = 0u; i < 6; i++)
		{
			var placeholder = slots.GetNode<TextureRect>($"%s{i}");
			var slot = new ThingSlot<T>(i, this, placeholder.Position, placeholder.Size);
			if (i >= OpenSlots) slot.Locked = true;
			slots.RemoveChild(placeholder);
			slots.AddChild(slot);
			this.slots[i] = slot;
		}
	}

}

public partial class ThingContainer<T> : IEnumerable<ThingSlot<T>>
{
	public ThingSlot<T> this[uint i] => slots[i];

	public class Iterator(ThingSlot<T>[] buffer) : IEnumerator<ThingSlot<T>>
	{
		private readonly ThingSlot<T>[] buffer = buffer;
		private uint index = uint.MaxValue;

		public ThingSlot<T> Current => buffer[index];
		object IEnumerator.Current => Current;

		public void Dispose() { }

		public bool MoveNext() => unchecked(++index) < buffer.Length;
		public void Reset() { index = 0; }
	}

	public IEnumerator<ThingSlot<T>> GetEnumerator() => new Iterator(slots);
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
