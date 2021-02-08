using System;
using System.Linq;
using Deptorygen2.Core.Structure;

namespace Deptorygen2.Core.Syntaxes.Parser
{
	internal class CollectionResolverLoader
	{
		private readonly Predicate<ResolverAnalysisContext> _filter;

		private readonly ResolutionLoader _resolutionLoader;
		/* 仕様都合の条件：
		 *		IEnumerable<T> 型である
		 *		Resolution属性で指定された型のうち、T に代入できるものが1つ以上ある
		 */

		public CollectionResolverLoader(Predicate<ResolverAnalysisContext> filter, ResolutionLoader resolutionLoader)
		{
			_filter = filter;
			_resolutionLoader = resolutionLoader;
		}

		public CollectionResolverSyntax? FromResolver(ResolverAnalysisContext resolver)
		{
			if (!_filter(resolver))
			{
				return null;
			}

			var parameters = ParameterSyntax.FromResolver(resolver.Symbol);
			var resolutions = resolver.Resolutions.Select(_resolutionLoader.FromStructure)
				.FilterNull()
				.ToArray();

			return new CollectionResolverSyntax(
				resolver.Symbol.Name,
				TypeName.FromSymbol(resolver.Return),
				parameters,
				resolutions);
		}
	}
}
