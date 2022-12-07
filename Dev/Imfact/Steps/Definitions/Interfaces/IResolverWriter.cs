namespace Imfact.Steps.Definitions.Interfaces;

internal interface IResolverWriter
{
	void Render(string returnTypeName, Hook[] hooks, string expression, ICodeBuilder builder);
}