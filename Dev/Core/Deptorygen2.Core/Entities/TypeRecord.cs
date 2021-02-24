using System;
using System.Text.RegularExpressions;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Entities
{
	// 等値性判定にはこちらを使う。追加の情報はTypeNodeに持たせる
	internal record TypeRecord(string FullNamespace, string Name)
	{
		public static TypeRecord FromSymbol(INamedTypeSymbol symbol)
		{
			return new (symbol.GetFullNameSpace(), symbol.Name);
		}

		public static TypeRecord FromSymbol(ITypeSymbol symbol)
		{
			return symbol is INamedTypeSymbol nts
				? FromSymbol(nts)
				: throw new ArgumentException(nameof(symbol));
		}

		public static TypeRecord FromRuntime(Type type, TypeRecord[]? typeArguments = null)
		{
			return new(
				type.Namespace ?? "",
				Regex.Replace(type.Name, @"`\d+$", ""));
		}
	}
}
