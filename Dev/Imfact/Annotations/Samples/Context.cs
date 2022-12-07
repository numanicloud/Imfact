namespace Imfact.Annotations.Samples;

internal interface IHook<T> where T : class
{
	void RegisterService(IResolverService service);
	T? Before();
	T After(T created);
}

internal interface IResolverService
{
	int CurrentResolutionId { get; }
	int ResolutionDepth { get; }
}

internal sealed class ResolverService : IResolverService
{
	public int CurrentResolutionId { get; set; } = 0;
	public int ResolutionDepth { get; set; } = 0;

	public Scope Enter()
	{
		if (ResolutionDepth == 0)
		{
			CurrentResolutionId++;
		}
		ResolutionDepth++;
		return new Scope(this);
	}

	public sealed class Scope : global::System.IDisposable
	{
		private readonly ResolverService _owner;

		public Scope(ResolverService owner)
		{
			_owner = owner;
		}

		public void Dispose()
		{
			_owner.ResolutionDepth--;
		}
	}
}