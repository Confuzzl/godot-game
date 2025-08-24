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

	[Export] public PhysClaw Claw { get; private set; }

	[ExportGroup("Buttons")]
	[Export] private Button startButton, shuffleButton, endButton;

	[ExportGroup("BG")]
	[Export] private Control front, background;

	[ExportGroup("Walls")]
	[Export] private StaticBody2D worldB, worldL, worldR, worldT;
	[Export] private StaticBody2D chuteSeparator;

	public override void _Ready()
	{
		(var x, var y) = GetViewportRect().Size;
		Position = new(x / 2, y);

		front.Size = new(WIDTH, FRONT_HEIGHT);
		front.Position = new(-LEFT_WIDTH, -FRONT_HEIGHT);

		background.Position = new(-LEFT_WIDTH, -HEIGHT);
		background.Size = new(WIDTH, INSIDE_HEIGHT);


		static void initWall(StaticBody2D w, Vector2 position)
		{
			w.CollisionLayer = Game.WORLD_LAYER;
			w.Position = position;
		}
		initWall(worldB, new(0, -FRONT_HEIGHT));
		initWall(worldL, new(-LEFT_WIDTH, -FRONT_HEIGHT));
		initWall(worldR, new(RIGHT_WIDTH, -FRONT_HEIGHT));
		initWall(worldT, new(0, -HEIGHT));

		chuteSeparator.Position = new(CHUTE_SEPARATOR_X_OFFSET, -FRONT_HEIGHT);
		((SegmentShape2D)chuteSeparator.GetNode<CollisionShape2D>("%Shape").Shape).B = new(0, -CHUTE_SEPARATOR_HEIGHT);
		chuteSeparator.CollisionLayer = Game.WORLD_LAYER;

		ReadyCapsule();
		ReadyState();

		startButton.Pressed += Start;
		endButton.Pressed += () => { if (State == StateEnum.IN_ROUND) State = StateEnum.OUT_OF_TOKENS; };
		shuffleButton.Pressed += () => { if (State == StateEnum.IN_ROUND) Shuffle(); };
	}
	public void Start()
	{
		restocker.BallRadius = 30;
		State = StateEnum.RESTOCKING;
	}
	public bool SendIt()
	{
		if (State != StateEnum.IN_ROUND) return false;
		return Claw.SendIt();
	}
}
