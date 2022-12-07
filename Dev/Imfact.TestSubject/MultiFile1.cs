using Imfact.Annotations;

namespace Imfact.TestSubject.MultiFile;

internal class Service
{
}

[Factory]
internal partial class MultiFile1Factory
{

	public partial Service ResolveService();
}