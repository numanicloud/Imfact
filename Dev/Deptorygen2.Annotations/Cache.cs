using System;

namespace Deptorygen2.Annotations
{
	public class Cache<T> : IHook<T> where T : class
	{
		private T? _cache;
		public T? Before(ResolutionContext context) => _cache;
		public T After(T created, ResolutionContext context) => _cache = created;
	}

	public class CachePerResolution<T> : IHook<T> where T : class
	{
		private T? _cache;
		private int _resolutionId = ResolutionContext.InvalidId;

		public T? Before(ResolutionContext context)
		{
			if (context.Id != _resolutionId)
			{
				return _cache = null;
			}
			return _cache;
		}

		public T After(T created, ResolutionContext context)
		{
			if (_cache is null)
			{
				_resolutionId = context.Id;
			}
			return _cache = created;
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class CacheAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class CachePerResolutionAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class TransientAttribute : Attribute
	{
	}
}