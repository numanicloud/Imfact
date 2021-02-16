using System;
using System.Collections.Generic;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using static Deptorygen2.Core.Utilities.DisposableType;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record TypeNode(TypeRecord Record,
		Accessibility Accessibility,
		DisposableType DisposableType) : INamespaceClaimer
	{
		public IEnumerable<ISemanticsNode> Traverse()
		{
			yield break;
		}

		public IEnumerable<string> GetRequiredNamespaces()
		{
			yield return Record.FullNamespace;
		}

		public static TypeNode FromSymbol(INamedTypeSymbol symbol)
		{
			var type = symbol.ConstructedFrom;
			var dispose = type.IsImplementing(typeof(IAsyncDisposable)) ? AsyncDisposable
				: type.IsImplementing(typeof(IDisposable)) ? Disposable
				: NonDisposable;

			return new TypeNode(TypeRecord.FromSymbol(symbol),
				symbol.DeclaredAccessibility,
				dispose);
		}

		public static TypeNode FromRuntime(Type type, TypeRecord[]? typeArguments = null)
		{
			var dispose = type.GetInterface(nameof(IAsyncDisposable)) is not null ? AsyncDisposable
				: type.GetInterface(nameof(IDisposable)) is not null ? Disposable
				: NonDisposable;

			return new TypeNode(TypeRecord.FromRuntime(type, typeArguments),
				type.IsPublic ? Accessibility.Public : Accessibility.Internal,
				dispose);
		}
	}
}
