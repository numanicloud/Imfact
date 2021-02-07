using System.Collections.Generic;
using System.Linq;
using Deptorygen.Generator.Interfaces;
using Deptorygen.Utilities;
using Deptorygen2.Core.Syntaxes;
using Microsoft.CodeAnalysis;

namespace Deptorygen.Generator.Syntaxes
{
	class ResolverSyntax : IServiceConsumer, IServiceProvider
	{
		public string MethodName { get; }
		public TypeName ReturnTypeName { get; }
		public ResolutionSyntax? ReturnTypeResolution { get; }
		public ResolutionSyntax[] Resolutions { get; }
		public ParameterSyntax[] Parameters { get; }
		public Accessibility Accessibility { get; }
		public string? DelegationKey { get; }

		public ResolverSyntax(string methodName,
			TypeName returnTypeName,
			ResolutionSyntax? returnTypeResolution,
			ResolutionSyntax[] resolutions,
			ParameterSyntax[] parameters,
			Accessibility accessibility, string? delegationKey)
		{
			MethodName = methodName;
			ReturnTypeName = returnTypeName;
			ReturnTypeResolution = returnTypeResolution;
			Resolutions = resolutions;
			Parameters = parameters;
			Accessibility = accessibility;
			DelegationKey = delegationKey;
		}

		public static (ResolverSyntax[], CollectionResolverSyntax[]) FromParent(INamedTypeSymbol factory)
		{
			var baseInterfaces = factory.AllInterfaces
				.SelectMany(x => x.GetMembers())
				.OfType<IMethodSymbol>();

			var resolvers = factory.GetMembers()
				.OfType<IMethodSymbol>()
				.Concat(baseInterfaces)
				.Where(x => x.MethodKind == MethodKind.Ordinary)
				.ToArray();

			var resolverResult = new List<ResolverSyntax>();
			var collectionResult = new List<CollectionResolverSyntax>();

			foreach (var item in resolvers)
			{
				if (CollectionResolverSyntax.FromResolver(item) is {} cr)
				{
					collectionResult.Add(cr);
				}
				else if(item.ReturnType is INamedTypeSymbol returnType)
				{
					var delegation = item.GetAttributes()
						.FirstOrDefault(x => x.AttributeClass?.Name == nameof(DelegationAttribute))
						?.ConstructorArguments.First().Value;

					var r = new ResolverSyntax(
						item.Name,
						TypeName.FromSymbol(returnType),
						ResolutionSyntax.FromType(returnType),
						ResolutionSyntax.FromResolversAttribute(item),
						ParameterSyntax.FromResolver(item),
						item.DeclaredAccessibility,
						delegation as string);
					resolverResult.Add(r);
				}
			}

			return (resolverResult.ToArray(), collectionResult.ToArray());
		}

		public IEnumerable<TypeName> GetRequiredServiceTypes()
		{
			return ReturnTypeResolution.AsEnumerable()
				.Concat(Resolutions)
				.SelectMany(x => x.Dependencies)
				.Except(Parameters.Select(x => x.TypeName));
		}

		public IEnumerable<TypeName> GetCapableServiceTypes()
		{
			yield return ReturnTypeName;
		}
	}
}
