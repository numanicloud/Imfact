using Imfact.Steps.Writing;
using Imfact.Steps.Writing.Coding;

namespace Imfact.Steps.Definitions.Methods
{
	internal record DisposeImplementation(string[] MemberNames, bool IsAsync) : Implementation
	{
		public override void Render(ICodeBuilder builder, ResolverWriter writer)
		{
			var op = IsAsync ? "await " : "";
			var method = IsAsync ? "DisposeAsync" : "Dispose";

			foreach (var name in MemberNames)
			{
				builder.AppendLine(op + $"{name}.{method}();");
			}
		}
	}
}
