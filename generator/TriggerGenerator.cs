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
        {
            var provider = Util.ClassesWithAttribute<Trigger::Container>(context);
            context.RegisterSourceOutput(provider, ExecuteTriggerClasses);

        }

    }

    private static void ExecuteTriggerClasses(SourceProductionContext source, GeneratorSyntaxContext syntax)
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
        foreach (INamedTypeSymbol nestedClass in syntax.SemanticModel.GetDeclaredSymbol(klassSyntax)!.GetTypeMembers())
        {
            if (nestedClass.IsAbstract) continue;

            string nestedKlassName = nestedClass.Name;
            string caps = Util.CapsUnderscore(nestedKlassName);
            builder.AppendLine($"\tpublic static readonly {nestedKlassName} {caps} = new();");
            if (Util.HasAttribute<Trigger::Timed>(nestedClass.BaseType!))
            {
                timers.Add(caps);
            }
        }
        builder.AppendLine($"\tpublic static readonly Timed[] TIMERS = [{string.Join(", ", timers)}];");
        builder.AppendLine("}");
        source.AddSource($"{klassName}.g.cs", builder.ToString());
    }
}
