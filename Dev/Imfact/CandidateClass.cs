using Imfact.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imfact
{
	internal record CandidateClass(ClassDeclarationSyntax Syntax,
		IAnalysisContext Context);
}
