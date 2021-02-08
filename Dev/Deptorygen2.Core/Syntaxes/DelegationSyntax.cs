using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Structure;
using Deptorygen2.Core.Syntaxes.Parser;

namespace Deptorygen2.Core.Syntaxes
{
	class DelegationSyntax : IServiceProvider
	{
		public string PropertyName { get; }
		public TypeName TypeName { get; }
		public ResolverSyntax[] Resolvers { get; }
		public CollectionResolverSyntax[] CollectionResolvers { get; }

		public DelegationSyntax(string propertyName, TypeName typeName, ResolverSyntax[] resolvers, CollectionResolverSyntax[] collectionResolvers)
		{
			PropertyName = propertyName;
			TypeName = typeName;
			Resolvers = resolvers;
			CollectionResolvers = collectionResolvers;
		}

		public IEnumerable<TypeName> GetCapableServiceTypes()
		{
			return Resolvers.Cast<IServiceProvider>()
				.Concat(CollectionResolvers)
				.SelectMany(x => x.GetCapableServiceTypes());
		}

		public static IEnumerable<DelegationSyntax> FromFactory(FactoryAnalysisContext factory)
		{
			// 解析都合の絞り込みなどはLoader内でやる
			// 仕様都合の絞り込みなどはここでやる
			var loader = new DelegationLoader(prop =>
				prop.TypeSymbol.HasAttribute(nameof(FactoryAttribute))
				&& prop.Symbol.IsReadOnly);

			return loader.ExtractProperties(factory)
				.Select(x => loader.FromPropertyInfo(factory.Context, x));
		}
	}
}
