using Godot;
using System;

namespace Matcha;

public partial class ThingSlotBase : TextureRect
{
	public static readonly Vector2 SIZE = new(50, 50);
	public uint Index { get; init; }
	public ThingContainerBase Container { get; init; }

	public ThingSlotBase(uint i, ThingContainerBase c)
	{
		Index = i;
		Container = c;
		StretchMode = StretchModeEnum.KeepCentered;
	}

	public void Trigger()
	{
		Container.TriggerParticles.EmitParticle(
			Container.Transform.Translated(Position + SIZE / 2),
			Vector2.Zero,
			Colors.Blue,
			Colors.White,
			(uint)(GpuParticles2D.EmitFlags.Position | GpuParticles2D.EmitFlags.Color)
		);
	}
}

public partial class ThingSlot<T>(uint i, ThingContainerBase c) : ThingSlotBase(i, c) where T : Things.Thing
{
	public T Thing
	{
		get;
		set
		{
			field?.Active = false;
			field = value;
			if (field == null) return;
			field.Slot = this;
			field.Active = true;
			Texture = field.Texture;
		}
	}
}
