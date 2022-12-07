// ReSharper disable once CheckNamespace
global using System;
global using System.Linq;
global using System.Collections.Generic;

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