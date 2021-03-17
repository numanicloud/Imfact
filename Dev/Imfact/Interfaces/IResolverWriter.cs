using Imfact.Steps.Definitions;

namespace Imfact.Interfaces
{
	internal interface IResolverWriter
	{
		void Render(string returnTypeName, Hook[] hooks, string expression, ICodeBuilder builder);
	}
}
