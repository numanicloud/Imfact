using Imfact.Steps.Aspects;
using Imfact.Steps.Semanticses;

namespace Imfact.Steps
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
