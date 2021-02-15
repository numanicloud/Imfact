using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deptorygen2.Core.Steps.Creation;
using Deptorygen2.Core.Steps.Definitions.Syntaxes;
using static Deptorygen2.Core.Steps.Writing.SourceCodeBuilder;

namespace Deptorygen2.Core.Steps.Writing
{
	internal class ResolverWriter
	{
		public void RenderImplementation(MethodNode method, ICreationAggregator creation, StringBuilder builder)
		{
			var writers = GetHookWriters(method, creation).ToArray();
			writers[0].Write(builder, writers.Skip(1).ToArray());
		}

		private IEnumerable<IHookWriter> GetHookWriters(MethodNode method, ICreationAggregator creation)
		{
			yield return new RootWriter(method);
			foreach (var hook in method.Hooks)
			{
				yield return new HookWriter(hook);
			}

			{
				var given = method.Parameters
					.Select(x => new GivenParameter(x.Type.TypeName, x.Name))
					.ToArray();
				var request = new CreationRequest(method.ResolutionType, given, true);
				var expression = creation.GetInjection(request);

				yield return new CoreWriter(expression ?? "__NoResolutionFound__");
			}
		}

		private interface IHookWriter
		{
			void Write(StringBuilder builder, IHookWriter[] rest);
		}

		private record RootWriter(MethodNode Method) : IHookWriter
		{
			public void Write(StringBuilder builder, IHookWriter[] rest)
			{
				builder.AppendLine($"{Method.ResolutionType.Name}? result;");
				builder.AppendLine();

				rest[0].Write(builder, rest.Skip(1).ToArray());

				builder.AppendLine();
				builder.AppendLine("return result;");
			}
		}

		private record HookWriter(HookNode Hook) : IHookWriter
		{
			public void Write(StringBuilder builder, IHookWriter[] rest)
			{
				builder.AppendLine($"result = {Hook.FieldName}.Before(context);");
				AppendBlock(builder, "if(result is null)", inner =>
				{
					rest[0].Write(inner, rest.Skip(1).ToArray());
				});
				builder.AppendLine($"result = {Hook.FieldName}.After(result, context);");
			}
		}

		private record CoreWriter(string Expression) : IHookWriter
		{
			public void Write(StringBuilder builder, IHookWriter[] rest)
			{
				builder.AppendLine($"result = {Expression};");
			}
		}
	}
}
