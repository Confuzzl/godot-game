using Godot;
using System.Diagnostics;

namespace Matcha.Things;


public abstract partial class ThingSlotBase
{
	public abstract Thing? BaseThing { get; }
	public GameSlot GameNode { get; protected set; }

	public static readonly Vector2 SIZE = new(50, 50);
	public uint Index { get; protected set; }

	protected Vector2 DefaultPosition
	{
		get;
		private set;
		//set
		//{
		//    field = value;
		//    GameNode.Position = value;
		//}
	}
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
		Container = c;

		Callable.From(() =>
		{
			DefaultPosition = GameNode.Position;
		}).CallDeferred();
	}

	private void OnDrag()
	{
		const float SCALE = 1.5f;
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
			GameNode.Position = DefaultPosition;
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

		Dragged = false;
		HOVERING?.OnHover();
	}
	protected abstract void OnHoverTooltip();
	protected abstract void OffHoverTooltip();

	public virtual void ProcessCallback(double delta)
	{
		Rect2 aabb = new(GameNode.Position, GameNode.Size);
		Hovered = aabb.HasPoint(Container.GetLocalMousePosition());
	}

	public abstract void InputCallback(InputEvent @event);
}

public abstract partial class ThingSlot<T>(uint i, ThingContainerBase c, GameSlot g) : ThingSlotBase(i, c, g)
	where T : Thing
{
	public override Thing? BaseThing => Thing;
	public T? Thing
	{
		get;
		set
		{
			field?.Active = false;
			field = value;
			GameNode.OnThingChange(field);
			GameNode.Texture = field?.Texture ?? NullThingTexture;
			if (field is null) return;
			field.Slot = this;
			field.Active = ActivateThingOnSet;
		}
	}
	protected virtual Texture2D? NullThingTexture => null;
	protected abstract bool ActivateThingOnSet { get; }

	public override void InputCallback(InputEvent @event)
	{
		if (!Hovered) return;

		if (@event is InputEventMouseButton mouse)
		{
			MouseEventCallback(mouse);
			if (Thing is null) return;
			Dragged = GameInventorySlot.FOCUSED != GameNode &&
				mouse.ButtonIndex == MouseButton.Left && mouse.Pressed && DragCondition;
		}

		if (@event is InputEventMouseMotion move)
		{
			if (!Dragged) return;
			GameNode.Position += move.Relative;
		}
	}
	protected virtual bool DragCondition => true;
	protected virtual void MouseEventCallback(InputEventMouseButton mouse) { }

	protected abstract void OnHoverTooltipImpl();
	protected sealed override void OnHoverTooltip()
	{
		if (CURRENT_DRAGGING is not null) return;
		OnHoverTooltipImpl();
	}
	protected sealed override void OffHoverTooltip()
	{
		Game.INSTANCE.Gui.Tooltip.Hide();
	}
}
public interface IInventorySlot
{
	public bool Locked { get; }
	public abstract void Trigger();
	public abstract void Sell();
	public abstract void BuyUnlock();
	public abstract uint? BuyPrice { get; }
}
public partial class InventorySlot<T> :
	ThingSlot<T>, IInventorySlot where T : Thing
{
	private new readonly GameInventorySlot GameNode;

	protected override Texture2D NullThingTexture => Locked ? LOCKED : null;

	public InventorySlot(uint i, InventoryContainer<T> c, GameInventorySlot g) : base(i, c, g)
	{
		GameNode = g;
		if (Locked = i >= c.OpenSlots)
		{
			GameNode.Texture = LOCKED;
			GameNode.SelfModulate = Container.SeparatorColor;
		}
		Thing = null;
	}

	protected override bool ActivateThingOnSet => true;

	public new InventoryContainer<T> Container => (InventoryContainer<T>)base.Container;


	private static readonly Texture2D LOCKED = Util.GetTexture("gui/thing_lock2.png");
	public bool Locked { get; private set; }

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

	protected override void MouseEventCallback(InputEventMouseButton mouse)
	{
		if (Game.INSTANCE.Gui.ActiveWindow is Shop &&
			mouse.ButtonIndex == MouseButton.Right)
		{
			GameInventorySlot.FOCUSED = GameNode;
		}
	}
	public override void InputCallback(InputEvent @event)
	{
		if (!Locked && Thing is null) return;
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
		static void swap(InventorySlot<T> a, InventorySlot<T> b)
		{
			Debug.Assert(a.GetType() == b.GetType());
			Debug.Assert(a.Container == b.Container);

			(a.Thing, b.Thing) = (b.Thing, a.Thing);
			a.GameNode.Position = a.DefaultPosition;
			b.GameNode.Position = b.DefaultPosition;
		}

		swap(this, (InventorySlot<T>)HOVERING);
		OnHover();
	}
	protected override void UnsuccessfulOffDragImpl() { }

	public void Sell()
	{
		Game.INSTANCE.Tickets += Thing.SellPrice;
		Thing = null;
	}
	public void BuyUnlock()
	{
		Game.INSTANCE.Tickets -= Container.LockedSlotPrice;
		Locked = false;
		Thing = null;
		GameNode.SelfModulate = Colors.White;
	}

	public uint? BuyPrice => Locked ? Container.LockedSlotPrice : null;
}
public partial class ShopSlot<T> :
	ThingSlot<T> where T : Thing
{
	public new GameShopSlot GameNode;

	public ShopSlot(uint i, ThingContainerBase c, GameShopSlot g) : base(i, c, g)
	{
		Thing = null;
		GameNode = g;
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
		GameNode.Position = DefaultPosition;
		GameNode.Texture = null;
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
