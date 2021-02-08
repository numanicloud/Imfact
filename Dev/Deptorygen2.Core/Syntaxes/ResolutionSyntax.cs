using System;
using System.Linq;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Syntaxes
{
	class ResolutionSyntax
	{
		public TypeName TypeName { get; }
		public TypeName[] Dependencies { get; }
		public bool IsDisposable { get; }

		public ResolutionSyntax(TypeName typeName, TypeName[] dependencies, bool isDisposable)
		{
			TypeName = typeName;
			Dependencies = dependencies;
			IsDisposable = isDisposable;
		}

		public static ResolutionSyntax? FromType(INamedTypeSymbol symbol)
		{
			if (symbol.Constructors.SingleOrDefault() is not { } constructor)
			{
				return null;
			}

			return FromConstructor(symbol, constructor);
		}

		private static ResolutionSyntax FromConstructor(INamedTypeSymbol symbol, IMethodSymbol constructor)
		{
			var isDisposable = symbol.IsImplementing(typeof(IDisposable));

			var dependencies = constructor.Parameters
				.Select(x => TypeName.FromSymbol(x.Type))
				.ToArray();

			return new ResolutionSyntax(TypeName.FromSymbol(symbol), dependencies, isDisposable);
		}

		public static ResolutionSyntax[] FromResolversAttribute(IMethodSymbol resolver,
			Func<INamedTypeSymbol, ResolutionSyntax?> symbolToSyntax)
		{
			return resolver.GetAttributes()
				.Where(x => x.AttributeClass?.Name == nameof(ResolutionAttribute))
				.Where(x => x.ConstructorArguments.Length == 1)
				.Select(x => x.ConstructorArguments[0].Value)
				.OfType<INamedTypeSymbol>()
				.Select(symbolToSyntax)
				.FilterNull()
				.ToArray();
		}
	}
}
