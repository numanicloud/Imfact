using Imfact.Entities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Semanticses.Interfaces
{
	internal interface IResolverSemantics
	{
		TypeNode ReturnType { get; }
		string MethodName { get; }
		Parameter[] Parameters { get; }
		Accessibility Accessibility { get; }
		Resolution[] Resolutions { get; }
		Hook[] Hooks { get; }
	}
}
