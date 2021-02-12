using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Utilities
{
	public class TypeName : IEquatable<TypeName>
	{
		public TypeName(string fullNamespace, string name, Accessibility accessibility, TypeName[]? typeArguments = null)
		{
			FullNamespace = fullNamespace;
			Name = typeArguments is null || typeArguments.Length == 0 ? name
				: $"{name}<{typeArguments.Select(x => x.Name).Join(", ")}>";
			NameWithoutArguments = name;
			Accessibility = accessibility;
			TypeArguments = typeArguments ?? new TypeName[0];
		}

		public string FullNamespace { get; }
		public string Name { get; }
		public Accessibility Accessibility { get; }
		public TypeName[] TypeArguments { get; }
		public string LowerCamelCase => NameWithoutArguments.ToLowerCamelCase();
		public string NameWithoutArguments { get; }

		public override string ToString()
		{
			return $"{FullNamespace}.{Name} ({Accessibility})";
		}

		public bool Equals(TypeName? other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return FullNamespace == other.FullNamespace && Name == other.Name
				&& TypeArguments.SequenceEqual(other.TypeArguments);
		}

		public override bool Equals(object? obj)
		{
			if (obj is null) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((TypeName) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (FullNamespace.GetHashCode() * 397) ^ Name.GetHashCode();
			}
		}

		public static TypeName FromSymbol(ITypeSymbol symbol)
		{
			return symbol is INamedTypeSymbol nts ? FromSymbol(nts) : throw new ArgumentException();
		}

		public static TypeName FromSymbol(INamedTypeSymbol symbol)
		{
			var typeArguments = symbol.TypeArguments
				.Select(FromSymbol)
				.ToArray();

			return new TypeName(symbol.GetFullNameSpace(), symbol.Name, symbol.DeclaredAccessibility, typeArguments);
		}

		public static TypeName FromType(Type type)
		{
			var accessibility = type.IsPublic ? Accessibility.Public : Accessibility.Internal;

			return new TypeName(
				type.Namespace ?? "",
				type.Name,
				accessibility);
		}

		public static bool operator ==(TypeName lop, TypeName rop) => lop?.Equals(rop) ?? false;

		public static bool operator !=(TypeName lop, TypeName rop) => !(lop?.Equals(rop) ?? true);
	}
}
