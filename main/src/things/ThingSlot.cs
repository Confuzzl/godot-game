using Godot;
using System.Diagnostics;

namespace Matcha.Things;

public abstract partial class ThingSlotBase
{
	public GameSlot GameNode { get; private set; }

	public static readonly Vector2 SIZE = new(50, 50);
	public uint Index { get; private set; }

	private Vector2 defaultPosition;
	public ThingContainerBase Container { get; init; }

	public static ThingSlotBase? CURRENT_DRAGGING { get; private set; }

	// null if is dragging
	public static ThingSlotBase? HOVERING { get; private set; }

	public bool Hovered
	{
		get;
		set
		{
			if (field != value)
				if (value) OnHover();
				else OffHover();
			field = value;
		}
	} = false;
	public bool Dragged
	{
		get;
		set
		{
			if (field != value)
				if (value) OnDrag();
				else OffDrag();
			field = value;
		}
	} = false;


	protected ThingSlotBase(uint i, ThingContainerBase c, GameSlot GameNode)
	{
		Index = i;
		this.GameNode = GameNode;
		GameNode.Proxy = this;
		defaultPosition = GameNode.Position;
		Container = c;
	}

	private void OnDrag()
	{
		const float SCALE = 1.25f;
		CURRENT_DRAGGING = this;
		HOVERING = null;
		GameNode.Scale = new(SCALE, SCALE);
		GameNode.ZIndex = 1;

		OnDragImpl();
	}
	private void OffDrag()
	{
		CURRENT_DRAGGING = null;
		GameNode.Scale = Vector2.One;
		GameNode.ZIndex = 0;

		if (SuccessfulDragCondition)
		{
			SuccessfulOffDragImpl();
		}
		else
		{
			UnsuccessfulOffDragImpl();
			GameNode.Position = defaultPosition;
		}
	}
	protected abstract bool SuccessfulDragCondition { get; }
	protected abstract void SuccessfulOffDragImpl();
	protected abstract void UnsuccessfulOffDragImpl();
	protected abstract void OnDragImpl();

	protected void OnHover()
	{
		if (CURRENT_DRAGGING != this)
			HOVERING = this;
		OnHoverTooltip();
	}
	protected void OffHover()
	{
		if (HOVERING == this)
			HOVERING = null;
		OffHoverTooltip();
		DragReturn();
	}
	protected abstract void OnHoverTooltip();
	protected abstract void OffHoverTooltip();

	private void DragReturn()
	{
		Dragged = false;
		HOVERING?.OnHover();
	}

	public virtual void ProcessCallback(double delta)
	{
		Rect2 aabb = new(GameNode.Position, GameNode.Size);
		Hovered = aabb.HasPoint(Container.GetLocalMousePosition());
	}

	public abstract void InputCallback(InputEvent @event);

	public static void Swap(ThingSlotBase a, ThingSlotBase b)
	{
		Debug.Assert(a.GetType() == b.GetType());
		Debug.Assert(a.Container == b.Container);
		//(a.Container, b.Container) = (b.Container, a.Container);
		(a.GameNode, b.GameNode) = (b.GameNode, a.GameNode);
		(a.Index, b.Index) = (b.Index, a.Index);
		(a.defaultPosition, b.defaultPosition) = (b.defaultPosition, a.defaultPosition);
		a.GameNode.Position = a.defaultPosition;
		b.GameNode.Position = b.defaultPosition;
	}
}

