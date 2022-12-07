using System;
using System.Collections.Generic;
using System.Linq;
using Imfact.Entities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imfact.Utilities;

internal static class AnalyzerHelper
{
	public static bool IsImplementing(this INamedTypeSymbol symbol, Type @interface)
	{
		var typeName = TypeAnalysis.FromRuntime(@interface);
		return symbol.AllInterfaces
			.Any(x => x.Name == typeName.Name
				&& x.GetFullNameSpace() == typeName.FullNamespace);
	}

	public static bool HasAttribute(this SyntaxList<AttributeListSyntax> attributes, AttributeName attributeName)
	{
		using var profiler = TimeProfiler.Create("HasAttribute");
		return attributes.SelectMany(a => a.Attributes)
			.Any(a => attributeName.MatchWithAnyName(a.Name.ToString()));
	}

	public static string GetFullNameSpace(this ITypeSymbol typeSymbol)
	{
		static IEnumerable<string> GetDescendingNameSpace(INamespaceSymbol nss)
		{
			if (nss.IsGlobalNamespace)
			{
				yield break;
			}

			var ns = nss.ContainingNamespace;
			foreach (var part in GetDescendingNameSpace(ns))
			{
				yield return part;
			}

			yield return nss.Name;
		}

		return GetDescendingNameSpace(typeSymbol.ContainingNamespace).Join(".");
	}

	public static Accessibility GetTypeAccessibilityMostStrict(
		params Accessibility[] accessibilities)
	{
		return accessibilities.Contains(Accessibility.Internal)
			? Accessibility.Internal
			: Accessibility.Public;
	}

	public static string ToKeyword(this Accessibility accessibility)
	{
		return accessibility switch
		{
			Accessibility.NotApplicable => "",
			Accessibility.Private => "private",
			Accessibility.ProtectedAndInternal => "private protected",
			Accessibility.Protected => "protected",
			Accessibility.Internal => "internal",
			Accessibility.ProtectedOrInternal => "protected internal",
			Accessibility.Public => "public",
			_ => throw new ArgumentOutOfRangeException(nameof(accessibility), accessibility, null)
		};
	}

	public static string GetCode(this TypeAnalysis type)
	{
		return $"{type.FullNamespace}.{type.Name}" switch
		{
			nameof(Byte) => "byte",
			nameof(Int16) => "short",
			nameof(Int32) => "int",
			nameof(Int64) => "long",
			nameof(SByte) => "sbyte",
			nameof(UInt16) => "ushort",
			nameof(UInt32) => "uint",
			nameof(UInt64) => "ulong",
			nameof(Single) => "float",
			nameof(Double) => "double",
			nameof(Decimal) => "decimal",
			nameof(Char) => "char",
			nameof(String) => "string",
			"System.Void" => "void",
			_ => type.FullBoundName
		};
	}
}