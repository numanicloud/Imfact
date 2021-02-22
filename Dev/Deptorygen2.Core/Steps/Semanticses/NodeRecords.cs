using System.Linq;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Generation(
		string[] RequiredNamespaces, Factory Factory,
		Dependency[] Dependencies, DisposableInfo DisposableInfo);

	internal record Factory(FactoryCommon Common,
		Delegation[] Delegations, Inheritance[] Inheritances, EntryResolver[] EntryResolvers)
		: FactoryCommon(Common);

	internal record Delegation(FactoryCommon Common, string PropertyName)
		: FactoryCommon(Common), IProbablyDisposable
	{
		public string MemberName => PropertyName;
	}

	internal record Inheritance(FactoryCommon Common, Parameter[] Parameters)
		: FactoryCommon(Common);

	internal record FactoryCommon(TypeNode Type,
		Resolver[] Resolvers,
		MultiResolver[] MultiResolvers) : IFactorySemantics;

	internal record Resolver(ResolverCommon Common, Resolution? ReturnTypeResolution)
		: ResolverCommon(Common)
	{
		public Resolution ActualResolution = ReturnTypeResolution.AsEnumerable()
			.Concat(Common.Resolutions).First();
	}

	internal record MultiResolver(ResolverCommon Common)
		: ResolverCommon(Common)
	{
		public TypeNode ElementType => ReturnType.TypeArguments[0];
	}

	internal record ResolverCommon(
		Accessibility Accessibility,
		TypeNode ReturnType,
		string MethodName,
		Parameter[] Parameters,
		Resolution[] Resolutions,
		Hook[] Hooks) : IResolverSemantics;

	internal record Dependency(TypeNode TypeName, string FieldName) : IProbablyDisposable
	{
		public TypeNode Type => TypeName;
		public string MemberName => FieldName;
		public string ParameterName => FieldName.TrimStart("_".ToCharArray());
	}

	internal record Hook(TypeNode HookType, string FieldName) : IProbablyDisposable
	{
		public TypeNode Type => HookType;
		public string MemberName => FieldName;
	}

	internal record Parameter(TypeNode Type, string ParameterName);

	internal record Resolution(TypeNode TypeName,
		TypeNode[] Dependencies,
		DisposableType DisposableType);

	internal record EntryResolver(string MethodName,
		TypeNode ReturnType,
		Parameter[] Parameters,
		Accessibility Accessibility);

	internal interface IProbablyDisposable
	{
		TypeNode Type { get; }
		string MemberName { get; }
	}
}
