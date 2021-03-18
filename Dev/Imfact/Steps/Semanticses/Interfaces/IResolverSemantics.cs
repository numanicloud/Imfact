using Imfact.Entities;
using Imfact.Steps.Semanticses.Records;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Semanticses.Interfaces
{
	internal interface IResolverSemantics
	{
		TypeAnalysis ReturnType { get; }
		string MethodName { get; }
		Parameter[] Parameters { get; }
		Accessibility Accessibility { get; }
		Resolution[] Resolutions { get; }
		Hook[] Hooks { get; }
	}
}
