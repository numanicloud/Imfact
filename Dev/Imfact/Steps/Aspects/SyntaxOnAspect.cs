using Imfact.Steps.Aspects;

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
}
