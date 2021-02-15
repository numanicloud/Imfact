using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Steps.Definitions
{
	internal record DefinitionRoot(Using[] Usings, Namespace Namespace);

	internal record Using(string Namespace);

	internal record Namespace(string Name, Class Class);

	internal record Class(string Name, Constructor Constructor,
		Method[] Methods, Property[] Properties, Field[] Fields,
		EntryMethod[] EntryMethods);

	internal record Constructor(string Name, Parameter[] Parameters,
		Assignment[] Assignments);

	// ユーザーからは触らない想定。実際の解決を行う実装が入っているメソッド
	internal record Method(Accessibility Accessibility, Type ReturnType, string Name,
		Parameter[] Parameters, TypeName ResolutionType, Hook[] Hooks);

	// MethodNodeのほうのメソッドを呼び出すメソッド。ユーザーが直接使う
	internal record EntryMethod(Accessibility Accessibility, Type ReturnType,
		string Name, Parameter[] Parameters);

	internal record Property(Type Type, string Name);

	internal record Field(Type Type, string Name);

	internal record Parameter(Type Type, string Name);

	internal record Type(TypeName TypeName)
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

	internal record Assignment(string Dest, string Src);

	internal record Hook(Type FieldType, string FieldName);
}
