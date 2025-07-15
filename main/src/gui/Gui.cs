using Godot;
using Matcha.Things;
using System;

namespace Matcha;
public partial class Gui : CanvasLayer
{
	public static string MSG = "";

	public abstract partial class Window : Node2D
	{
		public virtual void OnOpen() { }
		public virtual void OnClose() { }
	}

	public Window? ActiveWindow { get; private set; } = null;
	[Export] public Shop Shop { get; private set; }
	[Export] public Stats Stats { get; private set; }
	[Export] public Settings Settings { get; private set; }

	[Export] public CharacterContainer Characters { get; private set; }
	[Export] public ItemContainer Items { get; private set; }

	[Export] public Tooltip Tooltip { get; private set; }

	public override void _Ready()
	{
		//Shop = GetNode<Shop>("%Shop");
		//Stats = GetNode<Stats>("%Stats");
		//Settings = GetNode<Settings>("%Settings");
		CloseWindow(Shop);
		CloseWindow(Stats);
		CloseWindow(Settings);

		//Characters = GetNode<CharacterContainer>("%Characters");
		Characters[0].Thing = new Character.Chiikawa();
		Characters[1].Thing = new Character.Hachiware();
		Characters[2].Thing = new Character.Usagi();

		//Items = GetNode<ItemContainer>("%Items");
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
		label.Text = MSG;
		//GetNode<Label>("Label").Text = $"{Game.INSTANCE.Machine.MyState} {Game.INSTANCE.Machine.Claw.MyState}";
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
