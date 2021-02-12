using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Semanticses
{
	internal static class SemanticsBuilder
	{
		public static Builder<TSource, TComp, TGoal> GetBuilder<TSource, TComp, TGoal>(
			TSource source,
			Func<TComp, TGoal> completion)
		{
			return new Builder<TSource, TComp, TGoal>(source, completion);
		}

		public static TResult[] Build<TSource, T, TResult>(
			this IEnumerable<Builder<TSource, T, TResult>?> builders,
			Func<TSource, T> getCompliment)
		{
			return builders.FilterNull()
				.Select(x => x.Build(getCompliment))
				.ToArray();
		}
	}

	class Builder<TSource, TComp, TGoal>
	{
		private readonly TSource _source;
		private readonly Func<TComp, TGoal> _completion;

		public Builder(TSource source, Func<TComp, TGoal> completion)
		{
			_source = source;
			_completion = completion;
		}

		public TGoal Build(Func<TSource, TComp> getCompliment)
		{
			return _completion(getCompliment(_source));
		}
	}
}
