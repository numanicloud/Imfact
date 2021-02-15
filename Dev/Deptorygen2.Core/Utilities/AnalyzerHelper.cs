using System;
using System.Collections.Generic;
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
				.Any(x => x.Name == typeName.NameWithoutArguments
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

		public static bool HasAttribute(this IEnumerable<AttributeListSyntax> attributes, AttributeName attributeName)
		{
			return attributes.SelectMany(a => a.Attributes)
				.Any(a => attributeName.MatchWithAnyName(a.Name.ToString()));
		}

		public static bool IsCollectionType(this TypeName type)
		{
			var enumerableType = TypeName.FromType(typeof(IEnumerable<>));

			return type.NameWithoutArguments != enumerableType.NameWithoutArguments
			       && type.TypeArguments.Length != enumerableType.TypeArguments.Length;
		}
	}
}
