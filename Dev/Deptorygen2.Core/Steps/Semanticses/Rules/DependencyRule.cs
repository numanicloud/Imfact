using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Utilities;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Semanticses.Rules
{
	internal sealed class DependencyRule
	{
		public Dependency[] Extract(Factory factory)
		{
			var delegated = factory.Delegations.Cast<IFactorySemantics>()
				.Concat(factory.Inheritances)
				.SelectMany(x => x.Resolvers).Cast<IResolverSemantics>()
				.Concat(factory.Resolvers)
				.Concat(factory.MultiResolvers);

			var members = factory.Type.WrapByArray()
				.Concat(factory.Delegations.Select(x => x.Type))
				.Concat(factory.Inheritances.Select(x => x.Type))
				.GroupBy(x => x.Record)
				.Select(x => x.First())
				.ToDictionary(x => x.Record, x => x);

			var provided = delegated
				.GroupBy(x => x.ReturnType.Record)
				.Select(x => x.First())
				.ToDictionary(x => x.ReturnType.Record, x => x);

			// リゾルバーの依存先のうち、解決可能なものはフィールド化しない。
			// リゾルバーのうち、その戻り値が解決可能なものはリゾルバーの依存先をフィールド化しない。
			// ただし、自分自身を呼び出すことで解決できるようなものは解決可能とは見なさない。
			var forResolve = factory.Resolvers
				.SelectMany(x =>
				{
					var res = x.ReturnTypeResolution is null
						? x.Resolutions
						: x.ReturnTypeResolution.WrapByArray();
					return res.Where(r =>
					{
						var inverse = provided.GetValueOrDefault(r.TypeName.Record) is { } p 
						              && p.MethodName != x.MethodName;
						return !inverse;
					});
				})
				.Where(x => !members.ContainsKey(x.TypeName.Record))
				.SelectMany(x => x.Dependencies);

			var forMultiResolve = factory.MultiResolvers
				.SelectMany(x =>
				{
					return x.Resolutions.Where(r =>
					{
						var inverse = provided.GetValueOrDefault(r.TypeName.Record) is { } p
						              && p.MethodName != x.MethodName;
						return !inverse;
					});
				})
				.Where(x => !members.ContainsKey(x.TypeName.Record))
				.SelectMany(x => x.Dependencies);

			return forResolve.Concat(forMultiResolve)
				.Select(x => new Dependency(x, "_" + x.Name.ToLowerCamelCase()))
				.ToArray();
		}
	}
}
