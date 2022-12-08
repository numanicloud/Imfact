using System;
using System.Collections.Generic;
using System.Linq;
using Imfact.Entities;
using Imfact.Incremental;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imfact.Steps.Ranking;

internal class RankingStep
{
	private static readonly AttributeName FactoryAttribute = new(nameof(FactoryAttribute));
	private static readonly AttributeName ResolutionAttribute = new(nameof(ResolutionAttribute));

	public RankedClass[] Run(FactoryCandidate[] classes)
	{
		using var profiler = AggregationProfiler.GetScope();

		var relations = AnalyzeRelations(classes);
		var (rank0, notRank0) = ExtractRank0(relations);
		var ranks = DetermineRanking(rank0, notRank0);
			
		return ranks.SelectMany(x => x.Value
				.Select(y => new RankedClass(y.Self, y.Base, x.Key)))
			.ToArray();
	}

	private Dictionary<int, List<Relation>> DetermineRanking(List<Relation> rank0, List<Relation> notRank0)
	{
		using var profiler = AggregationProfiler.GetScope();
		var ranks = new Dictionary<int, List<Relation>>()
		{
			[0] = rank0
		};

		int reference = 0;
		while (notRank0.Any())
		{
			var nextRank = notRank0
				.Where(x => ranks[reference].Any(y => y.IsDerivedBy(x) || x.HasResolverOf(y)))
				.ToList();

			ranks[reference + 1] = nextRank;
			foreach (var item in nextRank)
			{
				notRank0.Remove(item);
			}

			reference += 1;

			if (reference > 10)
			{
				throw new Exception();
			}
		}

		return ranks;
	}

	private static (List<Relation> rank0, List<Relation> notRank0) ExtractRank0(Relation[] relations)
	{
		using var profiler = AggregationProfiler.GetScope();

		var marked = relations
			.Select(x =>
			{
				var hasBaseType = x.Base is { } b &&
					b.Symbol.SpecialType != SpecialType.System_Object;
				var hasResolverForFactory = HasResolverForFactory(x);
				return (hasBaseType || hasResolverForFactory, x);
			}).ToArray();

		var rank0 = marked.Where(x => !x.Item1).Select(x => x.x).ToList();
		var notRank0 = marked.Where(x => x.Item1).Select(x => x.x).ToList();

		if (rank0.Count == 0)
		{
			throw new Exception();
		}

        return (rank0, notRank0);

		bool HasResolverForFactory(Relation target)
		{
			return target.Self.Symbol.GetMembers()
				.OfType<IMethodSymbol>()
				.Where(m => m.DeclaringSyntaxReferences
					.Select(x => x.GetSyntax())
					.OfType<MethodDeclarationSyntax>()
					.Any())
				.Any(x => IsFactoryResolver(x) is not null);
		}
	}

	private static Relation[] AnalyzeRelations(FactoryCandidate[] factoryClasses)
	{
		return factoryClasses
			.Select(
				x =>
				{
					using var profiler = AggregationProfiler.GetScope();

					var baseType = factoryClasses
						.FirstOrDefault(y => SymbolEquals(y.Symbol, x.Symbol.BaseType));
					return new Relation(x, baseType);
				})
			.FilterNull()
			.ToArray();
	}

	private static ITypeSymbol? IsFactoryResolver(IMethodSymbol method)
	{
		// ファクトリークラスA をインスタンス化するメソッドを生成しなければならない場合、
		// このクラスよりも先に ファクトリークラスA を生成しなければならない

		var returnFactory = method.ReturnType.GetAttributes()
			.Any(a => a.AttributeClass is { } ac &&
				FactoryAttribute.MatchWithAnyName(ac.Name));
		if (returnFactory && method.ReturnType.TypeKind != TypeKind.Interface)
		{
			return method.ReturnType;
		}

		var resolutionAttr = method.GetAttributes()
			.FirstOrDefault(a => a.AttributeClass is { } ac &&
				ResolutionAttribute.MatchWithAnyName(ac.Name));
		if (resolutionAttr is null)
		{
			return null;
		}

		if (resolutionAttr.ConstructorArguments.Length == 1
			&& resolutionAttr.ConstructorArguments[0].Kind == TypedConstantKind.Type
			&& resolutionAttr.ConstructorArguments[0].Value is INamedTypeSymbol type)
		{
			var isFactory = type.GetAttributes()
				.Any(a => a.AttributeClass is { } ac &&
					FactoryAttribute.MatchWithAnyName(ac.Name));
			if (isFactory)
			{
				return type;
			}
		}

		return null;
	}

	private static bool SymbolEquals(ITypeSymbol left, INamedTypeSymbol? right)
	{
		return SymbolEqualityComparer.Default.Equals(left, right);
	}

	private record Relation(FactoryCandidate Self,
		FactoryCandidate? Base)
	{
		/// <summary>
		/// inheritor がこのクラスを継承するかどうかを返します。
		/// </summary>
		/// <param name="inheritor"></param>
		/// <returns></returns>
		public bool IsDerivedBy(Relation inheritor)
		{
			return SymbolEquals(Self.Symbol, inheritor.Base?.Symbol);
		}

		/// <summary>
		/// このクラスが resolution を解決するかどうかを返します。
		/// </summary>
		/// <param name="resolution"></param>
		/// <returns></returns>
		public bool HasResolverOf(Relation resolution)
		{
			// TODO: IsFactoryResolverを何度も走らせると重そう、キャッシュしたい
			return Self.Methods
				.Select(x => x.Symbol)
				.Select(IsFactoryResolver)
				.FilterNull()
				.Any(t => SymbolEquals(t, resolution.Self.Symbol));
		}
	}
}