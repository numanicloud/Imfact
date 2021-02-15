using System;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Steps.Definitions.Syntaxes
{
	internal record RootNode(UsingNode[] Usings, NamespaceNode Namespace);

	internal record UsingNode(string Namespace);

	internal record NamespaceNode(string Name, ClassNode Class);

	internal record ClassNode(string Name, ConstructorNode Constructor,
		MethodNode[] Methods, PropertyNode[] Properties, FieldNode[] Fields,
		EntryMethodNode[] EntryMethods);

	internal record ConstructorNode(string Name, ParameterNode[] Parameters,
		AssignmentNode[] Assignments);

	// ユーザーからは触らない想定。実際の解決を行う実装が入っているメソッド
	internal record MethodNode(Accessibility Accessibility, TypeNode ReturnType, string Name,
		ParameterNode[] Parameters, TypeName ResolutionType, HookNode[] Hooks);

	// MethodNodeのほうのメソッドを呼び出すメソッド。ユーザーが直接使う
	internal record EntryMethodNode(Accessibility Accessibility, TypeNode ReturnType,
		string Name, ParameterNode[] Parameters);

	internal record PropertyNode(TypeNode Type, string Name);

	internal record FieldNode(TypeNode Type, string Name);

	internal record ParameterNode(TypeNode Type, string Name);

	internal record TypeNode(TypeName TypeName)
	{
		public string Text => $"{TypeName.FullNamespace}.{TypeName.Name}" switch
		{
			nameof(System.Byte) => "byte",
			nameof(System.Int16) => "short",
			nameof(System.Int32) => "int",
			nameof(System.Int64) => "long",
			nameof(System.SByte) => "sbyte",
			nameof(System.UInt16) => "ushort",
			nameof(System.UInt32) => "uint",
			nameof(System.UInt64) => "ulong",
			nameof(System.Single) => "float",
			nameof(System.Double) => "double",
			nameof(System.Decimal) => "decimal",
			nameof(System.Char) => "char",
			nameof(System.String) => "string",
			"System.Void" => "void",
			_ => TypeName.Name
		};
	}

	internal record AssignmentNode(string Dest, string Src);

	internal record HookNode(TypeNode FieldType, string FieldName);
}
