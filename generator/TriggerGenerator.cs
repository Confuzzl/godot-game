using Matcha.Generator.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matcha.Generator;



[Generator]
public sealed class TriggerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        {
            var provider = GeneratorUtil.ClassesWithAttribute<TriggerContainerAttribute>(context);
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
            string caps = GeneratorUtil.CapsUnderscore(nestedKlassName);
            builder.AppendLine($"\tpublic static readonly {nestedKlassName} {caps} = new();");
            if (GeneratorUtil.HasAttribute<TimedTriggerAttribute>(nestedClass.BaseType!))
            {
                timers.Add(caps);
            }
        }
        builder.AppendLine($"\tpublic static readonly Timed[] TIMERS = [{string.Join(", ", timers)}];");
        builder.AppendLine("}");
        source.AddSource($"{klassName}.g.cs", builder.ToString());
    }
}
