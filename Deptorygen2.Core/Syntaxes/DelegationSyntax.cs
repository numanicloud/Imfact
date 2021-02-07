using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Microsoft.CodeAnalysis;

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

		public static IEnumerable<DelegationSyntax> FromFactory(INamedTypeSymbol factory)
		{
			var propertyInfo = ExtractProperties(factory, factory.BaseType);

			foreach (var (type, prop) in propertyInfo)
			{
				var resolvers = ResolverSyntax.FromParent(type);

				yield return new DelegationSyntax(prop.Name,
					TypeName.FromSymbol(type),
					resolvers.Item1,
					resolvers.Item2);
			}
		}

		private static IEnumerable<(INamedTypeSymbol, IPropertySymbol)> ExtractProperties(
			params INamedTypeSymbol?[] holders)
		{
			return from members in
					from t in holders.FilterNull()
					select t.GetMembers()
				from prop in members.OfType<IPropertySymbol>()
				where prop.IsReadOnly
				where prop.Type.HasAttribute(nameof(FactoryAttribute))
				let type = prop.Type as INamedTypeSymbol
				where type is not null
				select (type, prop);
		}

		public IEnumerable<TypeName> GetCapableServiceTypes()
		{
			return Resolvers.Cast<IServiceProvider>()
				.Concat(CollectionResolvers)
				.SelectMany(x => x.GetCapableServiceTypes());
		}
	}
}
