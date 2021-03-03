using System;

namespace Imfact.Annotations
{
	internal class Cache<T> : IHook<T> where T : class
	{
		private T? _cache;

		public void RegisterService(IResolverService service)
		{
		}

		public T? Before() => _cache;
		public T After(T created) => _cache = created;
	}

	internal class CachePerResolution<T> : IHook<T> where T : class
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

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	internal class CacheAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	internal class CachePerResolutionAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	internal sealed class TransientAttribute : Attribute
	{
	}
}