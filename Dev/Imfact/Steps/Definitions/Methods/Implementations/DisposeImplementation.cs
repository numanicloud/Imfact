using Imfact.Steps.Definitions.Interfaces;
using Imfact.Utilities.Coding;

namespace Imfact.Steps.Definitions.Methods.Implementations;

internal record DisposeImplementation(string[] MemberNames, bool IsAsync) : Implementation
{
	public override void Render(IFluentCodeBuilder builder, IResolverWriter writer)
	{
		var op = IsAsync ? "await " : "";
		var method = IsAsync ? "DisposeAsync" : "Dispose";

		foreach (var name in MemberNames)
		{
			builder.AppendLine(op + $"{name}.{method}();");
		}
	}
}