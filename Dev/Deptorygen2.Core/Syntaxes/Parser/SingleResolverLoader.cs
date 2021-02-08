using System;
using System.Linq;
using Deptorygen2.Core.Structure;

namespace Deptorygen2.Core.Syntaxes.Parser
{
	internal class SingleResolverLoader
	{
		private readonly Predicate<ResolverStructure> _filter;
		private readonly ResolutionLoader _resolutionLoader;

		public SingleResolverLoader(Predicate<ResolverStructure> filter, ResolutionLoader resolutionLoader)
		{
			_filter = filter;
			_resolutionLoader = resolutionLoader;
		}

		public ResolverSyntax? FromStructure(ResolverStructure item)
		{
			if (!_filter(item))
			{
				return null;
			}

			var resolutions = item.Resolutions.Select(s => _resolutionLoader.FromStructure(s))
				.FilterNull()
				.ToArray();

			return new ResolverSyntax(
				item.Symbol.Name,
				TypeName.FromSymbol(item.Return),
				ResolutionSyntax.FromType(item.Return),
				resolutions,
				ParameterSyntax.FromResolver(item.Symbol),
				item.Symbol.DeclaredAccessibility);
		}
	}
}
