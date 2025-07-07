using Godot;

namespace Matcha;
public partial class Machine : Node2D
{
	public const float LEFT_WIDTH = 3 * Game.UNIT;
	public const float RIGHT_WIDTH = 5 * Game.UNIT;
	public const float WIDTH = LEFT_WIDTH + RIGHT_WIDTH;
	public const float FRONT_HEIGHT = 1 * Game.UNIT;
	public const float INSIDE_HEIGHT = 7 * Game.UNIT;
	public const float HEIGHT = FRONT_HEIGHT + INSIDE_HEIGHT;

	partial void ReadyCapsule();
	partial void ReadyState();

	public PhysClaw Claw { get; private set; }

	public override void _Ready()
	{
		(var x, var y) = GetViewportRect().Size;
		Position = new(x / 2, y);

		Claw = GetNode<PhysClaw>("%PhysClaw");

		var front = GetNode<TextureRect>("%Front");
		front.Size = new(WIDTH, FRONT_HEIGHT);
		front.Position = new(-LEFT_WIDTH, -FRONT_HEIGHT);

		var bg = GetNode<ColorRect>("%Background");
		bg.Position = new(-LEFT_WIDTH, -HEIGHT);
		bg.Size = new(WIDTH, INSIDE_HEIGHT);
		bg.Color = new("#FFFFFF7F");


		void initWall(string name, Vector2 position)
		{
			var w = GetNode<StaticBody2D>(name);
			w.CollisionLayer = Game.WORLD_LAYER;
			w.Position = position;
		}
		initWall("%WorldB", new(0, -FRONT_HEIGHT));
		initWall("%WorldL", new(-LEFT_WIDTH, -FRONT_HEIGHT));
		initWall("%WorldR", new(RIGHT_WIDTH, -FRONT_HEIGHT));
		initWall("%WorldT", new(0, -HEIGHT));

		var sep = GetNode<StaticBody2D>("%ChuteSeparator");
		sep.Position = new(CHUTE_SEPARATOR_X_OFFSET, -FRONT_HEIGHT);
		((SegmentShape2D)sep.GetNode<CollisionShape2D>("%Shape").Shape).B = new(0, -CHUTE_SEPARATOR_HEIGHT);
		sep.CollisionLayer = Game.WORLD_LAYER;

		ReadyCapsule();
		ReadyState();

		GetNode<Button>("%StartButton").Pressed += Start;
		GetNode<Button>("%EndButton").Pressed += () => { if (MyState == State.IN_GAME) MyState = State.OUT_OF_TOKENS; };
		GetNode<Button>("%ShuffleButton").Pressed += () => { if (MyState == State.IN_GAME) Shuffle(); };
	}

	private void Start()
	{
		GetNode<Button>("%StartButton").Disabled = true;

		restocker.BallRadius = 30;

		MyState = State.RESTOCKING;

	}

	public bool SendIt()
	{
		if (MyState != State.IN_GAME) return false;
		return Claw.SendIt();
	}
}
