using System;
using System.Text;
using Imfact.Steps.Definitions.Interfaces;

namespace Imfact.Steps.Writing.Coding
{
	internal static class CodeHelper
	{
		public static ICodeBuilder GetBuilder(StringBuilder? builder = null)
		{
			return new CodeBuilder(builder);
		}

		public static void EnterBlock(this ICodeBuilder baseBuilder, Action<ICodeBuilder> build, bool withTrailingSemicolon = false)
		{
			using var block = BlockBuilder.EnterBlock(baseBuilder, withTrailingSemicolon);
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

		public static void EnterCsv(this ICodeBuilder baseBuilder, Action<ICodeBuilder> build)
		{
			var csv = new CsvBuilder(baseBuilder);
			build.Invoke(csv);
		}
	}
}
