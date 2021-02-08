using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Utilities
{
	internal static class AnalyzerHelper
	{
		public static bool IsImplementing(this INamedTypeSymbol symbol, Type @interface)
		{
			var typeName = TypeName.FromType(@interface);
			return symbol.AllInterfaces
				.Any(x => x.Name == typeName.Name
				          && x.GetFullNameSpace() == typeName.FullNamespace);
		}

		public static bool IsPartial(this MethodDeclarationSyntax method)
		{
			// partial情報の取り方：
			// https://stackoverflow.com/questions/23026884/how-to-determine-that-a-partial-method-has-no-implementation
			// https://github.com/dotnet/roslyn/issues/48

			return method.Body != null
			       && method.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
		}

		public static bool HasAttribute(this INamedTypeSymbol symbol, string attributeName)
		{
			return symbol.GetAttributes()
				.Any(attr => attr.AttributeClass?.Name == attributeName);
		}
	}
}
