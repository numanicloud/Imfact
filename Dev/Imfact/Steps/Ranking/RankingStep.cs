using System;
using System.Collections.Generic;
using System.Linq;
using Imfact.Annotations;
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
		public RankedClass[] Run(CandidateClass[] classes)
		{
			// ファクトリーではないものを弾く
			var factoryClasses = classes.Where(x =>
			{
				var hasAttr = x.Syntax.AttributeLists
					.HasAttribute(new AttributeName(nameof(FactoryAttribute)));
				var isPartial = x.Syntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));

				return hasAttr && isPartial;
			}).ToArray();

			// 継承関係を収集する
			var relations = factoryClasses
				.Select(x => x.Context.GetNamedTypeSymbol(x.Syntax) is {} symbol
					? new Relation(x.Syntax, symbol, symbol.BaseType, x.Context)
					: null)
				.FilterNull()
				.ToArray();

			// 0番目のランクとそれ以外のランクに分ける
			var rank0 = relations
				.Where(x => x.BaseSymbol?.SpecialType == SpecialType.System_Object)
				.ToList();
			var notRank0 = relations
				.Where(x => x.BaseSymbol is {} b && b.SpecialType != SpecialType.System_Object)
				.ToList();
			var ranks = new Dictionary<int, List<Relation>>()
			{
				[0] = rank0
			};

			// 各ランクのクラスたちを順番に収集する
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

			// レコード型にまとめて出力する
			return ranks.SelectMany(x => x.Value
					.Select(y => new RankedClass(y.Syntax, y.Symbol, y.BaseSymbol, x.Key, y.Context)))
				.ToArray();
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
