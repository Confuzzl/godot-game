using System;
using System.Collections.Generic;
using System.Text;

namespace Matcha.Generator.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ResourceNameAttribute(string _) : Attribute { }
[AttributeUsage(AttributeTargets.Class)]
public class ThingTypeAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public class TriggerContainerAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Class)]
public class TimedTriggerAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TriggeredByAttribute<T> : Attribute { }