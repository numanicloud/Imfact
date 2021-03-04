using System.Collections.Generic;
using Imfact.Steps.Dependency.Components;
using Imfact.Steps.Semanticses;
using Imfact.Steps.Semanticses.Interfaces;

namespace Imfact.Steps.Dependency
{
	internal record DependencyRoot(SemanticsRoot Semantics,
		InjectionResult Injection,
		string[] Usings,
		DisposableInfo DisposableInfo)
	{
		public Factory Factory => Semantics.Factory;
		public Delegation[] Delegations => Semantics.Factory.Delegations;
		public Inheritance[] Inheritances => Semantics.Factory.Inheritances;
		public Resolver[] Resolvers => Semantics.Factory.Resolvers;
		public MultiResolver[] MultiResolvers => Semantics.Factory.MultiResolvers;
		public Dictionary<IResolverSemantics, CreationExpTree> Creation => Injection.Creation;
		public Dictionary<IResolverSemantics, MultiCreationExpTree> MultiCreation =>
			Injection.MultiCreation;
		public Semanticses.Dependency[] Dependencies => Injection.Dependencies;
	}

	internal record InjectionResult(
		Dictionary<IResolverSemantics, CreationExpTree> Creation,
		Dictionary<IResolverSemantics, MultiCreationExpTree> MultiCreation,
		Semanticses.Dependency[] Dependencies);
}
