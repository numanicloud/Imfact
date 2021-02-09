using Deptorygen2.Core.Semanticses;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Interfaces
{
	internal interface IResolverSemantics
	{
		string MethodName { get; }
		ParameterSemantics[] Parameters { get; }
		Accessibility Accessibility { get; }
	}
}
