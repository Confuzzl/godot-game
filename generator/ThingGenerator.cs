using Matcha.Generator.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Text;

namespace Matcha.Generator;

[Generator]
public sealed class ThingGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = GeneratorUtil.ClassesWithAttribute<ThingTypeAttribute>(context);

        context.RegisterSourceOutput(provider, Execute);
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
        foreach (INamedTypeSymbol nestedClass in syntax.SemanticModel.GetDeclaredSymbol(klassSyntax)!.GetTypeMembers())
        {
            string nestedKlassName = nestedClass.Name;
            string textureName = nestedKlassName.ToLower();
            ITypeSymbol? trigger = null;

            foreach (AttributeData attr in nestedClass.GetAttributes())
            {
                if (attr.AttributeClass!.ToDisplayString() == typeof(ResourceNameAttribute).FullName)
                    textureName = (string)attr.ConstructorArguments[0].Value!;
                if (attr.AttributeClass.MetadataName == typeof(TriggeredByAttribute<object>).GetGenericTypeDefinition().Name)
                    trigger = attr.AttributeClass.TypeArguments[0];
            }

            builder.AppendLine($$"""
                    public partial class {{nestedKlassName}} : {{containerName}}
                    {
                        public {{nestedKlassName}}() : base("{{textureName}}", {{(trigger is null ? "null" : $"Triggers.{GeneratorUtil.CapsUnderscore(trigger.Name)}")}}) {}
                    }
                """);
        }
        builder.AppendLine("}");

        source.AddSource($"{containerName}.g.cs", builder.ToString());
    }
}
