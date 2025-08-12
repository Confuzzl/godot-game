using Matcha.Generator.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Text;

namespace Matcha.Generator;

using Thing = Attributes.Thing;

[Generator]
public sealed class ThingGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = Util.ClassesWithAttribute<Thing::BaseType>(context);

        context.RegisterSourceOutput(provider, Execute);
    }

    private static void MaybeSet<AttrT, T>(AttributeData attr, ref T field)
    {
        string fullname = attr.AttributeClass!.ToDisplayString();
        if (fullname == typeof(AttrT).FullName)
        {
            field = (T)attr.ConstructorArguments[0].Value!;
        }
    }

    private static void Execute(SourceProductionContext source, GeneratorSyntaxContext syntax)
    {
        var klassSyntax = (ClassDeclarationSyntax)syntax.Node;
        string containerName = klassSyntax.Identifier.ToString();

        StringBuilder builder = new();
        builder.AppendLine($$"""
            namespace Matcha.Things;
            public partial class {{containerName}}
            {
            """);
        foreach (INamedTypeSymbol thing in syntax.SemanticModel.GetDeclaredSymbol(klassSyntax)!.GetTypeMembers())
        {
            string thingName = thing.Name;
            string textureName = thingName.ToLower();
            string tooltip = string.Concat(thingName.Select(
                (x, i) => ((i > 0 && char.IsUpper(x)) ? " " : "") + x.ToString()));
            string description = "no description";
            uint price = 0;
            ITypeSymbol? trigger = null;


            foreach (AttributeData attr in thing.GetAttributes())
            {
                //string fullname = attr.AttributeClass!.ToDisplayString();

                MaybeSet<Thing::ResourceName, string>(attr, ref textureName);
                MaybeSet<Thing::TooltipName, string>(attr, ref tooltip);
                MaybeSet<Thing::TooltipDescription, string>(attr, ref description);
                MaybeSet<Thing::Price, uint>(attr, ref price);

                if (attr.AttributeClass!.MetadataName == typeof(Thing::TriggeredBy<object>).GetGenericTypeDefinition().Name)
                    trigger = attr.AttributeClass.TypeArguments[0];
            }

            builder.AppendLine($$"""
                    public partial class {{thingName}} : {{containerName}}
                    {
                        public {{thingName}}() : base("{{textureName}}", {{(trigger is null ? "null" : $"Triggers.{Util.CapsUnderscore(trigger.Name)}")}})
                        {
                            Price = {{price}};
                            TooltipName = "{{tooltip}}";
                            Description = "{{description}}";
                        }
                    }
                """);
        }
        builder.AppendLine("}");

        source.AddSource($"{containerName}.g.cs", builder.ToString());
    }
}
