using System;
using System.Collections.Generic;
using System.Text;

namespace Deptorygen2.Core.Steps.Writing
{
	internal static class CodeHelper
	{
		public static ICodeBuilder GetBuilder(StringBuilder? builder = null)
		{
			return new CodeBuilder(builder);
		}

		public static void EnterBlock(this ICodeBuilder baseBuilder, Action<ICodeBuilder> build)
		{
			using var block = BlockBuilder.EnterBlock(baseBuilder);
			build.Invoke(block);
		}

		public static void EnterSequence(this ICodeBuilder baseBuilder, Action<ICodeBuilder> build)
		{
			var sequence = new SequenceBuilder(baseBuilder);
			build.Invoke(sequence);
		}

		public static void EnterChunk(this ICodeBuilder baseBuilder, Action<ICodeBuilder> build)
		{
			using var chunk = new ChunkBuilder(baseBuilder);
			build.Invoke(chunk);
		}

		public static void EnterSequenceChunk<T>(this ICodeBuilder baseBuilder, IEnumerable<T> source, Action<T, ICodeBuilder> build)
		{
			baseBuilder.EnterChunk(chunk => chunk.EnterSequence(inner =>
			{
				foreach (var item in source)
				{
					build.Invoke(item, inner);
				}
			}));
		}
	}
}
