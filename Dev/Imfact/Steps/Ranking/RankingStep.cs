using System;
using System.Collections.Generic;
using System.Linq;
using Imfact.Entities;
using Imfact.Interfaces;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imfact.Steps.Ranking
{
    internal class RankingStep
	{
		private static readonly AttributeName FactoryAttribute = new(nameof(Annotations.Samples.FactoryAttribute));
		private static readonly AttributeName ResolutionAttribute = new(nameof(Annotations.Samples.ResolutionAttribute));

		public RankedClass[] Run(CandidateClass[] classes)
		{
			using var profiler = TimeProfiler.Create("Ranking");
			var factoryClasses = ExtractFactories(classes);
			var relations = AnalyzeRelations(factoryClasses);
			var (rank0, notRank0) = ExtractRank0(relations);
			var ranks = DetermineRanking(rank0, notRank0);
			
			return ranks.SelectMany(x => x.Value
					.Select(y => new RankedClass(y.Symbol, y.BaseSymbol, x.Key, y.Context)))
				.ToArray();
		}

		private static Dictionary<int, List<Relation>> DetermineRanking(List<Relation> rank0, List<Relation> notRank0)
		{
			using var profiler = TimeProfiler.Create("DetermineRanking");
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
			using var profiler = TimeProfiler.Create("Extract-Rank0-Ranking");

			var marked = relations
				.Select(x =>
				{
					var hasBaseType = x.BaseSymbol is { } b &&
						b.SpecialType != SpecialType.System_Object;
					var hasResolverForFactory = HasResolverForFactory(x);
					return (hasBaseType || hasResolverForFactory, x);
				}).ToArray();

			return (marked.Where(x => !x.Item1).Select(x => x.x).ToList(),
				marked.Where(x => x.Item1).Select(x => x.x).ToList());

			bool HasResolverForFactory(Relation target)
			{
				return target.Symbol.GetMembers()
					.OfType<IMethodSymbol>()
					.Where(m => m.DeclaringSyntaxReferences
						.Select(x => x.GetSyntax())
						.OfType<MethodDeclarationSyntax>()
						.Any())
					.Any(x => IsFactoryResolver(x) is not null);
			}
		}

		private static Relation[] AnalyzeRelations(CandidateClass[] factoryClasses)
		{
			return factoryClasses
				.Select(
					x =>
					{
						using var profiler = TimeProfiler.Create("Extract-Relations-Ranking");
						return x.Context.GetNamedTypeSymbol(x.Syntax) is { } symbol
							? new Relation(x.Syntax, symbol, symbol.BaseType, x.Context)
							: null;
					})
				.FilterNull()
				.ToArray();
		}

		private static CandidateClass[] ExtractFactories(CandidateClass[] classes)
		{
			return classes.Where(x =>
			{
				using var profiler = TimeProfiler.Create("Extract-Factory-Ranking");
				var hasAttr = x.Syntax.AttributeLists
					.HasAttribute(FactoryAttribute);
				var isPartial = x.Syntax.Modifiers.IndexOf(SyntaxKind.PartialKeyword) != -1;

				return hasAttr && isPartial;
			}).ToArray();
		}

		private static ITypeSymbol? IsFactoryResolver(IMethodSymbol method)
		{
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

		private record Relation(ClassDeclarationSyntax Syntax, INamedTypeSymbol Symbol,
			INamedTypeSymbol? BaseSymbol, IAnalysisContext Context)
		{
			public bool IsDerivedBy(Relation inheritor)
			{
				return SymbolEqualityComparer.Default.Equals(Symbol, inheritor.BaseSymbol);
			}

			public bool HasResolverOf(Relation resolution)
			{
				// TODO: IsFactoryResolverを何度も走らせると重そう、キャッシュしたい
				return Symbol.GetMembers()
					.OfType<IMethodSymbol>()
					.Select(IsFactoryResolver)
					.FilterNull()
					.Any(t => SymbolEqualityComparer.Default.Equals(t, resolution.Symbol));
			}
		}
	}
}
