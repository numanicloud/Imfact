using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Imfact.Annotations;
using Imfact.Entities;
using Imfact.Interfaces;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imfact.Steps.Aspects.Rules
{
	internal sealed class MethodRule
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

		public MethodAspect? ExtractAspect(MethodDeclarationSyntax syntax, IMethodSymbol symbol, bool partialOnly = false)
		{
			using var profiler = TimeProfiler.Create("Extract-Method-Aspect");
			if (partialOnly && !IsResolverPartial(syntax)
				|| symbol.ReturnType is not INamedTypeSymbol returnSymbol)
			{
				return null;
			}

			return new MethodAspect(symbol.Name,
				symbol.DeclaredAccessibility,
				GetKind(returnSymbol),
				GetReturnType(returnSymbol),
				GetAttributes(symbol, returnSymbol),
				GetParameters(symbol));
		}

		public MethodAspect? ExtractNotAsRootResolver(IMethodSymbol symbol)
		{
			if (symbol.ReturnType is not INamedTypeSymbol returnSymbol)
			{
				return null;
			}

			return new MethodAspect(symbol.Name,
				symbol.DeclaredAccessibility,
				GetKind(returnSymbol),
				GetReturnType(returnSymbol),
				GetAttributes(symbol, returnSymbol),
				GetParameters(symbol));
		}

		public ExporterAspect? ExtractExporterAspect
			(MethodDeclarationSyntax syntax, IMethodSymbol symbol)
		{
			using var profiler = TimeProfiler.Create("Extract-Exporter-Aspect");
			// Exporterである条件は：
			// ExporterAttributeがついているメソッドで、
			// 型引数が2つで、
			// 2つめの引数の型が2つめの型引数の値を返すFuncであること。
			if (symbol.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name == nameof(ExporterAttribute)) is null)
			{
				return null;
			}

			var typeParameters = syntax.TypeParameterList?.Parameters
				.Select(p =>
					new TypeAnalysis(new TypeId("", p.Identifier.ValueText, RecordArray<TypeId>.Empty),
						Accessibility.NotApplicable, DisposableType.NonDisposable))
				.ToArray() ?? Array.Empty<TypeAnalysis>();

			if (typeParameters.Length != 1)
			{
				return null;
			}

			var parameters = GetParameters(symbol);
			
			if (parameters.Length != 2
				&& parameters[1].Name != $"Func"
				&& parameters[1].Type.TypeArguments[0].Name == typeParameters[0].Name)
			{
				return null;
			}

			return new ExporterAspect(symbol.Name,
				GetParameters(symbol),
				typeParameters);
		}

		private ParameterAspect[] GetParameters(IMethodSymbol symbol)
		{
			return symbol.Parameters
				.Select(p => new ParameterAspect(TypeAnalysis.FromSymbol(p.Type), p.Name))
				.ToArray();
		}

		private MethodAttributeAspect[] GetAttributes(IMethodSymbol symbol, INamedTypeSymbol returnSymbol)
		{
			using var profiler = TimeProfiler.Create("Extract-Attribute-Aspect");
			return symbol.GetAttributes()
				.Select(x => _attributeRule.ExtractAspect(x, returnSymbol, symbol.Name))
				.FilterNull()
				.ToArray();
		}

		private ReturnTypeAspect GetReturnType(INamedTypeSymbol symbol)
		{
			using var profiler = TimeProfiler.Create("Extract-ReturnType-Aspect");
			return new(_typeRule.ExtractTypeToCreate(symbol), symbol.IsAbstract);
		}

		private static ResolverKind GetKind(INamedTypeSymbol returnSymbol)
		{
			var idSymbol = returnSymbol.ConstructedFrom;
			var typeNameValid = idSymbol.MetadataName == typeof(IEnumerable<>).Name;
			var typeArgValid = idSymbol.TypeArguments.Length == 1;
			return typeNameValid && typeArgValid ? ResolverKind.Multi : ResolverKind.Single;
		}

		private static bool IsResolverPartial(MethodDeclarationSyntax syntax)
		{
			return syntax.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword));
		}
	}
}
