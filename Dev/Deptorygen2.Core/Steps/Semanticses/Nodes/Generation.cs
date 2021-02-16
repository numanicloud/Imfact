namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Generation(
		string[] RequiredNamespaces, Factory Factory,
		Dependency[] Dependencies, DisposableInfo DisposableInfo)
	{
	}
}
