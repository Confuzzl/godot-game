using Matcha.Generator.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matcha.Generator;

using Thing = Attributes.Thing;

[Generator]
public sealed class ThingGenerator : IIncrementalGenerator
{
    //private static List<string> things = [];
    private static readonly List<string> characters = [];
    private static readonly List<string> items = [];

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        {
            var provider = Util.ClassesWithAttribute<Thing::CharacterBase>(context);
            context.RegisterSourceOutput(provider, static (source, syntax) => Execute(source, syntax, characters));
        }
        {
            var provider = Util.ClassesWithAttribute<Thing::ItemBase>(context);
            context.RegisterSourceOutput(provider, static (source, syntax) => Execute(source, syntax, items));
        }
        {
            var provider = Util.ClassesWithAttribute<Thing::Base>(context);
            context.RegisterSourceOutput(provider, Execute2);
        }
    }

    private static void MaybeSet<AttrT, T>(AttributeData attr, ref T field)
    {
        string fullname = attr.AttributeClass!.ToDisplayString();
        if (fullname == typeof(AttrT).FullName)
        {
            field = (T)attr.ConstructorArguments[0].Value!;
        }
    }

    private static void Execute(SourceProductionContext source, GeneratorSyntaxContext syntax, List<string> things)
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
            string textureName = /*thingName.ToLower()*/Util.LowerUnderscore(thingName);
            string tooltip = string.Concat(thingName.Select(
                (x, i) => ((i > 0 && char.IsUpper(x)) ? " " : "") + x.ToString()));
            string description = "no description";
            uint price = 0;
            ITypeSymbol? trigger = null;

            things.Add(thingName);

            foreach (AttributeData attr in thing.GetAttributes())
            {
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

    private void Execute2(SourceProductionContext source, GeneratorSyntaxContext syntax)
    {
        StringBuilder staticBuilder = new();
        staticBuilder.AppendLine($$"""
            namespace Matcha.Things; 
            public partial class Thing 
            { 
                static Thing() 
                {
            """);
        foreach (var thing in characters)
        {
            staticBuilder.AppendLine($"\t\tCHARACTER_CONSTRUCTORS.Add(static () => new Character.{thing}());");
        }
        foreach (var thing in items)
        {
            staticBuilder.AppendLine($"\t\tITEM_CONSTRUCTORS.Add(static () => new Item.{thing}());");
        }
        staticBuilder.AppendLine("""
                }
            }
            """);
        source.AddSource("ThingsStatic.g.cs", staticBuilder.ToString());
    }
}
