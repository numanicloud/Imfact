using Deptorygen2.Core.Steps.Creation;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Steps.Definitions.Syntaxes
{
	internal class SourceTreeDefinition
	{
		public RootNode Root { get; }
		public ICreationAggregator Creation { get; }

		public SourceTreeDefinition(RootNode root, ICreationAggregator creation)
		{
			Root = root;
			Creation = creation;
		}
	}

	internal record RootNode(UsingNode[] Usings, NamespaceNode Namespace);

	internal record UsingNode(string Namespace);

	internal record NamespaceNode(string Name, ClassNode Class);

	internal record ClassNode(string Name, ConstructorNode Constructor,
		MethodNode[] Methods, PropertyNode[] Properties, FieldNode[] Fields);

	internal record ConstructorNode(string Name, ParameterNode[] Parameters,
		AssignmentNode[] Assignments);

	internal record MethodNode(Accessibility Accessibility, TypeNode ReturnType, string Name,
		ParameterNode[] Parameters);

	internal record PropertyNode(TypeNode Type, string Name);

	internal record FieldNode(TypeNode Type, string Name);

	internal record ParameterNode(TypeNode Type, string Name);

	internal record TypeNode(TypeName TypeName)
	{
		public string Text => TypeName.Name;
	}

	internal record AssignmentNode(string Dest, string Src);
}
