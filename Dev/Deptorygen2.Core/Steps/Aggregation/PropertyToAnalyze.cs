using System.Linq;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Aggregation
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
	}
}
