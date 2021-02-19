using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Steps.Definitions.Methods;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Definitions
{
	internal record DefinitionRoot(Using[] Usings, Namespace Namespace);

	internal record Using(string Namespace);

	internal record Namespace(string Name, Class Class);

	internal record Class(string Name, MethodInfo[] Methods, Property[] Properties,
		Field[] Fields, DisposableInfo DisposableInfo);

	internal record Property(Type Type, string Name);

	internal record Field(Type Type, string Name, DisposableType Disposable);

	internal record Parameter(Type Type, string Name);

	internal record Type(TypeNode TypeName)
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
			_ => TypeName.FullBoundName
		};
	}

	internal record Assignment(string Dest, string Src);

	internal record Hook(Type FieldType, string FieldName);
}
