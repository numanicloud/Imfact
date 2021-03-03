using System;
using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Entities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Utilities
{
	internal static class AnalyzerHelper
	{
		public static bool IsImplementing(this INamedTypeSymbol symbol, Type @interface)
		{
			var typeName = TypeName.FromType(@interface);
			return symbol.AllInterfaces
				.Any(x => x.Name == typeName.NameWithoutArguments
						  && x.GetFullNameSpace() == typeName.FullNamespace);
		}

		public static bool HasAttribute(this IEnumerable<AttributeListSyntax> attributes, AttributeName attributeName)
		{
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
	}
}
