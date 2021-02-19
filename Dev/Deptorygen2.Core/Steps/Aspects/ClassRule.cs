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
	internal class ClassRule
	{
		private readonly IAnalysisContext _context;
		private readonly MethodRule _methodRule;

		public ClassRule(IAnalysisContext context)
		{
			var typeRule = new TypeRule();

			_context = context;

			_methodRule = new MethodRule(context,
				new AttributeRule(typeRule),
				typeRule);
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
				.Select(syntax1 => _methodRule.ExtractAspect(syntax1))
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
					return mm is null ? null : _methodRule.ExtractAspect(mm);
				}).FilterNull().ToArray();

			return new PropertyAspect(TypeNode.FromSymbol(symbol.Type),
				symbol.Name,
				methods);
		}
	}
}
