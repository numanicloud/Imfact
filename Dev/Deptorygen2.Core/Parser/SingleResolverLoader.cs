using System;
using System.Linq;
using Deptorygen2.Core.Syntaxes;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Parser
{
	internal class SingleResolverLoader
	{
		private readonly Predicate<ResolverAnalysisContext> _filter;
		private readonly ResolutionLoader _resolutionLoader;

		public SingleResolverLoader(Predicate<ResolverAnalysisContext> filter, ResolutionLoader resolutionLoader)
		{
			_filter = filter;
			_resolutionLoader = resolutionLoader;
		}

		public ResolverSemantics? FromStructure(ResolverAnalysisContext item)
		{
			if (!_filter(item))
			{
				return null;
			}

			var resolutions = item.Resolutions.Select(s => _resolutionLoader.FromStructure(s))
				.FilterNull()
				.ToArray();

			return new ResolverSemantics(
				item.Symbol.Name,
				TypeName.FromSymbol(item.Return),
				_resolutionLoader.FromTypeSymbol(item.Return),
				resolutions,
				ParameterSemantics.FromResolver(item.Symbol),
				item.Symbol.DeclaredAccessibility);
		}
	}
}
