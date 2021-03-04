using System;
using System.Linq;
using System.Text;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;
using static Imfact.Entities.DisposableType;

namespace Imfact.Entities
{
	internal record TypeNode(TypeId Id,
		Accessibility Accessibility,
		DisposableType DisposableType)
	{
		public string FullNamespace => Id.FullNamespace;
		public string Name => Id.Name;
		public TypeNode[] TypeArguments { get; init; } = new TypeNode[0];

		public string FullBoundName => TypeArguments.Any()
			? $"{Name}<{TypeArguments.Select(x => x.FullBoundName).Join(", ")}>"
			: Name;

		protected virtual bool PrintMembers(StringBuilder builder)
		{
			builder.Append($"{Accessibility} {FullNamespace}.{FullBoundName}");
			return true;
		}

		public static TypeNode FromSymbol(INamedTypeSymbol symbol)
		{
			var type = symbol.ConstructedFrom;
			var dispose = type.IsImplementing(typeof(IDisposable)) ? Disposable
				: IsImplementing(type, "System.IAsyncDisposable") ? AsyncDisposable
				: NonDisposable;

			var typeArguments = symbol.TypeArguments
				.Select(FromSymbol)
				.ToArray();

			return new TypeNode(TypeId.FromSymbol(symbol),
				symbol.DeclaredAccessibility,
				dispose)
			{
				TypeArguments = typeArguments
			};
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

			var args = typeArguments?.Select(x => x.Id).ToArray();

			return new TypeNode(TypeId.FromRuntime(type, args),
				type.IsPublic ? Accessibility.Public : Accessibility.Internal,
				dispose)
			{
				TypeArguments = typeArguments ?? new TypeNode[0]
			};
		}

		private static bool IsImplementing(INamedTypeSymbol symbol, string interfaceName)
		{
			return symbol.AllInterfaces.Any(x =>
				$"{x.GetFullNameSpace()}.{x.Name}" == interfaceName);
		}
	}
}
