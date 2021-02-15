using System.Linq;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Aggregation
{
	internal record PropertyToAnalyze(PropertyDeclarationSyntax Syntax, IPropertySymbol Symbol)
	{
		public bool IsDelegation()
		{
			return Symbol.Type.GetAttributes()
				.Select(x => x.AttributeClass)
				.FilterNull()
				.Select(x => new AttributeName(x.Name))
				.Any(x => x.NameWithoutSuffix == "Factory");
		}

		public MethodToAnalyze[] GetMethodToAnalyze()
		{
			return Symbol.Type.GetMembers().OfType<IMethodSymbol>()
				.Select(m =>
				{
					var mm = m.DeclaringSyntaxReferences
						.Select(x => x.GetSyntax())
						.OfType<MethodDeclarationSyntax>()
						.FirstOrDefault();
					return mm is { } syntax
						? new MethodToAnalyze(syntax, m)
						: null;
				})
				.FilterNull()
				.ToArray();
		}
	}
}
