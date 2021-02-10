﻿using System;
using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Parser
{
	internal delegate void DelegationResolverLoader(FactoryAnalysisContext delegated,
		List<ResolverSemantics> delegatedResolvers,
		List<CollectionResolverSemantics> delegatedCollectionResolvers);

	internal record DelegationFact(PropertyDeclarationSyntax Syntax)
	{

	}

	// 構文解析担当。
	// どのようなプロパティを探すか、という条件を外から与える
	internal class DelegationLoader
	{
		private readonly Predicate<PropertyInfo> _filter;

		public DelegationLoader(Predicate<PropertyInfo> filter)
		{
			_filter = filter;
		}

		public IEnumerable<DelegationSemantics> BuildDelegationSyntaxes(
			FactoryAnalysisContext factory,
			DelegationResolverLoader loadResolvers)
		{
			var properties = ExtractProperties(factory);

			foreach (var prop in properties)
			{
				var factoryContext = new FactoryAnalysisContext(
					prop.TypeSyntax,
					prop.TypeSymbol,
					factory.Context);
				var singleList = new List<ResolverSemantics>();
				var collectionList = new List<CollectionResolverSemantics>();

				loadResolvers(factoryContext, singleList, collectionList);

				yield return new DelegationSemantics(prop.Symbol.Name,
					TypeName.FromSymbol(prop.TypeSymbol),
					singleList.ToArray(),
					collectionList.ToArray());
			}
		}

		public IEnumerable<PropertyInfo> ExtractProperties(FactoryAnalysisContext factory)
		{
			return ExtractProperties(factory.Symbol, factory.Symbol.BaseType);
		}

		private IEnumerable<PropertyInfo> ExtractProperties(params INamedTypeSymbol?[] containers)
		{
			var propertyInfos = from members in
									from t in containers.FilterNull()
									select t.GetMembers()
								from member in members
								select GetPropertyInfo(member) into prop
								where _filter(prop)
								select prop;
			return propertyInfos.FilterNull();
		}

		private PropertyInfo? GetPropertyInfo(ISymbol symbol)
		{
			/* 解析都合の条件:
			 *		FieldType is INamedTypeSymbol
			 *		シンボルからシンタックスへ辿ることができる
			 * 仕様都合の条件
			 *		FactoryAttributeが付いてる
			 *		ReadOnlyである
			 */

			if (symbol is not IPropertySymbol { Type: INamedTypeSymbol type } prop)
			{
				return null;
			}

			var syntax = type.DeclaringSyntaxReferences
				.Select(x => x.GetSyntax())
				.OfType<ClassDeclarationSyntax>()
				.FirstOrDefault();

			if (syntax is null) return null;

			return new PropertyInfo(prop, type, syntax);
		}

		public record PropertyInfo(
			IPropertySymbol Symbol,
			INamedTypeSymbol TypeSymbol,
			ClassDeclarationSyntax TypeSyntax);
	}
}
