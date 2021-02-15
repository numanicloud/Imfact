using Deptorygen2.Core.Steps.Aspects.Nodes;
using Deptorygen2.Core.Steps.Semanticses.Nodes;

namespace Deptorygen2.Core.Steps
{
	public class SyntaxOnAspect
	{
		internal Class Class { get; }

		internal SyntaxOnAspect(Class @class)
		{
			Class = @class;
		}
	}

	public class DeptorygenSemantics
	{
		internal Generation Semantics { get; }

		internal DeptorygenSemantics(Generation semantics)
		{
			Semantics = semantics;
		}
	}
}
