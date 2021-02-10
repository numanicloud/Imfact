using System;
using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Semanticses;
using Deptorygen2.Core.Utilities;
using NacHelpers.Extensions;

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

		public ResolverSemantics? Build(ResolverBaseSemantics semantics,
			Func<ResolutionFact, ResolverBaseSemantics, ResolverSemantics> completion)
		{
			if (semantics.ReturnType.IsCollectionType())
			{
				return null;
			}

			var returnSyntax = semantics.Fact.Syntax.ReturnType;
			var returnType = new ResolutionFact(returnSyntax);

			return completion(returnType, semantics);
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
