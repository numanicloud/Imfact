using Imfact.Incremental;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Imfact;

internal sealed class GeneralRule
{
    private static GeneralRule? _instance;

    public static GeneralRule Instance => _instance ??= new GeneralRule();

    public bool IsIndirectResolver(IMethodSymbol method)
    {
        if (method.ReturnType is not INamedTypeSymbol named) return false;
        if (named.SpecialType != SpecialType.None) return false;
        return true;
    }

    public bool IsInheritanceTarget(INamedTypeSymbol baseType)
    {
        if (baseType.SpecialType != SpecialType.None) return false;
        return true;
    }

    public bool IsResolverToGenerate(MethodDeclarationSyntax method)
    {
        return method.Modifiers.IndexOf(SyntaxKind.PartialKeyword) != -1
            && method.Modifiers.Any(x =>
                x.IsKind(SyntaxKind.PublicKeyword)
                || x.IsKind(SyntaxKind.PrivateKeyword)
                || x.IsKind(SyntaxKind.ProtectedKeyword)
                || x.IsKind(SyntaxKind.InternalKeyword));
    }

    public bool IsFactoryCandidate(FactoryIncremental factory, AnnotationContext annotations)
    {
        var attributes = factory.Symbol.GetAttributes();
        if (!attributes.Any(IsFactoryAttribute)) return false;

        return true;

        bool IsFactoryAttribute(AttributeData attributeData)
        {
            return SymbolEqualityComparer.Default
                .Equals(attributeData.AttributeClass,
                    annotations.FactoryAttribute);
        }
    }
}
