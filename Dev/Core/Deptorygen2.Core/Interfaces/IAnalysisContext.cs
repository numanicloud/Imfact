using System.Collections.Generic;
using Deptorygen2.Core.Entities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Interfaces
{
	internal interface IAnalysisContext
	{
		Dictionary<TypeRecord, ConstructorRecord> Constructors { get; }
		ITypeSymbol? GetTypeSymbol(TypeSyntax syntax);
		IMethodSymbol? GetMethodSymbol(MethodDeclarationSyntax syntax);
		INamedTypeSymbol? GetNamedTypeSymbol(TypeDeclarationSyntax syntax);
		IPropertySymbol? GetPropertySymbol(PropertyDeclarationSyntax syntax);
	}

	internal record ConstructorRecord(TypeNode ClassType, Accessibility Accessibility, ParameterRecord[] Parameters);

	internal record ParameterRecord(TypeNode Type, string Name);
}
