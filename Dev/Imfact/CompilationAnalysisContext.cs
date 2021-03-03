using System.Collections.Generic;
using Imfact.Entities;
using Imfact.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imfact
{
	internal class CompilationAnalysisContext : IAnalysisContext
	{
		private readonly SemanticModel _semanticModel;

		public Dictionary<TypeRecord, ConstructorRecord> Constructors { get; } = new();

		public CompilationAnalysisContext(SemanticModel semanticModel)
		{
			_semanticModel = semanticModel;
		}

		public ITypeSymbol? GetTypeSymbol(TypeSyntax syntax)
		{
			return _semanticModel.GetSymbolInfo(syntax).Symbol is ITypeSymbol type ? type
				: null;
		}

		public IMethodSymbol? GetMethodSymbol(MethodDeclarationSyntax syntax)
		{
			return _semanticModel.GetDeclaredSymbol(syntax) is IMethodSymbol method ? method
				: null;
		}

		public IPropertySymbol? GetPropertySymbol(PropertyDeclarationSyntax syntax)
		{
			return _semanticModel.GetDeclaredSymbol(syntax) is IPropertySymbol prop ? prop
				: null;
		}

		public INamedTypeSymbol? GetNamedTypeSymbol(TypeDeclarationSyntax syntax)
		{
			return _semanticModel.GetDeclaredSymbol(syntax) is INamedTypeSymbol type
				? type
				: null;
		}
	}
}