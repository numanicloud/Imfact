﻿using Imfact.Interfaces;

namespace Imfact.Steps.Definitions.Methods
{
	internal record DisposeImplementation(string[] MemberNames, bool IsAsync) : Implementation
	{
		public override void Render(ICodeBuilder builder, IResolverWriter writer)
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
