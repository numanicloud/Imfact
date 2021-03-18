using Imfact.Entities;
using Imfact.Steps.Semanticses.Interfaces;

namespace Imfact.Steps.Semanticses.Records
{
	internal record Dependency(TypeAnalysis TypeName, string FieldName) : IVariableSemantics
	{
		public TypeAnalysis Type => TypeName;
		public string MemberName => FieldName;
	}

	internal record Hook(TypeAnalysis HookType, string FieldName) : IVariableSemantics
	{
		public TypeAnalysis Type => HookType;
		public string MemberName => FieldName;
	}

	internal record Parameter(TypeAnalysis Type, string ParameterName) : IVariableSemantics
	{
		public string MemberName => ParameterName;
	}
}
