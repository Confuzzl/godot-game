using Godot;
using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Matcha;

public abstract partial class ThingSlotBase : TextureRect
{
	private static readonly Texture2D LOCKED = Util.GetTexture("gui/thing_lock.png");
	private static readonly Texture2D LOCKED2 = Util.GetTexture("gui/thing_lock2.png");

	public static readonly Vector2 SIZE = new(50, 50);
	public uint Index { get; init; }
	public ThingContainerBase Container { get; init; }

	public bool Locked { get; set; }

	protected bool hovered = false;

	protected ThingSlotBase(uint i, ThingContainerBase c, Vector2 position, Vector2 size)
	{
		Name = $"s{i}";
		Position = position;
		Size = size;
		Debug.Assert(size == SIZE, "thing slot inconsistent size");
		Index = i;
		Container = c;
		StretchMode = StretchModeEnum.KeepCentered;
	}

	public void ShowLock()
	{
		Texture = LOCKED2;
		SelfModulate = Container.SeparatorColor;
	}
	public void HideLock()
	{
		Texture = null;
		SelfModulate = Colors.White;
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

public partial class ThingSlot<T> : ThingSlotBase where T : Things.Thing
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



	public ThingSlot(uint i, ThingContainerBase c, Vector2 p, Vector2 s) : base(i, c, p, s)
	{
		//new Tooltip().Set();

		MouseEntered += () =>
		{
			hovered = true;

			//Gui.MSG = $"{Thing?.TooltipName ?? "nothing"} {Thing?.TriggerBase.Tooltip ?? "nothing"}";
			//Gui.MSG = Foo();
			Game.INSTANCE.Gui.Tooltip.Set(Thing?.TooltipName ?? "nothing", Thing?.Description ?? "nothing", Thing?.TriggerBase.Tooltip ?? "nothing");
		};
		MouseExited += () =>
		{
			hovered = false;
			//Gui.MSG = "";
		};
	}

	private string Foo() => $"{Thing?.TooltipName ?? "none"} {Thing?.TriggerBase?.Tooltip ?? "none"}";
}
