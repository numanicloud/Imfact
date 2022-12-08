using Imfact.Incremental;
using Imfact.Main;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Xml;

namespace Imfact;

internal sealed class GeneralRule
{
    private static GeneralRule? _instance;

    public static GeneralRule Instance => _instance ??= new GeneralRule();

    public bool IsFactoryClassDeclaration(SyntaxNode node)
    {
        return node is TypeDeclarationSyntax { AttributeLists.Count: > 0 } syntax
            && node is not InterfaceDeclarationSyntax
            && syntax.Modifiers.IndexOf(SyntaxKind.PartialKeyword) != -1;
    }

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

    public bool IsDelegation(IPropertySymbol property, AnnotationContext annotations, Logger logger)
    {
        return property.Type.GetAttributes()
            .Select(x =>
            {
                var attr = x.AttributeClass;
                if (attr is null)
                {
                    logger.Debug($$"""
                        One AttributeData of {{property.Type.Name}} is null.
                        """);
                }
                else
                {
                    logger.Debug($$"""
                        {{attr.Name}} is an attribute of {{property.Type.Name}}.
                        """);
                }
                return attr;
            })
            .FilterNull()
            .Any(x =>
            {
                var judge = SymbolEqualityComparer.Default.Equals(x.OriginalDefinition, annotations.FactoryAttribute);
                if (judge)
                {
                    logger.Debug($$"""
                        {{x.GetFullNameSpace()}}.{{x.Name}} is equal to {{annotations.FactoryAttribute.Name}}.
                        """);
                }
                else
                {
                    logger.Debug($$"""
                        {{x.GetFullNameSpace()}}.{{x.Name}} is not equal to {{annotations.FactoryAttribute.Name}}.
                        """);
                }
                return judge;
            });
    }
}
