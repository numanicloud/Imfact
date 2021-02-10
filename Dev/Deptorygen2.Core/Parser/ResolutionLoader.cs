using System;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Parser
{
	internal record ResolutionFact(TypeSyntax ResolutionType);

	internal class ResolutionLoader
	{
		private readonly Predicate<AttributeData> _filter;
		private readonly IAnalysisContext _context;

		public ResolutionLoader(Predicate<AttributeData> filter, IAnalysisContext context)
		{
			/* 仕様都合の条件：
			 *		ResolutionAttribute型である
			 *		コンストラクタ引数が1つである
			 * 実装都合の条件：
			 *		コンストラクタ引数が1つ以上である
			 *		1番目のコンストラクタ引数の型がTypeである
			 */
			_filter = filter;
			_context = context;
		}

		public ResolutionSemantics? Build(AttributeSyntax attribute)
		{
			if (GetFact(attribute) is not { } fact)
			{
				return null;
			}

			return Build(fact);
		}

		public ResolutionSemantics? Build(ResolutionFact fact)
		{
			if (_context.GeTypeSymbol(fact.ResolutionType) is not INamedTypeSymbol symbol)
			{
				return null;
			}

			var isDisposable = symbol.IsImplementing(typeof(IDisposable));

			var dependencies = symbol.Constructors.Single().Parameters
				.Select(x => TypeName.FromSymbol(x.Type))
				.ToArray();

			return new ResolutionSemantics(
				TypeName.FromSymbol(symbol),
				dependencies,
				isDisposable);
		}

		public ResolutionFact? GetFact(AttributeSyntax attribute)
		{
			var name = new AttributeName("ResolutionAttribute");
			if (!name.MatchWithAnyName(attribute.Name.ToString()))
			{
				return null;
			}

			if (attribute.ArgumentList?.Arguments.Count != 1)
			{
				return null;
			}

			if (attribute.ArgumentList.Arguments[0].Expression is not TypeOfExpressionSyntax teos)
			{
				return null;
			}

			return new ResolutionFact(teos.Type);
		}

		public ResolutionSemantics? FromTypeSymbol(INamedTypeSymbol symbol)
		{
			if (symbol.Constructors.SingleOrDefault() is not { } constructor)
			{
				return null;
			}

			return FromConstructor(symbol, constructor);
		}

		private ResolutionSemantics FromConstructor(INamedTypeSymbol symbol, IMethodSymbol constructor)
		{
			var isDisposable = symbol.IsImplementing(typeof(IDisposable));

			var dependencies = constructor.Parameters
				.Select(x => TypeName.FromSymbol(x.Type))
				.ToArray();

			return new ResolutionSemantics(TypeName.FromSymbol(symbol), dependencies, isDisposable);
		}

		public ResolutionAnalysisContext[] GetStructures(IMethodSymbol resolver)
		{
			return resolver.GetAttributes()
				.Where(x => _filter(x))
				.Select(x => x.ConstructorArguments.FirstOrDefault().Value)
				.FilterNull()
				.OfType<INamedTypeSymbol>()
				.Select(x => new ResolutionAnalysisContext(x))
				.ToArray();
		}

		public ResolutionSemantics? FromStructure(ResolutionAnalysisContext analysisContext)
		{
			return FromTypeSymbol(analysisContext.Symbol);
		}
	}
}
