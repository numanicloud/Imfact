﻿using Imfact.Annotations;
using Imfact.Entities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Imfact.Interfaces;
using Imfact.Utilities;

namespace Imfact.Steps.Aspects.Rules
{
	internal class PropertyRule
	{
		private readonly IAnalysisContext _context;
		private readonly MethodRule _methodRule;

		public PropertyRule(IAnalysisContext context, MethodRule methodRule)
		{
			_context = context;
			_methodRule = methodRule;
		}

		public PropertyAspect? ExtractAspect(PropertyDeclarationSyntax syntax)
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

			return new PropertyAspect(TypeAnalysis.FromSymbol(symbol.Type),
				symbol.Name,
				methods);
		}
	}
}