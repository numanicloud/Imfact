using System;
using System.Linq;
using Deptorygen2.Core.Semanticses;
using Deptorygen2.Core.Utilities;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Parser
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

		public CollectionResolverSemantics? Build(ResolverBaseSemantics semantics)
		{
			if (!semantics.ReturnType.IsCollectionType())
			{
				return null;
			}

			return new CollectionResolverSemantics(
				semantics.MethodName,
				semantics.ReturnType,
				semantics.Parameters,
				semantics.Resolutions,
				semantics.Accessibility);
		}

		public CollectionResolverSemantics? FromResolver(ResolverAnalysisContext resolver)
		{
			if (!_filter(resolver))
			{
				return null;
			}

			var parameters = ParameterSemantics.FromResolver(resolver.Symbol);
			var resolutions = resolver.Resolutions.Select(_resolutionLoader.FromStructure)
				.FilterNull()
				.ToArray();

			return new CollectionResolverSemantics(
				resolver.Symbol.Name,
				TypeName.FromSymbol(resolver.Return),
				parameters,
				resolutions,
				resolver.Symbol.DeclaredAccessibility);
		}
	}
}
