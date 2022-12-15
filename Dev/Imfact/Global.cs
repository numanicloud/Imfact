// ReSharper disable once CheckNamespace
global using System;
global using System.Linq;
global using System.Collections.Generic;
global using System.Threading;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Imfact.Test")]

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