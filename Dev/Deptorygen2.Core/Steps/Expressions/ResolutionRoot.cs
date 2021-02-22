using System.Collections.Generic;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Nodes;

namespace Deptorygen2.Core.Steps.Expressions
{
	internal record ResolutionRoot(SemanticsRoot Semantics,
		InjectionResult Injection,
		string[] Usings);

	internal record InjectionResult(
		Dictionary<IResolverSemantics, CreationExpTree> Table,
		Dictionary<IResolverSemantics, MultiCreationExpTree> MultiCreation,
		Dependency[] Dependencies);
}
