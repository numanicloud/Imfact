using Imfact.Steps.Filter.Wrappers;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imfact.Steps.Filter.Rules;

internal sealed class ClassRule
{
    public required MethodRule MethodRule { private get; init; }

    public IFactoryClassWrapper? TransformClass(TypeDeclarationSyntax type,
        GeneratorSyntaxContext context,
        CancellationToken ct)
    {
        if (type.Modifiers.IndexOf(SyntaxKind.PartialKeyword) == -1) return null;

        var symbol = context.SemanticModel.GetDeclaredSymbol(type, ct);
        if (symbol is null) return null;

        // TODO: Testabilityのために、フィルタリングはTransformではやらずMatchでやる
        var methods = TransformMethods(type, context, ct);
        var baseType = TransformBaseType(symbol, ct);
        var resolutions = TransformResolutions(methods, ct);
        var delegations = TransformDelegations(symbol, ct);

        return new FactorySymbolWrapper()
        {
            Symbol = symbol,
            Methods = methods,
            BaseType = baseType,
            Resolutions = resolutions,
            Delegations = delegations
        };
    }

    public FilteredType? Match(IFactoryClassWrapper type, AnnotationContext annotations, CancellationToken ct)
	{
        if (!IsFactoryCandidate(type, annotations))
            return null;

        var factoryAttributeFullName = annotations.FactoryAttribute.GetFullNameSpace()
            + "."
            + annotations.FactoryAttribute.Name;

        return new FilteredType
        {
            Type = type.GetTypeAnalysis(),

            Methods = type.Methods
                .Select(m => MethodRule.Match(m, annotations, ct))
                .FilterNull()
                .ToArray(),

            BaseFactory = type.BaseType is not { } baseType
                ? null
                : MatchBaseType(baseType),

            ResolutionFactories = type.Resolutions
                .Where(IsFactoryReference)
                .Select(x => new FilteredResolution()
                {
                    Type = x.GetTypeAnalysis(),
                })
                .ToArray(),

            Delegations = type.Delegations
                .Where(IsFactoryReference)
                .Select(x => new FilteredDelegation()
                {
                    Type = x.GetTypeAnalysis()
                })
                .ToArray()
        };

        bool IsFactoryReference(ITypeWrapper reference) =>
            reference.IsConstructableClass
             && reference.GetAttributes()
                .Any(attr => annotations.FactoryAttribute.IsInSameModuleWith(reference)
                    ? annotations.FactoryAttribute.IsUsedAs(attr)
                    : attr.FullName == factoryAttributeFullName);

        FilteredBaseType? MatchBaseType(IBaseFactoryWrapper pivot)
        {
            return !IsFactoryReference(pivot)
                ? null
                : new FilteredBaseType
                {
                    Type = pivot.GetTypeAnalysis(),
                    BaseFactory = pivot.BaseType is not { } bt
                        ? null
                        : MatchBaseType(bt),
                    Methods = pivot.Methods
                        .Select(x => MethodRule.MatchAsInheritance(x, annotations, ct))
                        .FilterNull()
                        .ToArray()
                };
		}
	}

    private bool IsFactoryCandidate(IFactoryClassWrapper symbol, AnnotationContext annotations)
    {
        return symbol.GetAttributes()
            .Any(annotations.FactoryAttribute.IsUsedAs);
    }

    private IResolverWrapper[] TransformMethods(
        TypeDeclarationSyntax syntax,
        GeneratorSyntaxContext context,
        CancellationToken ct)
    {
        return syntax.Members
            .OfType<MethodDeclarationSyntax>()
            .Select(x => MethodRule.TransformResolver(x, context, ct))
            .FilterNull()
            .ToArray();
    }

    private IBaseFactoryWrapper? TransformBaseType(INamedTypeSymbol symbol, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        return Traverse(symbol);

        IBaseFactoryWrapper? Traverse(INamedTypeSymbol pivot)
        {
            if (pivot.BaseType is not { } baseType) return null;

            return new BaseFactorySymbolWrapper
            {
                Symbol = baseType,
                Methods = TransformIndirectResolvers(baseType),
                BaseType = Traverse(baseType)
            };
        }

        IResolverWrapper[] TransformIndirectResolvers(INamedTypeSymbol x)
        {
            ct.ThrowIfCancellationRequested();
            return x.GetMembers()
                .OfType<IMethodSymbol>()
                .Select(MethodRule.TransformResolver)
                .FilterNull()
                .ToArray();
        }
    }

	private static IResolutionFactoryWrapper[] TransformResolutions(IResolverWrapper[] methods, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return methods
            .Select(x => x.ReturnType.ToResolution())
            .ToArray();
    }

    private static IDelegationFactoryWrapper[] TransformDelegations(
        INamedTypeSymbol owner,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return owner.GetMembers()
            .OfType<IPropertySymbol>()
            .Select(property => property.Type is not INamedTypeSymbol named
                ? null
                : new DelegationSymbolWrapper { Symbol = named })
            .FilterNull()
            .Cast<IDelegationFactoryWrapper>()
            .ToArray();
    }
}