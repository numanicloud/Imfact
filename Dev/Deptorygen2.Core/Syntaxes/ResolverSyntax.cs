using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Structure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Deptorygen2.Core.Syntaxes
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

		public static (ResolverSyntax[], CollectionResolverSyntax[]) FromParent(FactoryAnalysisContext factory)
		{
			var resolvers = ExtractResolverMethods(factory);

			var resolverResult = new List<ResolverSyntax>();
			var collectionResult = new List<CollectionResolverSyntax>();

			foreach (var item in resolvers)
			{
				if (CollectionResolverSyntax.FromResolver(item) is { } cr)
				{
					collectionResult.Add(cr);
				}
				else if (item.ReturnType is INamedTypeSymbol returnType)
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

		private static async Task<IEnumerable<IMethodSymbol>> ExtractResolverMethods(FactoryAnalysisContext factory)
		{
			// partial情報の取り方：
			// https://stackoverflow.com/questions/23026884/how-to-determine-that-a-partial-method-has-no-implementation
			// https://github.com/dotnet/roslyn/issues/48

			static bool IsPartialMethod(IMethodSymbol method)
			{
				return method.DeclaringSyntaxReferences
					.Select(x => x.GetSyntax())
					.OfType<MethodDeclarationSyntax>()
					.FilterNull()
					.Where(node => node.Body != null)
					.Any(node => node.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)));
			}
			
			var holders = new[] { factory.Symbol, factory.Symbol.BaseType };

			return from members in
					   from holder in holders.FilterNull()
					   select holder.GetMembers()
				   from member in members.OfType<IMethodSymbol>()
				   where member.MethodKind == MethodKind.Ordinary
				   where IsPartialMethod(member)
				   select member;
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
