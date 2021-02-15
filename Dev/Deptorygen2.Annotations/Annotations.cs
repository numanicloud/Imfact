using System;
using System.Collections.Generic;

namespace Deptorygen2.Annotations
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class FactoryAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	public sealed class ResolutionAttribute : Attribute
	{
		public Type Type { get; }

		public ResolutionAttribute(Type type)
		{
			Type = type;
		}
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	public sealed class HookAttribute : Attribute
	{
		public Type Type { get; }

		public HookAttribute(Type hookType)
		{
			Type = hookType;
		}
	}

	public interface IHook<T> where T : class
	{
		T? Before(ResolutionContext context);
		T After(T created, ResolutionContext context);
	}

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

	public class ResolutionContext
	{
		private static int _nextId = 0;

		public static int InvalidId = -1;

		public int Id { get; }

		public ResolutionContext()
		{
			Id = _nextId;
			_nextId++;
		}
	}
}
