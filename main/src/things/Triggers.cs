using System.Collections.Generic;

namespace Matcha.Things;

using Trigger = Generator.Attributes.Trigger;

[Trigger::Container]
public static partial class Triggers
{
    public abstract class Base(string? tooltip = null)
    {
        public List<Thing> Things { get; } = [];
        public string Tooltip { get; init; } = tooltip ?? "???";

        public void Trigger()
        {
            foreach (var thing in Things)
                thing.Trigger();
        }
    }

    public partial class OnSend() : Base("when claw is sent");
    public partial class OnGrab() : Base("when claw grabs");
    public partial class OnDeposit() : Base("when claw deposits");
    public partial class OnRestock() : Base("every restock");
    public partial class OnMerge() : Base("every merge");
    public partial class OnPassRoundGoal() : Base("every time goal is passed");

    [Trigger::Timed]
    public abstract class Timed(double i, string t = null) : Base(t)
    {
        public double TimeSinceLastTriggered { get; set; } = 0;
        public double Interval { get; } = i;
    }
    public partial class EveryHalfSecond() : Timed(0.5, "every 0.5 seconds");
    public partial class EveryOneSecond() : Timed(1.0, "every second");
    public partial class EveryTwoSecond() : Timed(2.0, "every 2 seconds");
}



