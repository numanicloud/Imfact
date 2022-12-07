using System.Text;

namespace Imfact.Annotations;

internal class AnnotationDefinitions
{
	public const string NamespaceName = "Imfact.Annotations";

	public const string FactoryAttributeName = "FactoryAttribute";
	public const string ResolutionAttributeName = "ResolutionAttribute";
	public const string HookAttributeName = "HookAttribute";
	public const string ExporterAttributeName = "ExporterAttribute";
	public const string IHookName = "IHook";
	public const string IResolverServiceName = "IResolverService";
	public const string ResolverServiceName = "ResolverService";
	public const string CacheName = "Cache";
	public const string CacheAttributeName = "CacheAttribute";
	public const string CachePerResolutionAttributeName = "CachePerResolutionAttribute";
	public const string TransientAttributeName = "TransientAttributeName";

	public static readonly string FactoryAttribute = $@"
[global::System.AttributeUsage(global::System.AttributeTargets.Class | global::System.AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
internal sealed class {FactoryAttributeName} : global::System.Attribute
{{
}}";

	public static readonly string ResolutionAttribute = $@"
[global::System.AttributeUsage(global::System.AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
internal sealed class {ResolutionAttributeName} : global::System.Attribute
{{
	public global::System.Type Type {{ get; }}

	public {ResolutionAttributeName}(global::System.Type type)
	{{
		Type = type;
	}}
}}";

	public static readonly string HookAttribute = $@"
[global::System.AttributeUsage(global::System.AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
internal sealed class {HookAttributeName} : global::System.Attribute
{{
	public global::System.Type Type {{ get; }}

	public {HookAttributeName}(global::System.Type hookType)
	{{
		Type = hookType;
	}}
}}";

	public static readonly string ExporterAttribute = $@"
[global::System.AttributeUsage(global::System.AttributeTargets.Method, Inherited = false)]
internal sealed class {ExporterAttributeName} : global::System.Attribute
{{
}}";

	public static readonly string IHook = $@"
internal interface {IHookName}<T> where T : class
{{
	void RegisterService(IResolverService service);
	T? Before();
	T After(T created);
}}";

	public static readonly string IResolverService = $@"
internal interface {IResolverServiceName}
{{
	int CurrentResolutionId {{ get; }}
	int ResolutionDepth {{ get; }}
}}";

	public static readonly string ResolverService = $@"
internal sealed class {ResolverServiceName} : {IResolverServiceName}
{{
	public int CurrentResolutionId {{ get; set; }} = 0;
	public int ResolutionDepth {{ get; set; }} = 0;

	public Scope Enter()
	{{
		if (ResolutionDepth == 0)
		{{
			CurrentResolutionId++;
		}}
		ResolutionDepth++;
		return new Scope(this);
	}}

	public sealed class Scope : global::System.IDisposable
	{{
		private readonly {ResolverServiceName} _owner;

		public Scope({ResolverServiceName} owner)
		{{
			_owner = owner;
		}}

		public void Dispose()
		{{
			_owner.ResolutionDepth--;
		}}
	}}
}}";

	public static readonly string Cache = $@"
internal sealed class {CacheName}<T> : {IHookName}<T> where T : class
{{
	private T? _cache;

	public void RegisterService({IResolverServiceName} service)
	{{
	}}

	public T? Before() => _cache;
	public T After(T created) => _cache = created;
}}";

	public static readonly string CachePerResolution = @"
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
}";

	public static readonly string CacheAttribute = $@"
[global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
internal sealed class {CacheAttributeName} : global::System.Attribute
{{
}}";

	public static readonly string CachePerResolutionAttribute = $@"
[global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
internal sealed class {CachePerResolutionAttributeName} : global::System.Attribute
{{
}}";

	public static readonly string TransientAttribute = $@"
[global::System.AttributeUsage(global::System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
internal sealed class {TransientAttributeName} : global::System.Attribute
{{
}}";

	public static readonly string IServiceImporter = """
		internal interface IServiceImporter
		{
			void Import<TInterface>(Func<TInterface> resolver) where TInterface : class;
		}
		""";

	public static string BuildAnnotationCode()
	{
		var builder = new StringBuilder();
		builder.AppendLine($"namespace {NamespaceName};");

		builder.AppendLine(FactoryAttribute);
		builder.AppendLine(ResolutionAttribute);
		builder.AppendLine(HookAttribute);
		builder.AppendLine(ExporterAttribute);
		builder.AppendLine(IHook);
		builder.AppendLine(IResolverService);
		builder.AppendLine(ResolverService);
		builder.AppendLine(Cache);
		builder.AppendLine(CacheAttribute);
		builder.AppendLine(CachePerResolution);
		builder.AppendLine(CachePerResolutionAttribute);
		builder.AppendLine(TransientAttribute);
		builder.AppendLine(IServiceImporter);

		return builder.ToString();
	}
}