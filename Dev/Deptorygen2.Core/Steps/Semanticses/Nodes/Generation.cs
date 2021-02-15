using Deptorygen2.Core.Steps.Aspects.Nodes;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Generation(string[] RequiredNamespaces,
		Factory Factory,
		Dependency[] Dependencies)
	{
		public static Builder<Class,
			(string[], Factory?, Dependency[]),
			Generation?> GetBuilder(Class @class)
		{
			return new(@class, tuple => tuple.Item2 is not null
				? new Generation(
					tuple.Item1, tuple.Item2, tuple.Item3)
				: null);
		}
	}
}
