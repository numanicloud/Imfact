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
		private static readonly AttributeName FactoryAttribute = new(nameof(Annotations.FactoryAttribute));

		public RankedClass[] Run(CandidateClass[] classes)
		{
			using var profiler = TimeProfiler.Create("Ranking");
			var factoryClasses = ExtractFactories(classes);
			var relations = AnalyzeRelations(factoryClasses);
			var (rank0, notRank0) = ExtractRank0(relations);
			var ranks = DetermineRanking(rank0, notRank0);
			
			return ranks.SelectMany(x => x.Value
					.Select(y => new RankedClass(y.Syntax, y.Symbol, y.BaseSymbol, x.Key, y.Context)))
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
					.Where(x => ranks[reference].Any(y => y.IsDerivedBy(x)))
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
			var rank0 = relations
				.Where(x => x.BaseSymbol?.SpecialType == SpecialType.System_Object)
				.ToList();
			var notRank0 = relations
				.Where(x => x.BaseSymbol is { } b && b.SpecialType != SpecialType.System_Object)
				.ToList();
			return (rank0, notRank0);
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

		private record Relation(ClassDeclarationSyntax Syntax, INamedTypeSymbol Symbol,
			INamedTypeSymbol? BaseSymbol, IAnalysisContext Context)
		{
			public bool IsDerivedBy(Relation inheritor)
			{
				return SymbolEqualityComparer.Default.Equals(Symbol, inheritor.BaseSymbol);
			}
		}
	}
}
