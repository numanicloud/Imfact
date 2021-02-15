using System.Linq;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Aspects.Nodes
{
	internal record Property(PropertyDeclarationSyntax Syntax, IPropertySymbol Symbol)
	{
		public bool IsDelegation()
		{
			return Symbol.Type.GetAttributes()
				.Select(x => x.AttributeClass)
				.FilterNull()
				.Select(x => new AttributeName(x.Name))
				.Any(x => x.NameWithoutSuffix == "Factory");
		}

		public Method[] GetMethodToAnalyze()
		{
			return Symbol.Type.GetMembers().OfType<IMethodSymbol>()
				.Select(m =>
				{
					var mm = m.DeclaringSyntaxReferences
						.Select(x => x.GetSyntax())
						.OfType<MethodDeclarationSyntax>()
						.FirstOrDefault();
					return mm is { } syntax
						? new Method(syntax, m)
						: null;
				})
				.FilterNull()
				.ToArray();
		}
	}
}
