namespace Imfact.Utilities;

internal class NullDisposable : IDisposable
{
	public static readonly NullDisposable Instance = new();

	public void Dispose()
	{
	}
}