using System;
using System.Text;
using Imfact.Steps.Definitions.Interfaces;

namespace Imfact.Steps.Writing.Coding;

internal class FluentCodeBuilder : IFluentCodeBuilder
{
	private readonly ICodeBuilder _impl;

	public FluentCodeBuilder(ICodeBuilder impl)
	{
		_impl = impl;
	}

	public void Append(string text) => _impl.Append(text);

	public void AppendLine(string text = "") => _impl.AppendLine(text);

	public string GetText() => _impl.GetText();

	public string OnPlainBuilder(Action<IFluentCodeBuilder> build)
	{
		var code = new FluentCodeBuilder(new CodeBuilder(new StringBuilder()));
		build.Invoke(code);
		return code.GetText();
	}

	public void EnterBlock(Action<IFluentCodeBuilder> build, bool withTrailingSemicolon = false)
	{
		_impl.EnterBlock(
			block => build(new FluentCodeBuilder(block)),
			withTrailingSemicolon);
	}

	public void EnterSequence(Action<IFluentCodeBuilder> build)
	{
		_impl.EnterSequence(sequence => build(new FluentCodeBuilder(sequence)));
	}

	public void EnterChunk(Action<IFluentCodeBuilder> build)
	{
		_impl.EnterChunk(chunk => build(new FluentCodeBuilder(chunk)));
	}

	public void EnterCsv(Action<IFluentCodeBuilder> build)
	{
		_impl.EnterCsv(csv => build(new FluentCodeBuilder(csv)));
	}
}