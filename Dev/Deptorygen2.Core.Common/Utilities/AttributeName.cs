namespace Deptorygen2.Core.Utilities
{
	public record AttributeName(string NameWithAttributeSuffix)
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
