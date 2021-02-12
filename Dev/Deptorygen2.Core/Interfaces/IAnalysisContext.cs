using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Interfaces
{
	public interface IAnalysisContext
	{
		ITypeSymbol? GetTypeSymbol(TypeSyntax syntax);
		IMethodSymbol? GetMethodSymbol(MethodDeclarationSyntax syntax);
		INamedTypeSymbol? GetNamedTypeSymbol(TypeDeclarationSyntax syntax);
		IPropertySymbol? GetPropertySymbol(PropertyDeclarationSyntax syntax);
	}
}
