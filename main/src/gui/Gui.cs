using Godot;
using Matcha.Things;
using System;

namespace Matcha;
public partial class Gui : CanvasLayer
{
    public static string MSG = "";

    public abstract partial class Window : Control
    {
        public virtual void OnOpen() { }
        public virtual void OnClose() { }
    }

    public Window? ActiveWindow { get; private set; } = null;

    [ExportGroup("Labels")]
    [Export] public Label TicketLabel { get; private set; }
    [Export] public Label TokenLabel { get; private set; }
    [Export] public Label RoundLabel { get; private set; }

    [ExportGroup("Windows")]
    [Export] public Shop Shop { get; private set; }
    [Export] public Stats Stats { get; private set; }
    [Export] public Settings Settings { get; private set; }

    [ExportGroup("Inventory")]
    [Export] public InventoryCharacterContainer Characters { get; private set; }
    [Export] public InventoryItemContainer Items { get; private set; }

    [ExportGroup("")]
    [Export] public Tooltip Tooltip { get; private set; }

    public override void _Ready()
    {
        CloseWindow(Shop);
        CloseWindow(Stats);
        CloseWindow(Settings);

        Characters[0].Thing = new Character.Chiikawa();
        Characters[1].Thing = new Character.Hachiware();
        Characters[2].Thing = new Character.Usagi();

        Items[0].Thing = new Item.Matcha();
        Items[1].Thing = new Item.Boba();

        Action press(Window window) => () =>
        {
            GetTree().Paused = true;
            CloseWindow();
            OpenWindow(window);
        };

        GetNode<Button>("%ShopButton").Pressed += press(Shop);
        GetNode<Button>("%StatsButton").Pressed += press(Stats);
        GetNode<Button>("%SettingsButton").Pressed += press(Settings);
    }
    public override void _Process(double delta)
    {
        var label = GetNode<Label>("Label");
        //label.Text = MSG;
        label.Text = $"{ThingSlotBase.CURRENT_DRAGGING?.Index ?? uint.MaxValue} {ThingSlotBase.HOVERING?.Index ?? uint.MaxValue}";
        //label.Text = $"{Game.INSTANCE.Machine.MyState} {Game.INSTANCE.Machine.Claw.MyState}";
    }


    public void CloseWindow()
    {
        if (ActiveWindow is null) return;
        CloseWindow(ActiveWindow);
        ActiveWindow = null;
    }
    private void OpenWindow(Window window)
    {
        window.OnOpen();
        window.Visible = true;
        window.SetProcess(true);
        ActiveWindow = window;
    }
    private static void CloseWindow(Window window)
    {
        window.OnClose();
        window.Visible = false;
        window.SetProcess(false);
    }
}
