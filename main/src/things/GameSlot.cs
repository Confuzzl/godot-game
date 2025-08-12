using Godot;
using System.Diagnostics;

namespace Matcha.Things;
public partial class GameSlot : TextureRect
{
	public ThingSlotBase Proxy
	{
		get;
		set
		{
			field = value;
			if (value is not null) OnProxySet();
		}
	}

	protected virtual void OnProxySet() { }

	public GameSlot()
	{
		PivotOffset = Size / 2;
		StretchMode = StretchModeEnum.KeepCentered;
	}

	public override void _Process(double delta)
	{
		Proxy?.ProcessCallback(delta);
	}
	public override void _Input(InputEvent @event)
	{
		Proxy?.InputCallback(@event);
	}

	public virtual void OnThingChange(Thing? thing) { }
}
