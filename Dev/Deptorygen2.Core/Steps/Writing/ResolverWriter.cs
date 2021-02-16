using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deptorygen2.Core.Steps.Creation;
using Deptorygen2.Core.Steps.Creation.Abstraction;
using Deptorygen2.Core.Steps.Definitions;
using NacHelpers.Extensions;
using static Deptorygen2.Core.Steps.Writing.SourceCodeBuilder;

namespace Deptorygen2.Core.Steps.Writing
{
	internal class ResolverWriter
	{
		public void RenderImplementation(Method method, ICreationAggregator creation, ICodeBuilder builder)
		{
			var given = method.Parameters
				.Select(x => new GivenParameter(x.Type.TypeName, x.Name))
				.ToArray();
			var request = new CreationRequest(method.ResolutionType, given, true);
			var expression = creation.GetInjection(request) ?? "__NoResolutionFound__";

			var writers = GetHookWriters(method.ReturnType.Text, method.Hooks, expression).ToArray();
			writers[0].Write(builder, writers.Skip(1).ToArray());
		}

		public void RenderImplementation(EnumMethod method, ICreationAggregator creation, ICodeBuilder builder)
		{
			var given = method.Parameters
				.Select(x => new GivenParameter(x.Type.TypeName, x.Name))
				.ToArray();
			var request = new MultipleCreationRequest(method.ResolutionTypes, given, true);

			var expBuilder = CodeHelper.GetBuilder();
			expBuilder.AppendLine($"new {method.ElementType.Text}[]");
			expBuilder.EnterBlock(inner =>
			{
				inner.AppendLine(creation.GetInjections(request).Join(",\n"));
			});

			var returnType = $"IEnumerable<{method.ElementType.Text}>";
			var writers = GetHookWriters(returnType, method.Hooks, expBuilder.GetText()).ToArray();
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
				builder.AppendLine($"result = {Hook.FieldName}.Before(context);");
				builder.AppendLine("if(result is null)");
				builder.EnterBlock(block =>
				{
					rest[0].Write(block, rest.Skip(1).ToArray());
				});
				builder.AppendLine($"result = {Hook.FieldName}.After(result, context);");
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
