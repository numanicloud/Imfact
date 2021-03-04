using System;
using System.Linq;
using System.Text.RegularExpressions;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Entities
{
	// 等値性判定にはこちらを使う。追加の情報はTypeNodeに持たせる
	internal record TypeId(string FullNamespace, string Name, TypeArgId Parameters)
	{
		public static TypeId FromSymbol(INamedTypeSymbol symbol)
		{
			var args = symbol.TypeArguments
				.Select(FromSymbol)
				.ToArray();
			return new(symbol.GetFullNameSpace(), symbol.Name, new TypeArgId(args));
		}

		public static TypeId FromSymbol(ITypeSymbol symbol)
		{
			return symbol is INamedTypeSymbol nts
				? FromSymbol(nts)
				: throw new ArgumentException(nameof(symbol));
		}

		public static TypeId FromRuntime(Type type, TypeId[]? typeArguments = null)
		{
			var tpr = typeArguments is null
				? TypeArgId.Empty
				: new TypeArgId(typeArguments);

			return new(
				type.Namespace ?? "",
				Regex.Replace(type.Name, @"`\d+$", ""),
				tpr);
		}
	}

	internal record TypeArgId(TypeId[] Arguments)
	{
		public static readonly TypeArgId Empty = new(new TypeId[0]);

		public virtual bool Equals(TypeArgId? other)
		{
			if (other?.Arguments.Length != Arguments.Length)
			{
				return false;
			}

			for (int i = 0; i < Arguments.Length; i++)
			{
				if (other.Arguments[i] != Arguments[i])
				{
					return false;
				}
			}

			return true;
		}

		public override int GetHashCode()
		{
			return Arguments.Aggregate(0,
				(acc, x) => unchecked(acc * 457 + x.GetHashCode() * 389));
		}
	}
}
