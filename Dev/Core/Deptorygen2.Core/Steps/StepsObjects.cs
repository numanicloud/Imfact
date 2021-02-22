using Deptorygen2.Core.Steps.Aspects;
using Deptorygen2.Core.Steps.Semanticses.Nodes;

namespace Deptorygen2.Core.Steps
{
	public class SyntaxOnAspect
	{
		internal ClassAspect Class { get; }

		internal SyntaxOnAspect(ClassAspect @class)
		{
			Class = @class;
		}
	}

	public class DeptorygenSemantics
	{
		internal SemanticsRoot Semantics { get; }

		internal DeptorygenSemantics(SemanticsRoot semantics)
		{
			Semantics = semantics;
		}
	}
}
