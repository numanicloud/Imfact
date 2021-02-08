using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deptorygen2.Core.Structure;
using Deptorygen2.Core.Syntaxes.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Syntaxes
{
	class FactorySyntax : Interfaces.IServiceProvider
	{
		public INamedTypeSymbol ItselfSymbol { get; }
		public ResolverSyntax[] Resolvers { get; }
		public CollectionResolverSyntax[] CollectionResolvers { get; }
		public DelegationSyntax[] Delegations { get; }

		public FactorySyntax(INamedTypeSymbol itselfSymbol,
			ResolverSyntax[] resolvers,
			CollectionResolverSyntax[] collectionResolvers,
			DelegationSyntax[] delegations)
		{
			ItselfSymbol = itselfSymbol;
			Resolvers = resolvers;
			CollectionResolvers = collectionResolvers;
			Delegations = delegations;
		}

		public static async Task<FactorySyntax> FromDeclarationAsync(
			ClassDeclarationSyntax syntax,
			SourceGenAnalysisContext context)
		{
			var factoryLoader = new FactoryLoader();
			var structure = await factoryLoader.GetContextAsync(syntax, context);

			var resolvers = ResolverSyntax.FromParent(structure);
			var delegations = DelegationSyntax.FromFactory(structure).ToArray();

			return new FactorySyntax(structure.Symbol, resolvers.Item1, resolvers.Item2, delegations);
		}

		public IEnumerable<TypeName> GetCapableServiceTypes()
		{
			yield return TypeName.FromSymbol(ItselfSymbol);
			foreach (var delegation in Delegations)
			{
				yield return delegation.TypeName;
			}
		}
	}
}
