using System;
using System.Collections.Generic;
using System.Text;
using NacHelpers.Extensions;

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

	internal interface ICodeBuilder
	{
		void Append(string text);
		void AppendLine(string text = "");
		string GetText();
	}

	internal class CodeBuilder : ICodeBuilder
	{
		private readonly StringBuilder _builder = new();

		public CodeBuilder(StringBuilder? builder = null)
		{
			if (builder is not null)
			{
				_builder = builder;
			}
		}

		public void Append(string text)
		{
			_builder.Append(text);
		}

		public void AppendLine(string text)
		{
			_builder.AppendLine(text);
		}

		public string GetText()
		{
			return _builder.ToString();
		}
	}

	internal class BlockBuilder : ICodeBuilder, IDisposable
	{
		private readonly ICodeBuilder _codeBuilder;
		private bool _isOnLineStart = true;

		private BlockBuilder(ICodeBuilder codeBuilder)
		{
			_codeBuilder = codeBuilder;
		}

		public static BlockBuilder EnterBlock(ICodeBuilder codeBuilder)
		{
			var instance = new BlockBuilder(codeBuilder);
			codeBuilder.AppendLine("{");
			return instance;
		}

		public void Append(string text)
		{
			_codeBuilder.Append(_isOnLineStart ? text.Indent(1) : text);
			_isOnLineStart = false;
		}

		public void AppendLine(string text)
		{
			_codeBuilder.AppendLine(_isOnLineStart ? text.Indent(1) : text);
			_isOnLineStart = true;
		}

		public string GetText() => _codeBuilder.GetText();

		public void Dispose()
		{
			_codeBuilder.AppendLine("}");
		}
	}

	internal class SequenceBuilder : ICodeBuilder
	{
		private readonly ICodeBuilder _codeBuilder;
		private bool _isFirstLine = true;
		private bool _isLineStart = true;

		public SequenceBuilder(ICodeBuilder codeBuilder)
		{
			_codeBuilder = codeBuilder;
		}

		public void Append(string text)
		{
			if (!_isFirstLine && _isLineStart)
			{
				_codeBuilder.AppendLine();
			}

			_codeBuilder.Append(text);
			_isLineStart = false;
		}

		public void AppendLine(string text)
		{
			if (!_isFirstLine && _isLineStart)
			{
				_codeBuilder.AppendLine();
			}

			_codeBuilder.AppendLine(text);
			_isFirstLine = false;
			_isLineStart = true;
		}

		public string GetText() => _codeBuilder.GetText();
	}

	internal class ChunkBuilder : ICodeBuilder, IDisposable
	{
		private readonly ICodeBuilder _baseBuilder;
		private readonly StringBuilder _stringBuilder = new ();

		public ChunkBuilder(ICodeBuilder baseBuilder)
		{
			_baseBuilder = baseBuilder;
		}

		public void Append(string text) => _stringBuilder.Append(text);

		public void AppendLine(string text) => _stringBuilder.AppendLine(text);

		public string GetText() => _stringBuilder.ToString();

		public void Dispose()
		{
			var text = _stringBuilder.ToString().TrimEnd();
			if (text != string.Empty)
			{
				_baseBuilder.AppendLine(text);
			}
		}
	}
}