public abstract partial class ThingSlot<T>(uint i, ThingContainerBase c, GameSlot g) :
	ThingSlotBase(i, c, g) where T : Thing
{
	public virtual T? Thing
	{
		get;
		set
		{
			field?.Active = false;
			field = value;
			if (field is null) return;
			field.Slot = this;
			field.Active = ActivateThingOnSet;
			GameNode.Texture = field.Texture;
		}
	}
	protected abstract bool ActivateThingOnSet { get; }

	public override void InputCallback(InputEvent @event)
	{
		if (!Hovered || Thing is null) return;

		if (@event is InputEventMouseButton mouse)
		{
			Dragged = mouse.Pressed && DragCondition;
		}
		if (@event is InputEventMouseMotion move)
		{
			if (!Dragged) return;
			GameNode.Position += move.Relative;
		}
	}
	protected virtual bool DragCondition => true;

	protected abstract void OnHoverTooltipImpl();
	protected override void OnHoverTooltip()
	{
		if (CURRENT_DRAGGING is not null) return;
		OnHoverTooltipImpl();
	}
	protected override void OffHoverTooltip()
	{
		Game.INSTANCE.Gui.Tooltip.Hide();
	}
}
public interface IInventorySlot
{
	public abstract void Trigger();
	public abstract void ShowLock();
	public abstract void HideLock();
}
public partial class InventorySlot<T>(uint i, ThingContainerBase c, GameSlot g) :
	ThingSlot<T>(i, c, g), IInventorySlot where T : Thing
{
	protected override bool ActivateThingOnSet => true;

	public new InventoryContainer<T> Container => (InventoryContainer<T>)base.Container;


	private static readonly Texture2D LOCKED = Util.GetTexture("gui/thing_lock2.png");
	public bool Locked
	{
		get;
		set { if (field = value) ShowLock(); }
	}
	public void ShowLock()
	{
		GameNode.Texture = LOCKED;
		GameNode.SelfModulate = Container.SeparatorColor;
	}
	public void HideLock()
	{
		GameNode.Texture = null;
		GameNode.SelfModulate = Colors.White;
	}

	public void Trigger()
	{
		Container.TriggerParticles.EmitParticle(
			Transform2D.Identity.Translated(GameNode.GlobalPosition + SIZE / 2),
			Vector2.Zero,
			Colors.Blue,
			Colors.White,
			(uint)(GpuParticles2D.EmitFlags.Position | GpuParticles2D.EmitFlags.Color)
		);
	}

	public override void InputCallback(InputEvent @event)
	{
		if (Locked) return;
		base.InputCallback(@event);
	}

	protected override void OnHoverTooltipImpl()
	{
		if (Thing is null)
		{
			if (Locked) Game.INSTANCE.Gui.Tooltip.Set("Locked Slot", "", "");
		}
		else
		{
			Game.INSTANCE.Gui.Tooltip.Set(Thing.TooltipName, Thing.Description, Thing.TriggerBase?.Tooltip ?? "no trigger");
		}
	}

	protected override bool SuccessfulDragCondition =>
		HOVERING is InventorySlot<T> hov && Container == hov.Container && !hov.Locked;

	protected override void OnDragImpl() { }

	protected override void SuccessfulOffDragImpl()
	{
		Swap(this, HOVERING);
		OnHover();
	}
	protected override void UnsuccessfulOffDragImpl() { }
}
public partial class ShopSlot<T>(uint i, ThingContainerBase c, GameShopSlot g) :
	ThingSlot<T>(i, c, g) where T : Thing
{
	public override T? Thing
	{
		get => base.Thing;
		set
		{
			base.Thing = value;
			if (g.Container.Visible = value is not null)
			{
				g.Label.Text = $"{value.Price}";
			}
		}
	}

	protected override bool ActivateThingOnSet => false;

	protected override void OnHoverTooltipImpl()
	{
		if (Thing is not null)
			Game.INSTANCE.Gui.Tooltip.Set(Thing.TooltipName, Thing.Description, Thing.TriggerBase?.Tooltip ?? "no trigger");
	}

	protected override bool DragCondition => Game.INSTANCE.Tickets >= Thing.Price;

	protected override bool SuccessfulDragCondition =>
		HOVERING is InventorySlot<T> hov && hov.Thing is null && !hov.Locked;

	protected override void SuccessfulOffDragImpl()
	{
		((InventorySlot<T>)HOVERING).Thing = Thing;
		Thing = null;
	}
	protected override void UnsuccessfulOffDragImpl()
	{
		Game.INSTANCE.Tickets += Thing.Price;
	}

	protected override void OnDragImpl()
	{
		Game.INSTANCE.Tickets -= Thing.Price;
	}
}
