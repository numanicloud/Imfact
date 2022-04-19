using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Imfact.Annotations
{
	internal class AnnotationGenerator
	{
		private const string annotationText = @"using System;
namespace Imfact.Annotations
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

	internal class ResolverService : IResolverService
	{
		public int CurrentResolutionId { get; private set; } = 0;
		public int ResolutionDepth { get; private set; } = 0;

		public Scope Enter()
		{
			if (ResolutionDepth == 0)
			{
				CurrentResolutionId++;
			}
			ResolutionDepth++;
			return new Scope(this);
		}

		public class Scope : IDisposable
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

	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public sealed class ExporterAttribute : Attribute
	{
	}
}
";

		public static void AddSource(in GeneratorExecutionContext context)
		{
			context.AddSource("Annotations", SourceText.From(annotationText, Encoding.UTF8));
		}

		public static SyntaxTree[] GetSyntaxTrees(CSharpParseOptions options)
		{
			return new[]
			{
				CSharpSyntaxTree.ParseText(SourceText.From(annotationText, Encoding.UTF8), options),
			};
		}
	}
}
