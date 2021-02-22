using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deptorygen2.Annotations;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Ranking
{
	internal class RankingStep
	{
		public RankedClass[] Run(ClassDeclarationSyntax[] classes,
			IAnalysisContext context)
		{
			// ファクトリーではないものを弾く
			var factoryClasses = classes.Where(x =>
			{
				var hasAttr = x.AttributeLists
					.HasAttribute(new AttributeName(nameof(FactoryAttribute)));
				var isPartial = x.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));

				return hasAttr && isPartial;
			}).ToArray();

			// 継承関係を収集する
			var relations = factoryClasses
				.Select(x => context.GetNamedTypeSymbol(x) is {} symbol
					? new Relation(x, symbol, symbol.BaseType)
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
					.Select(y => new RankedClass(y.Syntax, y.Symbol, y.BaseSymbol, x.Key)))
				.ToArray();
		}

		private record Relation(ClassDeclarationSyntax Syntax, INamedTypeSymbol Symbol,
			INamedTypeSymbol? BaseSymbol)
		{
			public bool IsDerivedBy(Relation inheritor)
			{
				return SymbolEqualityComparer.Default.Equals(Symbol, inheritor.BaseSymbol);
			}
		}
	}
}
