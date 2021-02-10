using Deptorygen2.Core.Aggregation;
using Deptorygen2.Core.Definitions;
using Deptorygen2.Core.Semanticses;

namespace Deptorygen2.Core.Steps.Api
{
	public class SyntaxOnAspect
	{
		internal ClassToAnalyze Class { get; }

		internal SyntaxOnAspect(ClassToAnalyze @class)
		{
			Class = @class;
		}
	}

	public class DeptorygenSemantics
	{
		internal FactorySemantics Semantics { get; }

		internal DeptorygenSemantics(FactorySemantics semantics)
		{
			Semantics = semantics;
		}
	}

	public class DeptorygenDefinition
	{
		internal SourceCodeDefinition Definition { get; }

		internal DeptorygenDefinition(SourceCodeDefinition definition)
		{
			Definition = definition;
		}
	}
}
