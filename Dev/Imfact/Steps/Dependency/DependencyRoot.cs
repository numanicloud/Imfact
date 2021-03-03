using System.Collections.Generic;
using Imfact.Steps.Dependency.Components;
using Imfact.Steps.Semanticses;
using Imfact.Steps.Semanticses.Interfaces;

namespace Imfact.Steps.Dependency
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
