namespace Imfact.Annotations;

[global::System.AttributeUsage(global::System.AttributeTargets.Class | global::System.AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
internal sealed class FactoryAttribute : global::System.Attribute
{
}

[global::System.AttributeUsage(global::System.AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
internal sealed class ResolutionAttribute : global::System.Attribute
{
	public global::System.Type Type { get; }

	public ResolutionAttribute(global::System.Type type)
	{
		Type = type;
	}
}

[global::System.AttributeUsage(global::System.AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
internal sealed class ResolutionAttribute<T> : global::System.Attribute
{
}

[global::System.AttributeUsage(global::System.AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
internal sealed class HookAttribute : global::System.Attribute
{
	public global::System.Type Type { get; }

	public HookAttribute(global::System.Type hookType)
	{
		Type = hookType;
	}
}

[global::System.AttributeUsage(global::System.AttributeTargets.Method, Inherited = false)]
internal sealed class ExporterAttribute : global::System.Attribute
{
}