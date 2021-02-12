using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Deptorygen2.Generator
{
	interface IHook<T> where T : class
	{
		T? Before();
		T? After(T created);
	}

	class Cache<T> : IHook<T> where T : class
	{
		private T? _cache;
		public T? Before() => _cache;
		public T? After(T created) => _cache = created;
	}

	class AnnotationGenerator
	{
		private static readonly string Code = @"
using System;

namespace Deptorygen
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	sealed class FactoryAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	sealed class ResolutionAttribute : Attribute
	{
		public Type Type { get; }

		public ResolutionAttribute(Type type)
		{
			Type = type;
		}
	}

	interface IHook<T> where T : class
	{
		T? Before();
		T? After(T created);
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	sealed class HookAttribute : Attribute
	{
		public Type Type { get; }

		public HookAttribute(Type hookType)
		{
			Type = hookType;
		}
	}

	class Cache<T> : IHook<T> where T : class
	{
		private T? _cache;
		public T? Before() => _cache;
		public T? After(T created) => _cache = created;
	}
}
";

		public static void AddSource(in GeneratorExecutionContext context)
		{
			var code1 = SourceText.From(Code, Encoding.UTF8);
			context.AddSource("DeptorygenAnnotations.g", code1);
		}
	}
}
