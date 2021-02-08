using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core
{
	public static class Helpers
	{
		public static string Join(this IEnumerable<string> source, string delimiter)
		{
			return string.Join(delimiter, source);
		}

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

		// ReSharper disable once IdentifierTypo
		public static bool IsStructualEqual<T>(this IEnumerable<T> source, IEnumerable<T> second)
			where T : notnull
		{
			using var enumerator1 = source.GetEnumerator();
			using var enumerator2 = second.GetEnumerator();

			while (true)
			{
				var end1 = !enumerator1.MoveNext();
				var end2 = !enumerator2.MoveNext();

				if (end1 && end2)
				{
					return true;
				}

				if (end1 != end2 || !enumerator1.Current.Equals(enumerator2.Current))
				{
					return false;
				}
			}
		}

		public static bool IsSuperSet<T>(this IEnumerable<T> container, IEnumerable<T> contained)
		{
			var array = container.ToArray();
			foreach (var item in contained)
			{
				if (!array.Contains(item))
				{
					return false;
				}
			}

			return true;
		}


		public static bool HasAttribute(this ITypeSymbol symbol, string attributeName)
		{
			return symbol.GetAttributes()
				.Any(attr => attr.AttributeClass?.Name == attributeName);
		}

		public static string ToLowerCamelCase(this string name)
		{
			return name[0].ToString().ToLower() + name.Substring(1);
		}

		public static IEnumerable<T> FilterNull<T>(this IEnumerable<T?> source) where T : class
		{
			foreach (var item in source)
			{
				if (item is {})
				{
					yield return item;
				}
			}
		}

		public static IEnumerable<T> AsEnumerable<T>(this T? source) where T : class
		{
			return source is null
				? Enumerable.Empty<T>()
				: new T[] { source };
		}

		public static IEnumerable<(T item, bool isLast)> WithFooterFlag<T>(this IEnumerable<T> source)
			where T : class
		{
			T? prevItem = null;
			foreach (var item in source)
			{
				if (prevItem is {})
				{
					yield return (prevItem, false);
				}

				prevItem = item;
			}

			if (prevItem is {})
			{
				yield return (prevItem, true);
			}
		}

		public static string Indent(this string source, int level)
		{
			var indent = Enumerable.Repeat("\t", level).Join("");
			return source.Split('\n')
				.Select(x => indent + x)
				.Join("\n");
		}
	}
}
