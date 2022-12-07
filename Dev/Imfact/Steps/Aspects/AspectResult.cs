namespace Imfact.Steps.Aspects;

public class AspectResult
{
	internal ClassAspect Class { get; }

	internal AspectResult(ClassAspect @class)
	{
		Class = @class;
	}
}