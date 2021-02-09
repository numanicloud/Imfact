using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Semanticses
{
	internal record ResolutionSemantics(TypeName TypeName, TypeName[] Dependencies, bool IsDisposable);
}
