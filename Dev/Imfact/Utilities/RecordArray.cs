using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Imfact.Utilities;

internal record RecordArray<T>(T[] Elements) : IEnumerable<T> where T : IEquatable<T>
{
	public static readonly RecordArray<T> Empty = new(Array.Empty<T>());

	public T this[int index] => Elements[index];

	public virtual bool Equals(RecordArray<T>? other)
	{
		if (other?.Elements.Length != Elements.Length)
		{
			return false;
		}

		for (int i = 0; i < Elements.Length; i++)
		{
			if (!other.Elements[i].Equals(Elements[i]))
			{
				return false;
			}
		}

		return true;
	}

	public IEnumerator<T> GetEnumerator() => Elements.AsEnumerable().GetEnumerator();

	public override int GetHashCode()
	{
		return Elements.Aggregate(0,
			(acc, x) => acc * 457 + x.GetHashCode() * 389);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}