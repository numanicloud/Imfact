﻿using System;
using System.Linq;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using NacHelpers.Extensions;
using static Deptorygen2.Core.Utilities.DisposableType;

namespace Deptorygen2.Core.Entities
{
	internal record TypeNode(TypeRecord Record,
		Accessibility Accessibility,
		DisposableType DisposableType)
	{
		public string FullNamespace => Record.FullNamespace;
		public string Name => Record.Name;
		public TypeNode[] TypeArguments { get; init; } = new TypeNode[0];

		public string FullBoundName => TypeArguments.Any()
			? $"{Name}<{TypeArguments.Select(x => x.FullBoundName).Join(", ")}>"
			: Name;

		public static TypeNode FromSymbol(INamedTypeSymbol symbol)
		{
			var type = symbol.ConstructedFrom;
			var dispose = type.IsImplementing(typeof(IDisposable)) ? Disposable
				: IsImplementing(type, "System.IAsyncDisposable") ? AsyncDisposable
				: NonDisposable;

			var typeArguments = symbol.TypeArguments
				.Select(FromSymbol)
				.ToArray();

			return new TypeNode(TypeRecord.FromSymbol(symbol),
				symbol.DeclaredAccessibility,
				dispose)
			{
				TypeArguments = typeArguments
			};
		}

		private static bool IsImplementing(INamedTypeSymbol symbol, string interfaceName)
		{
			return symbol.AllInterfaces.Any(x =>
				$"{x.GetFullNameSpace()}.{x.Name}" == interfaceName);
		}

		public static TypeNode FromSymbol(ITypeSymbol symbol)
		{
			return symbol is INamedTypeSymbol nts
				? FromSymbol(nts)
				: throw new ArgumentException(nameof(symbol));
		}

		public static TypeNode FromRuntime(Type type, TypeNode[]? typeArguments = null)
		{
			var dispose = type.GetInterface(nameof(IDisposable)) is not null ? Disposable
				: type.GetInterface("IAsyncDisposable") is not null ? AsyncDisposable
				: NonDisposable;

			var args = typeArguments?.Select(x => x.Record).ToArray();

			return new TypeNode(TypeRecord.FromRuntime(type, args),
				type.IsPublic ? Accessibility.Public : Accessibility.Internal,
				dispose)
			{
				TypeArguments = typeArguments ?? new TypeNode[0]
			};
		}
	}
}