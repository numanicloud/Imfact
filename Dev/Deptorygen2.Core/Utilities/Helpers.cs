using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Utilities
{
	public static class Helpers
	{
		public static string GetFullNameSpace(this ITypeSymbol typeSymbol)
		{
			static IEnumerable<string> GetFullNameSpace(INamespaceSymbol nss)
			{
				if (nss.IsGlobalNamespace)
				{
					yield break;
				}

				var ns = nss.ContainingNamespace;
				foreach (var part in GetFullNameSpace(ns))
				{
					yield return part;
				}

				yield return nss.Name;
			}

			return GetFullNameSpace(typeSymbol.ContainingNamespace).Join(".");
		}
		
		public static string ToLowerCamelCase(this string name)
		{
			return name[0].ToString().ToLower() + name.Substring(1);
		}

		public static IEnumerable<T> AsEnumerable<T>(this T? source) where T : class
		{
			return source is null
				? Enumerable.Empty<T>()
				: new T[] { source };
		}

		public static TValue? GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key)
			where TValue : notnull
		{
			if (dic.TryGetValue(key, out var value))
			{
				return value;
			}

			return default;
		}

		public static Accessibility GetTypeAccessibilityMostStrict(
			params Accessibility[] accessibilities)
		{
			return accessibilities.Contains(Accessibility.Internal)
				? Accessibility.Internal
				: Accessibility.Public;
		}

		public static TResult? Then<T, TResult>(this T? prevStep, Func<T, TResult?> func)
			where T : class
			where TResult : class
		{
			if (prevStep is null)
			{
				return null;
			}

			return func(prevStep);
		}
	}
}
