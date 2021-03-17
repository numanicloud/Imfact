using Imfact.Steps.Definitions.Methods;
using Imfact.Steps.Writing.Coding;

namespace Imfact.Steps.Writing
{
	internal sealed class ImplWriter
	{
		private readonly ResolverWriter _writer;

		public ImplWriter(ResolverWriter writer)
		{
			_writer = writer;
		}

		public void Render(Implementation impl, ICodeBuilder builder)
		{
			switch (impl)
			{
				case ConstructorImplementation constructor:
					RenderConstructor(constructor, builder);
					break;

				case DisposeImplementation dispose:
					RenderDispose(dispose, builder);
					break;

				case ExpressionImplementation exp:
					RenderExp(exp, builder);
					break;

				case MultiExpImplementation multiExp:
					RenderMultiExp(multiExp, builder);
					break;
			}
		}

		private void RenderDispose(DisposeImplementation impl, ICodeBuilder builder)
		{
			var op = impl.IsAsync ? "await " : "";
			var method = impl.IsAsync ? "DisposeAsync" : "Dispose";

			foreach (var name in impl.MemberNames)
			{
				builder.AppendLine(op + $"{name}.{method}();");
			}
		}

		private void RenderExp(ExpressionImplementation impl, ICodeBuilder builder)
		{
			_writer.Render(impl.ReturnType.GetCode(), impl.Hooks, impl.Expression, builder);
		}

		private void RenderMultiExp(MultiExpImplementation impl, ICodeBuilder builder)
		{
			var expBuilder = CodeHelper.GetBuilder();
			expBuilder.AppendLine($"new {impl.ElementType.GetCode()}[]");
			expBuilder.EnterBlock(inner =>
			{
				inner.EnterCsv(csv =>
				{
					foreach (var expression in impl.Expressions)
					{
						csv.AppendLine(expression);
					}
				});
			});

			var returnType = $"IEnumerable<{impl.ElementType.GetCode()}>";
			_writer.Render(returnType, impl.Hooks, expBuilder.GetText(), builder);
		}

		private void RenderConstructor(ConstructorImplementation impl, ICodeBuilder builder)
		{
			builder.EnterSequence(inner =>
			{
				inner.EnterChunk(chunk =>
				{
					foreach (var item in impl.Initializations)
					{
						chunk.AppendLine($"{item.Name} = {item.ParamName};");
					}
				});

				inner.EnterChunk(chunk =>
				{
					foreach (var hook in impl.Hooks)
					{
						var typeName = hook.TypeAnalysis.FullBoundName;
						chunk.AppendLine($"{hook.FieldName} = new {typeName}();");
					}
				});

				inner.EnterChunk(chunk =>
				{
					chunk.AppendLine("__resolverService = new ResolverService();");
					chunk.AppendLine("this.RegisterService(__resolverService);");
				});
			});
		}
	}
}

