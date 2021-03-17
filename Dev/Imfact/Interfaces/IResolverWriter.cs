using Imfact.Steps.Definitions;
using Imfact.Steps.Writing.Coding;

namespace Imfact.Interfaces
{
	internal interface IResolverWriter
	{
		void Render(string returnTypeName, Hook[] hooks, string expression, ICodeBuilder builder);
	}
}
