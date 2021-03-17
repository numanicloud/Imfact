using System;
using System.Text;
using Imfact.Entities;
using Imfact.Interfaces;

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

		public static string GetCode(this TypeAnalysis type)
		{
			return $"{type.FullNamespace}.{type.Name}" switch
			{
				nameof(Byte) => "byte",
				nameof(Int16) => "short",
				nameof(Int32) => "int",
				nameof(Int64) => "long",
				nameof(SByte) => "sbyte",
				nameof(UInt16) => "ushort",
				nameof(UInt32) => "uint",
				nameof(UInt64) => "ulong",
				nameof(Single) => "float",
				nameof(Double) => "double",
				nameof(Decimal) => "decimal",
				nameof(Char) => "char",
				nameof(String) => "string",
				"System.Void" => "void",
				_ => type.FullBoundName
			};
		}
	}
}
