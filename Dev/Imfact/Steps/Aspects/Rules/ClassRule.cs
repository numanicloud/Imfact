using Imfact.Entities;
using Imfact.Incremental;
using Imfact.Steps.Cacheability;
using Imfact.Steps.Filter;
using Imfact.Steps.Filter.Wrappers;
using Imfact.Steps.Ranking;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Aspects.Rules;

internal class ClassRule
{
	public required MethodRule MethodRule { private get; init; }
	public required PropertyRule PropertyRule { private get; init; }

	public ClassAspect Aggregate(RankedClass root)
	{
		try
		{
			return ExtractThis(root.Self, ExtractBases(root.Self));
		}
		catch (Exception ex)
		{
			throw new Exception($"Exception occured in extracting class aspect of {root.Self.Symbol.Name}.",
				ex); 
		}
	}

	private ClassAspect ExtractThis(FactoryCandidate factory, ClassAspect[] baseClasses)
	{
		using var profiler = AggregationProfiler.GetScope();

		var symbol = factory.Symbol;
		return new(TypeAnalysis.FromSymbol(symbol),
			baseClasses,
			ExtractInterfaces(symbol),
			GetMethodAspects(factory, partialOnly: true),
			GetPropertyAspects(symbol, factory.Annotations),
			null);
	}

    private ClassAspect[] ExtractBases(FactoryCandidate factory)
	{
		// 基底クラスのコンストラクタを呼ぶためには基底クラスを通す必要がある
		// 基底クラスのリゾルバーを呼ぶためには基底クラスを通す必要がある
		return TraverseBase(factory.Symbol)
			.Select(x => new FactoryCandidate(x, GetMethods(x), factory.Annotations))
			.Select(ExtractBase)
			.ToArray();

		IEnumerable<INamedTypeSymbol> TraverseBase(INamedTypeSymbol pivot)
		{
			using var profiler = AggregationProfiler.GetScope();

			if (pivot.BaseType is not null 
				&& GeneralRule.Instance.IsInheritanceTarget(pivot.BaseType))
			{
				yield return pivot.BaseType;
				foreach (var baseSymbol in TraverseBase(pivot.BaseType))
				{
					yield return baseSymbol;
				}
			}
		}

		ResolverCandidate[] GetMethods(INamedTypeSymbol holder)
		{
			return holder.GetMembers()
				.OfType<IMethodSymbol>()
				.Where(GeneralRule.Instance.IsIndirectResolver)
				.Select(x => new ResolverCandidate(x, false))
				.ToArray();
		}
	}

	// NOTE: 基底クラスのプロパティを追う必要は無いのでは？
	// 逆に、コンストラクタはこちらにしか無いので、BaseClassAspect みたいな型で区別してもいいかも
	private ClassAspect ExtractBase(FactoryCandidate factory)
	{
		using var profiler = AggregationProfiler.GetScope();

		var symbol = factory.Symbol;
		return new(
			TypeAnalysis.FromSymbol(symbol),
			new ClassAspect[0],
			ExtractInterfaces(symbol),
			GetMethodAspects(factory, false),
			GetPropertyAspects(symbol, factory.Annotations),
			GetConstructor(symbol));
	}

	private InterfaceAspect[] ExtractInterfaces(INamedTypeSymbol thisSymbol)
	{
		return thisSymbol.AllInterfaces
			.Select(x => new InterfaceAspect(TypeAnalysis.FromSymbol(x)))
			.ToArray();
	}

	private MethodAspect[] GetMethodAspects(FactoryCandidate factory, bool partialOnly)
	{
		var members = partialOnly
			? factory.Methods.Where(x => x.IsToGenerate)
			: factory.Methods;

		var methodSymbols = members
			.Select(x => x.Symbol);

		return ExtractMethods(methodSymbols);
	}

	private MethodAspect[] ExtractMethods(IEnumerable<IMethodSymbol> methodSymbols)
	{
		throw new NotImplementedException();
	}

	private PropertyAspect[] GetPropertyAspects(INamedTypeSymbol @class, AnnotationContext annotations)
	{
		return @class.GetMembers()
			.OfType<IPropertySymbol>()
			.Select(x => PropertyRule.ExtractAspect(x, annotations))
			.FilterNull()
			.ToArray();
	}

	private ConstructorAspect GetConstructor(INamedTypeSymbol symbol)
	{
		var cc = symbol.Constructors.MaxItem(x => x.Arity);

		var parameters = cc.Parameters
			.Select(x => new ParameterAspect(TypeAnalysis.FromSymbol(x.Type), x.Name))
			.ToArray();

		return new ConstructorAspect(cc.DeclaredAccessibility, parameters);
	}
}

internal class ClassRuleAlt
{
	public required MethodRule MethodRule { private get; init; }
	public required PropertyRule PropertyRule { private get; init; }

	public ClassAspect Transform(CacheabilityResult input, CancellationToken ct)
	{
		var baseTypes = Traverse(input.Type.BaseFactory)
			.Select(x => ExtractBaseType(x, ct))
			.ToArray();

		return ExtractThis(input.Type, baseTypes, ct);

		IEnumerable<FilteredBaseType> Traverse(FilteredBaseType? pivot)
		{
			if (pivot is null) yield break;

			yield return pivot;
			foreach (var type in Traverse(pivot.BaseFactory))
			{
				yield return type;
			}
		}
	}

	private ClassAspect ExtractThis(FilteredType self, ClassAspect[] bases, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

	private ClassAspect ExtractBaseType(FilteredBaseType baseType, CancellationToken ct)
	{
        throw new NotImplementedException();
	}

	private InterfaceAspect[] ExtractInterfaces(IInterfaceImplementor thisSymbol)
	{
		return thisSymbol.AllInterfaces
			.Select(x => new InterfaceAspect(x.GetTypeAnalysis()))
			.ToArray();
	}

	private MethodAspect[] ExtractMethods(IEnumerable<FilteredMethod> methodSymbols)
	{
		return methodSymbols
			.Select(MethodRule.ExtractAspect)
			.FilterNull()
			.ToArray();
	}

	private ConstructorAspect GetConstructor(INamedTypeSymbol symbol)
	{
		var cc = symbol.Constructors.MaxItem(x => x.Arity);

		var parameters = cc.Parameters
			.Select(x => new ParameterAspect(TypeAnalysis.FromSymbol(x.Type), x.Name))
			.ToArray();

		return new ConstructorAspect(cc.DeclaredAccessibility, parameters);
	}
}