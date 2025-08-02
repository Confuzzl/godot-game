using Godot;

namespace Matcha;
public partial class Settings : Gui.Window
{
	[Export] public HSlider SFX { get; private set; }
	[Export] public HSlider Music { get; private set; }

	public override void _Ready()
	{
		//SFX = GetNode<HSlider>("%Container/%SFX/HSlider");
		SFX.ValueChanged += (val) =>
		{

		};
		//Music = GetNode<HSlider>("%Container/%Music/HSlider");
		Music.ValueChanged += (val) =>
		{
			Game.INSTANCE.Music.VolumeLinear = (float)val;
		};
	}
}
