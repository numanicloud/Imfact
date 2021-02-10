using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Parser
{
	internal class ParameterLoader
	{
		private readonly IAnalysisContext _context;

		public ParameterLoader(IAnalysisContext context)
		{
			_context = context;
		}

		public ParameterSemantics? Build(ParameterSyntax syntax)
		{
			if (syntax.Type is null
				|| _context.GeTypeSymbol(syntax.Type) is not {} typeSymbol)
			{
				return null;
			}

			return new ParameterSemantics(TypeName.FromSymbol(typeSymbol),
				syntax.Identifier.ValueText);
		}
	}
}
