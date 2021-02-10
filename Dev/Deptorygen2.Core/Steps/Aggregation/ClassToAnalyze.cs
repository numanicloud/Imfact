using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Aggregation
{
	internal record ClassToAnalyze(ClassDeclarationSyntax Syntax, INamedTypeSymbol Symbol)
	{
		public bool IsFactory()
		{
			return Syntax.AttributeLists.HasAttribute(new AttributeName("FactoryAttribute"));
		}

		public MethodToAnalyze[] GetMethods(IAnalysisContext context)
		{
			return Syntax.Members
				.OfType<MethodDeclarationSyntax>()
				.Select(m => context.GetMethodSymbol(m) is { } symbol
					? new MethodToAnalyze(m, symbol)
					: null)
				.FilterNull()
				.ToArray();
		}

		public PropertyToAnalyze[] GetProperties(IAnalysisContext context)
		{
			return Syntax.Members
				.OfType<PropertyDeclarationSyntax>()
				.Select(m => context.GetPropertySymbol(m) is { } symbol
					? new PropertyToAnalyze(m, symbol)
					: null)
				.FilterNull()
				.ToArray();
		}
	}
}
