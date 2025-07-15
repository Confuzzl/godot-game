using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Text;

namespace Matcha.Generator;

using Trigger = Attributes.Trigger;


[Generator]
public sealed class TriggerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = Util.ClassesWithAttribute<Trigger::Container>(context);
        context.RegisterSourceOutput(provider, ExecuteSingletons);
    }

    private static void ExecuteSingletons(SourceProductionContext source, GeneratorSyntaxContext syntax)
    {
        var klassSyntax = (ClassDeclarationSyntax)syntax.Node;
        string klassName = klassSyntax.Identifier.ToString();

        List<string> timers = [];

        StringBuilder builder = new();
        builder.AppendLine($$"""
            namespace Matcha.Things;
            public static partial class {{klassName}}
            {
            """);
        foreach (INamedTypeSymbol trigger in syntax.SemanticModel.GetDeclaredSymbol(klassSyntax)!.GetTypeMembers())
        {
            if (trigger.IsAbstract) continue;

            string nestedKlassName = trigger.Name;
            string caps = Util.CapsUnderscore(nestedKlassName);

            builder.AppendLine($"\tpublic static readonly {nestedKlassName} {caps} = new();");
            if (Util.HasAttribute<Trigger::Timed>(trigger.BaseType!))
            {
                timers.Add(caps);
            }
        }
        builder.AppendLine($"\tpublic static readonly Timed[] TIMERS = [{string.Join(", ", timers)}];");
        builder.AppendLine("}");
        source.AddSource($"{klassName}Singletons.g.cs", builder.ToString());
    }
}
