using System.Collections.Generic;

namespace Matcha.Things;

using Trigger = Generator.Attributes.Trigger;

[Trigger::Container]
public static partial class Triggers
{
    public abstract class Base
    {
        public List<Thing> Things { get; } = [];
        public string Tooltip { get; init; }

        public void Trigger()
        {
            foreach (var thing in Things)
                thing.Trigger();
        }
    }

    public partial class OnSend : Base;
    public partial class OnGrab : Base;
    public partial class OnDeposit : Base;
    public partial class OnRestock : Base;
    public partial class OnMerge : Base;
    public partial class OnPassRoundGoal : Base;

    [Trigger::Timed]
    public abstract class Timed(double i) : Base()
    {
        public double TimeSinceLastTriggered { get; set; } = 0;
        public double Interval { get; } = i;
    }
    public partial class EveryHalfSecond() : Timed(0.5);
    public partial class EveryOneSecond() : Timed(1.0);
    public partial class EveryTwoSecond() : Timed(2.0);
}



