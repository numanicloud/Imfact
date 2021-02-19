using System.Collections.Generic;
using Deptorygen2.Annotations;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Deptorygen2.Core.Entities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Aspects
{
	internal class AspectRule
	{
		private readonly IAnalysisContext _context;
		private readonly AttributeReceptor _attributeReceptor;

		public AspectRule(IAnalysisContext context)
		{
			_context = context;
			_attributeReceptor = new AttributeReceptor(this);
		}

		public ClassAspect? Aggregate(ClassDeclarationSyntax root)
		{
			var hasAttr = root.AttributeLists.HasAttribute(new AttributeName(nameof(FactoryAttribute)));
			var isPartial = root.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));

			if (!hasAttr || !isPartial)
			{
				return null;
			}

			if (_context.GetNamedTypeSymbol(root) is not { } symbol)
			{
				return null;
			}

			var baseClasses = TraverseBase(symbol).Select(x =>
			{
				var syntax = x.DeclaringSyntaxReferences
					.Select(y => y.GetSyntax())
					.OfType<ClassDeclarationSyntax>()
					.FirstOrDefault();
				return syntax is not null
					? ExtractAspect(syntax, symbol)
					: null;
			}).FilterNull().ToArray();

			return ExtractAspect(root, symbol, baseClasses);

			static IEnumerable<INamedTypeSymbol> TraverseBase(INamedTypeSymbol pivot)
			{
				if (pivot.BaseType is not null)
				{
					yield return pivot.BaseType;
					foreach (var baseSymbol in TraverseBase(pivot.BaseType))
					{
						yield return baseSymbol;
					}
				}
			}
		}

		private ClassAspect ExtractAspect(ClassDeclarationSyntax syntax,
			INamedTypeSymbol symbol,
			ClassAspect[]? baseClasses = null)
		{
			var methods = syntax.Members
				.OfType<MethodDeclarationSyntax>()
				.Select(ExtractAspect)
				.FilterNull()
				.ToArray();

			var properties = syntax.Members
				.OfType<PropertyDeclarationSyntax>()
				.Select(ExtractAspect)
				.FilterNull()
				.ToArray();

			return new ClassAspect(TypeNode.FromSymbol(symbol),
				baseClasses ?? new ClassAspect[0],
				methods,
				properties);
		}

		private MethodAspect? ExtractAspect(MethodDeclarationSyntax syntax)
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
				.Select(x => _attributeReceptor.ExtractAspect(x, returnSymbol, symbol.Name))
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
			return new(ExtractTypeToCreate(symbol), symbol.IsAbstract);
		}

		internal TypeToCreate ExtractTypeToCreate(INamedTypeSymbol symbol, params ITypeSymbol[] typeArguments)
		{
			var args = symbol.Constructors.FirstOrDefault()?.Parameters
				.Select(x => TypeNode.FromSymbol(x.Type))
				.ToArray() ?? new TypeNode[0];

			if (symbol.IsUnboundGenericType)
			{
				symbol = symbol.ConstructedFrom.Construct(typeArguments);
			}

			return new TypeToCreate(TypeNode.FromSymbol(symbol), args);
		}

		private ParameterAspect? ExtractAspect(ParameterSyntax syntax)
		{
			if (syntax.Type is null || _context.GetTypeSymbol(syntax.Type) is not { } symbol)
			{
				return null;
			}

			return new ParameterAspect(TypeNode.FromSymbol(symbol), symbol.Name);
		}

		private PropertyAspect? ExtractAspect(PropertyDeclarationSyntax syntax)
		{
			if (_context.GetPropertySymbol(syntax) is not { } symbol)
			{
				return null;
			}

			var isDelegation = symbol.Type.GetAttributes()
				.Select(x => x.AttributeClass)
				.FilterNull()
				.Select(x => new AttributeName(x.Name))
				.Any(x => x.NameWithAttributeSuffix == nameof(FactoryAttribute));
			if (!isDelegation)
			{
				return null;
			}

			var methods = symbol.Type.GetMembers().OfType<IMethodSymbol>()
				.Select(m =>
				{
					var mm = m.DeclaringSyntaxReferences
						.Select(x => x.GetSyntax())
						.OfType<MethodDeclarationSyntax>()
						.FirstOrDefault();
					return mm is null ? null : ExtractAspect(mm);
				}).FilterNull().ToArray();

			return new PropertyAspect(TypeNode.FromSymbol(symbol.Type),
				symbol.Name,
				methods);
		}
	}
}
