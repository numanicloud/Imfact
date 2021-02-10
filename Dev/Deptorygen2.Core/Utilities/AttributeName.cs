using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deptorygen2.Core.Utilities
{
	internal record AttributeName(string NameWithAttributeSuffix)
	{
		private static readonly int TrailLength = "Attribute".Length;

		public string NameWithoutSuffix => NameWithAttributeSuffix[..^TrailLength];

		public bool MatchWithAnyName(string attributeName)
		{
			return attributeName == NameWithAttributeSuffix
			       || attributeName == NameWithoutSuffix;
		}
	}
}
