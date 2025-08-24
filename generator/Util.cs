using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matcha.Generator;
public static class Util
{
    //public static bool DerivesFrom<T>(INamedTypeSymbol? symbol)
    //{
    //    if (symbol is null) return false;
    //    if (symbol.ToDisplayString() == typeof(T).FullName) return true;
    //    return DerivesFrom<T>(symbol.BaseType);
    //}


    public static bool HasAttribute<T>(INamedTypeSymbol symbol)
    {
        foreach (var attr in symbol.GetAttributes())
            if (attr.AttributeClass!.ToDisplayString() == typeof(T).FullName)
                return true;
        return false;
    }
    public static AttributeData? GetAttribute<T>(INamedTypeSymbol symbol)
    {
        foreach (var attr in symbol.GetAttributes())
            if (attr.AttributeClass!.ToDisplayString() == typeof(T).FullName)
                return attr;
        return null;
    }
    public static string CapsUnderscore(string str) => string.Concat(str.Select(
                (x, i) => ((i > 0 && char.IsUpper(x)) ? "_" : "") + x.ToString().ToUpperInvariant()));
    public static string LowerUnderscore(string str) => string.Concat(str.Select(
                (x, i) => ((i > 0 && char.IsUpper(x)) ? "_" : "") + x.ToString().ToLowerInvariant()));

    public static IncrementalValuesProvider<GeneratorSyntaxContext>
        ClassesWithAttribute<T>(IncrementalGeneratorInitializationContext context) =>
        context.SyntaxProvider
            .CreateSyntaxProvider(
            predicate: static (sn, _) => sn is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
            transform: static (sc, _) => ClassDeclarationTransform<T>(sc)
            ).Where(context => context.HasValue).Select((pair, _) => pair!.Value);

    private static GeneratorSyntaxContext? ClassDeclarationTransform<T>(GeneratorSyntaxContext context)
    {
        var klass = (ClassDeclarationSyntax)context.Node;


        foreach (var attrList in klass.AttributeLists)
        {
            foreach (var attr in attrList.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attr).Symbol is not ISymbol symbol) continue;
                if (symbol.ContainingType.ToDisplayString() == typeof(T).FullName)
                {
                    return context;
                }
            }
        }
        return null;
    }
}