namespace Deptorygen2.Core.Utilities
{
	internal record AttributeName(string NameWithAttributeSuffix)
	{
		public string NameWithoutSuffix => NameWithAttributeSuffix
			.TrimEnd("Attribute".ToCharArray());

		public bool MatchWithAnyName(string attributeName)
		{
			return attributeName == NameWithAttributeSuffix
			       || attributeName == NameWithoutSuffix;
		}
	}
}
