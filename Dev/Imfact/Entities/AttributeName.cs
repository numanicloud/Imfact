namespace Imfact.Entities;

internal record AttributeName(string NameWithAttributeSuffix)
{
	public string NameWithoutSuffix { get; } = NameWithAttributeSuffix
		.TrimEnd("Attribute".ToCharArray());

	public bool MatchWithAnyName(string attributeName)
	{
		return attributeName == NameWithAttributeSuffix
			|| attributeName == NameWithoutSuffix;
	}
}