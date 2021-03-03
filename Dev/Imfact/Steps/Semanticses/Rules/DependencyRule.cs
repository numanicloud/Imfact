using System.Linq;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Semanticses.Rules
{
	internal sealed class DependencyRule
	{
		public Dependency[] Extract(Factory factory)
		{
			// 戻り値からの依存先は、自身のリゾルバーを使ってもよい
			var cannotReturn = factory.Resolvers
				.Concat<IResolverSemantics>(factory.MultiResolvers)
				.Select(x => x.ReturnType)
				.ToDictionaryWithDistinct(x => x.Record, x => x, g => g.First());

			// 戻り値そのものは委譲または継承されたリゾルバーのみを使って解決できる
			var canReturn = factory.Delegations
				.Concat<IFactorySemantics>(factory.Inheritances)
				.SelectMany(x =>
					x.Resolvers.Concat<IResolverSemantics>(x.MultiResolvers))
				.Select(x => x.ReturnType)
				.Append(factory.Type)
				.Concat(factory.Delegations.Select(x => x.Type))
				.Concat(factory.Inheritances.Select(x => x.Type))
				.ToDictionaryWithDistinct(x => x.Record, x => x, g => g.First());

			// 戻り値そのものが解決されてしまうなら、その依存先も解決された扱いにする
			var singleResolutions = factory.Resolvers
				.Select(x => x.ActualResolution)
				.Where(x => !canReturn.ContainsKey(x.TypeName.Record));

			var multiResolutions = factory.MultiResolvers
				.SelectMany(x => x.Resolutions);

			// 依存先たちの中でも、ファクトリー内のリゾルバーで解決できるものはファクトリーとしての依存先にはならない
			return singleResolutions.Concat(multiResolutions)
				.SelectMany(x => x.Dependencies)
				.Where(x => !canReturn.ContainsKey(x.Record))
				.Where(x => !cannotReturn.ContainsKey(x.Record))
				.Select(x => new Dependency(x, "_" + x.Name.ToLowerCamelCase()))
				.ToArray();
		}
	}
}
