using System;
using System.Linq;
using System.Text;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;
using static Imfact.Entities.DisposableType;

namespace Imfact.Entities
{
	internal record TypeAnalysis(TypeId Id,
		Accessibility Accessibility,
		DisposableType DisposableType)
	{
		public string FullNamespace => Id.FullNamespace;
		public string Name => Id.Name;
		public TypeAnalysis[] TypeArguments { get; init; } = new TypeAnalysis[0];

		public string FullBoundName => TypeArguments.Any()
			? $"{Name}<{TypeArguments.Select(x => x.FullBoundName).Join(", ")}>"
			: Name;

		protected virtual bool PrintMembers(StringBuilder builder)
		{
			builder.Append($"{Accessibility} {FullNamespace}.{FullBoundName}");
			return true;
		}

		public static TypeAnalysis FromSymbol(INamedTypeSymbol symbol)
		{
			var type = symbol.ConstructedFrom;
			var dispose = IsImplementing(type, "System.IDisposable") ? Disposable
				: IsImplementing(type, "System.IAsyncDisposable") ? AsyncDisposable
				: NonDisposable;

			var typeArguments = symbol.TypeArguments
				.Select(FromSymbol)
				.ToArray();

			return new TypeAnalysis(TypeId.FromSymbol(symbol),
				symbol.DeclaredAccessibility,
				dispose)
			{
				TypeArguments = typeArguments
			};
		}

		public static TypeAnalysis FromSymbol(ITypeSymbol symbol)
		{
			return symbol is INamedTypeSymbol nts
				? FromSymbol(nts)
				: throw new ArgumentException(nameof(symbol));
		}

		public static TypeAnalysis FromRuntime(Type type, TypeAnalysis[]? typeArguments = null)
		{
			var dispose = type.GetInterface(nameof(IDisposable)) is not null ? Disposable
				: type.GetInterface("IAsyncDisposable") is not null ? AsyncDisposable
				: NonDisposable;

			var args = typeArguments?.Select(x => x.Id).ToArray();

			return new TypeAnalysis(TypeId.FromRuntime(type, args),
				type.IsPublic ? Accessibility.Public : Accessibility.Internal,
				dispose)
			{
				TypeArguments = typeArguments ?? new TypeAnalysis[0]
			};
		}

		public static bool IsImplementing(INamedTypeSymbol symbol, string interfaceName)
		{
			return symbol.AllInterfaces.Any(x =>
				$"{x.GetFullNameSpace()}.{x.Name}" == interfaceName);
		}
	}
}


/* メモ
 * 型の (Runtime, Symbol) から (インターフェース実装, 名前, 名前空間, ...) を抽出
 * クラスの (Symbol, Syntax) から (名前, 型, メンバー, ...) を抽出
 * メソッドの (Symbol, Syntax) から (名前, 引数, ...) を抽出
 */
