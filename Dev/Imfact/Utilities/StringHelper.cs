using System.Collections.Generic;
using System.Linq;

namespace Deptorygen2.Core.Utilities
{
	internal static class StringHelper
	{
		public static string Join(this IEnumerable<string> elements, string delimiter)
		{
			return string.Join(delimiter, elements);
		}

		public static string ToLowerCamelCase(this string name)
		{
			return name[0].ToString().ToLower() + name.Substring(1);
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
