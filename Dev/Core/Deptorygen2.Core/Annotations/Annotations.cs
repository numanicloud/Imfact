using System;
using System.Collections.Generic;

namespace Deptorygen2.Annotations
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	internal sealed class FactoryAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	internal sealed class ResolutionAttribute : Attribute
	{
		public Type Type { get; }

		public ResolutionAttribute(Type type)
		{
			Type = type;
		}
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	internal sealed class HookAttribute : Attribute
	{
		public Type Type { get; }

		public HookAttribute(Type hookType)
		{
			Type = hookType;
		}
	}

	internal interface IHook<T> where T : class
	{
		T? Before(ResolutionContext context);
		T After(T created, ResolutionContext context);
	}

	internal class ResolutionContext
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
