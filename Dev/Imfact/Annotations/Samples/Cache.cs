namespace Imfact.Annotations;

internal sealed class Cache<T> : IHook<T> where T : class
{
	private T? _cache;

	public void RegisterService(IResolverService service)
	{
	}

	public T? Before() => _cache;
	public T After(T created) => _cache = created;
}

internal sealed class CachePerResolution<T> : IHook<T> where T : class
{
	private T? _cache;
	private IResolverService? _service;
	private int? _resolutionId = null;

	public void RegisterService(IResolverService service)
	{
		_service = service;
	}

	public T? Before()
	{
		if (_service?.CurrentResolutionId != _resolutionId)
		{
			return _cache = null;
		}
		return _cache;
	}

	public T After(T created)
	{
		if (_cache is null)
		{
			_resolutionId = _service?.CurrentResolutionId;
		}
		return _cache = created;
	}
}

[global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
internal sealed class CacheAttribute : global::System.Attribute
{
}

[global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
internal sealed class CachePerResolutionAttribute : global::System.Attribute
{
}

[global::System.AttributeUsage(global::System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
internal sealed class TransientAttribute : global::System.Attribute
{
}