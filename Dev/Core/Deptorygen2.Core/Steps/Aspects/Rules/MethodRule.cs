using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Steps.Aspects
{
	class MethodRule
	{
		private readonly IAnalysisContext _context;
		private readonly AttributeRule _attributeRule;
		private readonly TypeRule _typeRule;

		public MethodRule(IAnalysisContext context,
			AttributeRule attributeRule,
			TypeRule typeRule)
		{
			_context = context;
			_attributeRule = attributeRule;
			_typeRule = typeRule;
		}

		public MethodAspect? ExtractAspect(MethodDeclarationSyntax syntax)
		{
			if (_context.GetMethodSymbol(syntax) is not { } symbol)
			{
				return null;
			}

			if (symbol.ReturnType is not INamedTypeSymbol returnSymbol)
			{
				return null;
			}

			var returnType = ExtractReturnTypeAspect(returnSymbol);
			var attributes = symbol.GetAttributes()
				.Select(x => _attributeRule.ExtractAspect(x, returnSymbol, symbol.Name))
				.FilterNull()
				.ToArray();
			var parameters = syntax.ParameterList.Parameters
				.Select(ExtractAspect)
				.FilterNull()
				.ToArray();

			return new MethodAspect(symbol.Name, symbol.DeclaredAccessibility,
				GetKind(), returnType, attributes, parameters);

			ResolverKind GetKind()
			{
				var idSymbol = returnSymbol.ConstructedFrom;
				var typeNameValid = idSymbol.MetadataName == typeof(IEnumerable<>).Name;
				var typeArgValid = idSymbol.TypeArguments.Length == 1;
				return typeNameValid && typeArgValid ? ResolverKind.Multi : ResolverKind.Single;
			}
		}

		private ReturnTypeAspect ExtractReturnTypeAspect(INamedTypeSymbol symbol)
		{
			return new(_typeRule.ExtractTypeToCreate(symbol), symbol.IsAbstract);
		}

		public ParameterAspect? ExtractAspect(ParameterSyntax syntax)
		{
			if (syntax.Type is null || _context.GetTypeSymbol(syntax.Type) is not { } symbol)
			{
				return null;
			}

			return new ParameterAspect(TypeNode.FromSymbol(symbol), symbol.Name);
		}
	}
}
