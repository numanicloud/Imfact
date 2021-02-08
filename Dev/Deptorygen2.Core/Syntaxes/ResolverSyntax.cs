using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Structure;
using Deptorygen2.Core.Syntaxes.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

		public ResolverSyntax(string methodName,
			TypeName returnTypeName,
			ResolutionSyntax? returnTypeResolution,
			ResolutionSyntax[] resolutions,
			ParameterSyntax[] parameters,
			Accessibility accessibility)
		{
			MethodName = methodName;
			ReturnTypeName = returnTypeName;
			ReturnTypeResolution = returnTypeResolution;
			Resolutions = resolutions;
			Parameters = parameters;
			Accessibility = accessibility;
		}

		public static (ResolverSyntax[], CollectionResolverSyntax[]) FromParent(FactoryAnalysisContext factory)
		{
			// 1. あるコンテナ群から
			// 2. 対象を収集して
			// 3. 実際に生成元とすべきものだけに絞り込み
			// 4. そういった対象についての必要な情報を持つ Syntax クラスを生成して
			// 5. そのリストを返す

			var loader = new ResolverLoader(method =>
				method.Syntax.IsPartial() && method.Symbol.MethodKind == MethodKind.Ordinary);
			var singleLoader = new SingleResolverLoader();
			var collectionLoader = new CollectionResolverLoader();

			var resolvers = loader.ExtractResolverMethods(factory);

			var resolverResult = new List<ResolverSyntax>();
			var collectionResult = new List<CollectionResolverSyntax>();

			foreach (var item in resolvers)
			{
				if (collectionLoader.FromResolver(item.Symbol) is { } cr)
				{
					collectionResult.Add(cr);
				}
				else if (singleLoader.FromResolver(item) is {} r)
				{
					resolverResult.Add(r);
				}
			}

			return (resolverResult.ToArray(), collectionResult.ToArray());
		}

		private static ResolverSyntax? FromResolver(ResolverStructure item)
		{
			if (item.Symbol.ReturnType is not INamedTypeSymbol returnType)
			{
				return null;
			}

			return new ResolverSyntax(
				item.Symbol.Name,
				TypeName.FromSymbol(returnType),
				ResolutionSyntax.FromType(returnType),
				ResolutionSyntax.FromResolversAttribute(item.Symbol),
				ParameterSyntax.FromResolver(item.Symbol),
				item.Symbol.DeclaredAccessibility);
		}

		private static IEnumerable<ResolverStructure> ExtractResolverMethods(FactoryAnalysisContext factory)
		{
			// partial情報の取り方：
			// https://stackoverflow.com/questions/23026884/how-to-determine-that-a-partial-method-has-no-implementation
			// https://github.com/dotnet/roslyn/issues/48

			static bool IsPartialMethod(ResolverStructure structure)
			{
				return structure.Syntax.Body != null
					&& structure.Syntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
			}

			ResolverStructure? GetStructure(IMethodSymbol symbol)
			{
				var syntax = symbol.DeclaringSyntaxReferences
					.Select(x => x.GetSyntax())
					.OfType<MethodDeclarationSyntax>()
					.FilterNull()
					.FirstOrDefault();
				return syntax is null ? null : new ResolverStructure(syntax, symbol, factory);
			}

			var holders = new[] { factory.Symbol, factory.Symbol.BaseType };

			var structures = from members in
								 from holder in holders.FilterNull()
								 select holder.GetMembers()
							 from member in members.OfType<IMethodSymbol>()
							 where member.MethodKind == MethodKind.Ordinary
							 select GetStructure(member);

			return from structure in structures.FilterNull()
				   where IsPartialMethod(structure)
				   select structure;
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
