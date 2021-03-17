using System;
using System.Collections.Generic;
using System.Linq;

namespace Imfact.Utilities
{
	public static class Helpers
	{
		public static TValue? GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key)
			where TValue : notnull
		{
			if (dic.TryGetValue(key, out var value))
			{
				return value;
			}

			return default;
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

		public static Dictionary<TKey, TValue> ToDictionaryWithDistinct<T, TKey, TValue>(
			this IEnumerable<T> source, Func<T, TKey> keySelector, Func<T, TValue> valueSelector,
			Func<IEnumerable<T>, T> selectFromGroup)
		{
			return source.GroupBy(keySelector).Select(selectFromGroup)
				.ToDictionary(keySelector, valueSelector);
		}

		/// <summary>
		/// 単一の値を、それ1つだけを含むコレクションに変換します。
		/// </summary>
		/// <typeparam name="T">値の型。</typeparam>
		/// <param name="source">コレクションに含む値。</param>
		/// <returns></returns>
		public static IEnumerable<T> WrapOrEmpty<T>(this T? source) where T : class
		{
			return source is null
				? Enumerable.Empty<T>()
				: new T[] { source };
		}

		/// <summary>
		/// 単一の値を、それ1つだけを含む配列に変換します。
		/// </summary>
		/// <typeparam name="T">値の型。</typeparam>
		/// <param name="item">配列に含む値。</param>
		/// <returns></returns>
		public static T[] WrapByArray<T>(this T item)
		{
			return new T[] { item };
		}

		public static IEnumerable<T> FilterNull<T>
			(this IEnumerable<T?> source) where T : struct
		{
			foreach (var item in source)
			{
				if (item.HasValue)
				{
					yield return item.Value;
				}
			}
		}

		public static IEnumerable<T> FilterNull<T>(this IEnumerable<T?> source) where T : notnull
		{
			foreach (var item in source)
			{
				if (item is not null)
				{
					yield return item;
				}
			}
		}

		public static IEnumerable<TR> FilterMap<T, TR>
			(this IEnumerable<T> source, Func<T, TR?> map)
			where T : notnull
			where TR : notnull
		{
			foreach (var item in source)
			{
				if (map(item) is {} value)
				{
					yield return value;
				}
			}
		}

		public static (T1, T2) Pair<T1, T2>(this T1 item1, T2 item2)
		{
			return (item1, item2);
		}
	}
}
