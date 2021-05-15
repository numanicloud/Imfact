using Imfact.Coding.Builders;
using System;

var result = ICodeBuilder.OnPlainBuilder(builder =>
{
	builder.EnterLineSpaced(spaced =>
	{
		spaced.EnterChunk(chunk =>
		{
			chunk.AppendLine("Hello, World!");
			chunk.AppendLine("HogeHoge");
		});

		spaced.EnterChunk(chunk =>
		{
			chunk.AppendLine("Human");
			chunk.AppendLine("Perfect");
		});

		spaced.EnterChunk(chunk =>
		{
			chunk.EnterBlock(block =>
			{
				block.AppendLine("homura");
				block.AppendLine("saikyo");
			});
		});

		spaced.EnterBlock(block =>
		{
			block.AppendLine("persona");
			block.AppendLine("super-init");
		});
	});
	builder.AppendLine("QED");
});

Console.WriteLine(result);