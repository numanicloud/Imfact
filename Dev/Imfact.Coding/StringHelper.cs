using System.Collections.Generic;
using System.Linq;

namespace Imfact.Coding
{
	public static class StringHelper
	{
		public static string Join(this IEnumerable<string> elements, string delimiter)
		{
			return string.Join(delimiter, elements);
		}

		public static string Indent(this string source, int level = 1)
		{
			var indent = Enumerable.Repeat("\t", level).Join("");
			return source.Split('\n')
				.Select(x => indent + x)
				.Join("\n");
		}
	}
}
