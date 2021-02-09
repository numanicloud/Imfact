using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Syntaxes
{
	internal record ResolutionSemantics(TypeName TypeName, TypeName[] Dependencies, bool IsDisposable);
}
