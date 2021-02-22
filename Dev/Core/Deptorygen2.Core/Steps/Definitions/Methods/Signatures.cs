using Microsoft.CodeAnalysis;
using NacHelpers.Extensions;
using System.Linq;

namespace Deptorygen2.Core.Steps.Definitions.Methods
{
	internal record OrdinalSignature(Accessibility Accessibility, Type ReturnType,
		string Name, Parameter[] Parameters, string[] Modifiers) : Signature
	{
		public override string GetSignatureString()
		{
			var access = Accessibility.ToString().ToLower();
			var ret = ReturnType.Text;
			var parameter = GetParameterList(Parameters);

			var mod = Modifiers.Any() ? Modifiers.Join(" ") : "";

			return $"{access} {mod} {ret} {Name}({parameter})";
		}
	}

	internal record ConstructorSignature(Accessibility Accessibility,
		string Name,
		Parameter[] Parameters,
		Parameter[]? BaseParameters) : Signature
	{
		public override string GetSignatureString()
		{
			var ps = Parameters;
			if (BaseParameters is not null)
			{
				ps = ps.Concat(BaseParameters).ToArray();
			}

			var access = Accessibility.ToString().ToLower();
			var parameterList = GetParameterList(ps);

			if (BaseParameters is not null)
			{
				var baseArgList = BaseParameters.Select(x => x.Name).Join(", ");
				return $"{access} {Name}({parameterList}) : base({baseArgList})";
			}
			else
			{
				return $"{access} {Name}({parameterList})";
			}
		}
	}
}
