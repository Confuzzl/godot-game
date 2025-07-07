using Godot;
using Matcha.Things;
using System;

namespace Matcha;
public partial class Gui : CanvasLayer
{
	public Gui()
	{

	}

	public override void _Ready()
	{
		base._Ready();
		// Offset = -GetViewport().GetVisibleRect().Size / 2;

		var characters = GetNode<CharacterContainer>("%Characters");
		characters.BackgroundColor = new("#071900");
		characters.SeparatorColor = new("#00ff00");
		characters[0].Thing = new Character.Chiikawa();
		characters[1].Thing = new Character.Hachiware();
		characters[2].Thing = new Character.Usagi();

		var items = GetNode<ItemContainer>("%Items");
		items.BackgroundColor = new("#000A19");
		items.SeparatorColor = new("#0000ff");
		items[0].Thing = new Item.Matcha();
		items[1].Thing = new Item.Boba();
	}


	public override void _Process(double delta)
	{
		// base._Process(delta);
		GetNode<Label>("Label").Text = $"{Game.INSTANCE.Machine.MyState} {Game.INSTANCE.Machine.Claw.MyState}";
		// GetNode<Label>("Label2").Text = $"{Game.INSTANCE.machine.Claw.timeSinceGrab:0.00} {Game.INSTANCE.machine.Claw.dropTime:0.00}";
		// GetNode<Label>("Label3").Text = $"{Game.INSTANCE.machine.Claw.Position}";
	}

}
