using Deptorygen2.Core.Steps.Aggregation;

namespace Deptorygen2.Core.Steps.Semanticses
{
	internal record GenerationSemantics(string[] RequiredNamespaces,
		FactorySemantics Factory,
		DependencySemantics[] Dependencies)
	{
		public static Builder<ClassToAnalyze,
			(string[], FactorySemantics?, DependencySemantics[]),
			GenerationSemantics?> GetBuilder(ClassToAnalyze @class)
		{
			return new(@class, tuple => tuple.Item2 is not null
				? new GenerationSemantics(
					tuple.Item1, tuple.Item2, tuple.Item3)
				: null);
		}
	}
}
