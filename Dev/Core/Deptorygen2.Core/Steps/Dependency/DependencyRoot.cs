using System.Collections.Generic;
using Deptorygen2.Core.Steps.Dependency.Components;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;

namespace Deptorygen2.Core.Steps.Expressions
{
	internal record DependencyRoot(SemanticsRoot Semantics,
		InjectionResult Injection,
		string[] Usings,
		DisposableInfo DisposableInfo);

	internal record InjectionResult(
		Dictionary<IResolverSemantics, CreationExpTree> Table,
		Dictionary<IResolverSemantics, MultiCreationExpTree> MultiCreation,
		Semanticses.Dependency[] Dependencies);
}
