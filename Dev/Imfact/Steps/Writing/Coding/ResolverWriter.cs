using System.Collections.Generic;
using System.Linq;
using Imfact.Interfaces;
using Imfact.Steps.Definitions;
using Imfact.Steps.Definitions.Interfaces;
using Imfact.Steps.Writing.Coding;

namespace Imfact.Steps.Writing
{
	internal class ResolverWriter : IResolverWriter
	{
		public void Render(string returnTypeName, Hook[] hooks, string expression, ICodeBuilder builder)
		{
			var writers = GetHookWriters(returnTypeName, hooks, expression).ToArray();
			writers[0].Write(builder, writers.Skip(1).ToArray());
		}

		private IEnumerable<IHookWriter> GetHookWriters(string returnTypeName, Hook[] hooks, string expression)
		{
			yield return new RootWriter(returnTypeName);
			foreach (var hook in hooks)
			{
				yield return new HookWriter(hook);
			}
			yield return new CoreWriter(expression);
		}

		private interface IHookWriter
		{
			void Write(ICodeBuilder builder, IHookWriter[] rest);
		}

		private record RootWriter(string ReturnTypeName) : IHookWriter
		{
			public void Write(ICodeBuilder builder, IHookWriter[] rest)
			{
				builder.AppendLine("using var scope = __resolverService.Enter();");
				builder.AppendLine($"{ReturnTypeName}? result;");
				builder.AppendLine();

				rest[0].Write(builder, rest.Skip(1).ToArray());

				builder.AppendLine();
				builder.AppendLine("return result;");
			}
		}

		private record HookWriter(Hook Hook) : IHookWriter
		{
			public void Write(ICodeBuilder builder, IHookWriter[] rest)
			{
				builder.AppendLine($"result = {Hook.FieldName}.Before();");
				builder.AppendLine("if(result is null)");
				builder.EnterBlock(block =>
				{
					rest[0].Write(block, rest.Skip(1).ToArray());
				});
				builder.AppendLine($"result = {Hook.FieldName}.After(result);");
			}
		}

		private record CoreWriter(string Expression) : IHookWriter
		{
			public void Write(ICodeBuilder builder, IHookWriter[] rest)
			{
				builder.AppendLine($"result = {Expression};");
			}
		}
	}
}
