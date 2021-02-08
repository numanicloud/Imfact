using System;
using System.Linq;
using Deptorygen2.Core.Structure;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Syntaxes.Parser
{
	internal class ResolutionLoader
	{
		private readonly Predicate<AttributeData> _filter;

		public ResolutionLoader(Predicate<AttributeData> filter)
		{
			/* 仕様都合の条件：
			 *		ResolutionAttribute型である
			 *		コンストラクタ引数が1つである
			 * 実装都合の条件：
			 *		コンストラクタ引数が1つ以上である
			 *		1番目のコンストラクタ引数の型がTypeである
			 */
			_filter = filter;
		}

		public static ResolutionSyntax? FromTypeSymbol(INamedTypeSymbol symbol)
		{
			if (symbol.Constructors.SingleOrDefault() is not { } constructor)
			{
				return null;
			}

			return FromConstructor(symbol, constructor);
		}

		private static ResolutionSyntax FromConstructor(INamedTypeSymbol symbol, IMethodSymbol constructor)
		{
			var isDisposable = symbol.IsImplementing(typeof(IDisposable));

			var dependencies = constructor.Parameters
				.Select(x => TypeName.FromSymbol(x.Type))
				.ToArray();

			return new ResolutionSyntax(TypeName.FromSymbol(symbol), dependencies, isDisposable);
		}

		public ResolutionStructure[] GetStructures(IMethodSymbol resolver)
		{
			return resolver.GetAttributes()
				.Where(x => _filter(x))
				.Select(x => x.ConstructorArguments.FirstOrDefault().Value)
				.FilterNull()
				.OfType<INamedTypeSymbol>()
				.Select(x => new ResolutionStructure(x))
				.ToArray();
		}

		public ResolutionSyntax? FromStructure(ResolutionStructure structure)
		{
			return FromTypeSymbol(structure.Symbol);
		}
	}
}
