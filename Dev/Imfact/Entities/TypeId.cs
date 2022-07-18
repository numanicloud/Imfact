using System;
using System.Linq;
using System.Text.RegularExpressions;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Entities
{
	// 等値性判定にはこちらを使う。追加の情報はTypeNodeに持たせる
	internal record TypeId(string FullNamespace, string Name, RecordArray<TypeId> Parameters)
	{
		public static TypeId FromSymbol(INamedTypeSymbol symbol)
		{
			var args = symbol.TypeArguments
				.Select(FromSymbol)
				.ToArray();
			return new(symbol.GetFullNameSpace(), symbol.Name, new RecordArray<TypeId>(args));
		}

		public static TypeId FromSymbol(ITypeSymbol symbol)
		{
			return symbol is INamedTypeSymbol nts
				? FromSymbol(nts)
				: symbol is ITypeParameterSymbol tps
					? new TypeId("", tps.Name, RecordArray<TypeId>.Empty)
					: throw new ArgumentException($"{nameof(symbol)} is not INamedTypeSymbol. This is {symbol.GetType()}.");
		}

		public static TypeId FromRuntime(Type type, TypeId[]? typeArguments = null)
		{
			var tpr = typeArguments is null
				? RecordArray<TypeId>.Empty
				: new RecordArray<TypeId>(typeArguments);

			return new(
				type.Namespace ?? "",
				Regex.Replace(type.Name, @"`\d+$", ""),
				tpr);
		}
	}
}
