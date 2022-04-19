using System.Linq;
using System.Text;
using Imfact.Entities;
using Imfact.Steps.Semanticses.Interfaces;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Semanticses.Records
{
	internal record Resolver(ResolverCommon Common, Resolution? ReturnTypeResolution)
		: ResolverCommon(Common)
	{
		public Resolution ActualResolution = Common.Resolutions
			.Append(ReturnTypeResolution)
			.FilterNull()
			.First();

		protected override bool PrintMembers(StringBuilder builder)
		{
			builder.Append($"{Accessibility} {ReturnType.Name} {MethodName}({Parameters.Select(x => x.ParameterName).Join(", ")})");
			return true;
		}
	}

	internal record MultiResolver(ResolverCommon Common)
		: ResolverCommon(Common)
	{
		public TypeAnalysis ElementType => ReturnType.TypeArguments[0];
	}

	internal record ResolverCommon(
		Accessibility Accessibility,
		TypeAnalysis ReturnType,
		string MethodName,
		Parameter[] Parameters,
		Resolution[] Resolutions,
		Hook[] Hooks) : IResolverSemantics
	{
		public override int GetHashCode()
		{
			var value = 1;
			value += (int)Accessibility * 3;
			value += ReturnType.Id.GetHashCode() * 7;
			value += MethodName.GetHashCode() * 19;
			value += Parameters.Length * 11;
			value += Resolutions.Length * 23;
			value += Hooks.Length * 2;
			return value;
		}
	}

	internal record Resolution(TypeAnalysis TypeName,
		TypeAnalysis[] Dependencies,
		DisposableType DisposableType);

	internal record Exporter(string MethodName, Parameter[] Parameters);
}
