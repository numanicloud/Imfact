using Imfact.Entities;
using Imfact.Steps.Definitions.Methods;
using Imfact.Steps.Dependency.Components;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Definitions
{
	internal record DefinitionRoot(Using[] Usings, Namespace Namespace);

	internal record Using(string Namespace);

	internal record Namespace(string Name, Class Class);

	internal record Class(string Name, MethodInfo[] Methods,
		Field[] Fields, DisposableInfo DisposableInfo);

	internal record Property(string Name);

	internal record Field
		(TypeAnalysis TypeAnalysis, string Name, DisposableType Disposable,
		bool IsReadonly = true, Accessibility Accessibility = Accessibility.Private);

	internal record Parameter(TypeAnalysis TypeAnalysis, string Name, bool IsNullable);

	internal record Type(TypeAnalysis TypeName);

	internal record Hook(TypeAnalysis TypeAnalysis, string FieldName);
}
