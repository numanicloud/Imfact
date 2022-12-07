// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;

internal class IsExternalInit { }

internal class RequiredMemberAttribute : Attribute
{
}

internal class CompilerFeatureRequiredAttribute : Attribute
{
	public CompilerFeatureRequiredAttribute(string featureName)
	{
	}
}