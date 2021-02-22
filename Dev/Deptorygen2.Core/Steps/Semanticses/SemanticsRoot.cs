namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record SemanticsRoot(
		string[] RequiredNamespaces, Factory Factory,
		Dependency[] Dependencies, DisposableInfo DisposableInfo);
}