using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Deptorygen2.Core.Annotations
{
	internal class AnnotationGenerator
	{
		private const string annotationText = @"using System;
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
";

		private const string cacheText = @"using System;
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
}";

		public static void AddSource(in GeneratorExecutionContext context)
		{
			context.AddSource("Annotations", SourceText.From(annotationText, Encoding.UTF8));
			context.AddSource("Caches", SourceText.From(cacheText, Encoding.UTF8));
		}

		public static SyntaxTree[] GetSyntaxTrees(CSharpParseOptions options)
		{
			return new[]
			{
				CSharpSyntaxTree.ParseText(SourceText.From(annotationText, Encoding.UTF8), options),
				CSharpSyntaxTree.ParseText(SourceText.From(cacheText, Encoding.UTF8), options),
			};
		}
	}
}
