using System.Linq;
using Deptorygen2.Core.Steps.Writing;
using Deptorygen2.Core.Steps.Writing.Coding;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Definitions.Methods
{
	internal record MethodInfo(Signature Signature, Attribute[] Attributes, Implementation Implementation);

	internal abstract record Signature
	{
		protected string GetParameterList(Parameter[] parameters)
		{
			return parameters.Select(x =>
				{
					var nullable = x.IsNullable ? "?" : "";
					var defaultValue = x.IsNullable ? " = null" : "";
					return $"{x.Type.Text}{nullable} {x.Name}{defaultValue}";
				})
				.Join(", ");
		}

		public abstract string GetSignatureString();
	}

	internal record Attribute(string Text);

	internal abstract record Implementation
	{
		public abstract void Render(ICodeBuilder builder, ResolverWriter writer);
	}
}
