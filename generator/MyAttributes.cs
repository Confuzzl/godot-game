using System;

namespace Matcha.Generator.Attributes
{
    namespace Thing
    {
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)] public class BaseType : Attribute { }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)] public class ResourceName(string _) : Attribute { }
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)] public class Price(uint _) : Attribute { }
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)] public class TooltipName(string _) : Attribute { }
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)] public class TooltipDescription(string _) : Attribute { }
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)] public class TriggeredBy<T> : Attribute { }
    }

    namespace Trigger
    {
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)] public class Container : Attribute { }
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)] public class Timed : Attribute { }
    }
}


