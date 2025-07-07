using Godot;
using System;

namespace Matcha;

public partial class ThingContainerBase : Node2D
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

	public GpuParticles2D TriggerParticles;

	public override void _Ready()
	{
		TriggerParticles = GetNode<GpuParticles2D>("%SlotTriggerParticles");
		TriggerParticles.Amount = 1;
	}
}
public partial class ThingContainer<T> : ThingContainerBase where T : Things.Thing
{

	public override void _Ready()
	{
		base._Ready();
		var slots = GetNode<Node2D>("%Slots");
		for (var i = 0u; i < 6; i++)
		{
			var placeholder = slots.GetNode<TextureRect>($"%s{i}");
			var slot = new ThingSlot<T>(i, this)
			{
				Name = $"s{i}",
				Size = placeholder.Size,
				Position = placeholder.Position
			};
			slots.RemoveChild(placeholder);
			slots.AddChild(slot);
		}
	}

	public ThingSlot<T> this[uint i] => GetNode<ThingSlot<T>>($"%Slots/s{i}");

}
