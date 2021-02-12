using Deptorygen2.Core.Steps.Aggregation;
using Deptorygen2.Core.Steps.Semanticses;

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
		internal GenerationSemantics Semantics { get; }

		internal DeptorygenSemantics(GenerationSemantics semantics)
		{
			Semantics = semantics;
		}
	}
}
