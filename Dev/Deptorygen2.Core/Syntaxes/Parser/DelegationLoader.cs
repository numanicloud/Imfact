using System;
using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Structure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Syntaxes.Parser
{
	// 構文解析担当。
	// どのようなプロパティを探すか、という条件を外から与える
	internal class DelegationLoader
	{
		private readonly Predicate<PropertyInfo> _filter;

		public DelegationLoader(Predicate<PropertyInfo> filter)
		{
			_filter = filter;
		}

		public IEnumerable<PropertyInfo> ExtractProperties(FactoryAnalysisContext factory)
		{
			return ExtractProperties(factory.Symbol, factory.Symbol.BaseType);
		}

		public DelegationSyntax FromPropertyInfo(SourceGenAnalysisContext context, PropertyInfo prop)
		{
			var factoryContext = new FactoryAnalysisContext(
				prop.TypeSyntax,
				prop.TypeSymbol,
				context);

			var resolvers = ResolverSyntax.FromParent(factoryContext);

			return new DelegationSyntax(prop.Symbol.Name,
				TypeName.FromSymbol(prop.TypeSymbol),
				resolvers.Item1,
				resolvers.Item2);
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
			 *		Type is INamedTypeSymbol
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
