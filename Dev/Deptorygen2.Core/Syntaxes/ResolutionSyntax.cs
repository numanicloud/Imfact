namespace Deptorygen2.Core.Syntaxes
{
	internal record ResolutionSyntax(TypeName TypeName, TypeName[] Dependencies, bool IsDisposable);
}
