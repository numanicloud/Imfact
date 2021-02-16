using System;
using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aspects.Nodes;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Attribute = Deptorygen2.Core.Steps.Aspects.Nodes.Attribute;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Resolution(TypeNode TypeName,
		TypeNode[] Dependencies,
		bool IsDisposable) : INamespaceClaimer
	{
		public static Resolution? Build(Attribute attribute,
			IAnalysisContext context)
		{
			if (!attribute.IsResolution())
			{
				return null;
			}

			var syntaxNode = attribute.Data.ApplicationSyntaxReference?.GetSyntax();
			if (syntaxNode is not AttributeSyntax syntax)
			{
				return null;
			}

			var argument1 = syntax.ArgumentList?.Arguments[0];

			if (argument1?.Expression is TypeOfExpressionSyntax toes
				&& context.GetTypeSymbol(toes.Type) is INamedTypeSymbol symbol)
			{
				return BuildInternal(toes.Type, symbol, context);
			}

			return null;
		}

		public static Resolution? Build(ReturnType returnType, IAnalysisContext context)
		{
			if (!returnType.IsResolution())
			{
				return null;
			}

			return BuildInternal(returnType.Syntax, returnType.Symbol, context);
		}

		private static Resolution? BuildInternal(TypeSyntax syntax,
			INamedTypeSymbol symbol,
			IAnalysisContext context)
		{
			var isDisposable = symbol.IsImplementing(typeof(IDisposable));

			var dependencies = symbol.Constructors.Single().Parameters
				.Select(x => TypeNode.FromSymbol(x.Type))
				.ToArray();

			return new Resolution(
				TypeNode.FromSymbol(symbol),
				dependencies,
				isDisposable);
		}

		public IEnumerable<ISemanticsNode> Traverse()
		{
			yield break;
		}

		public IEnumerable<string> GetRequiredNamespaces()
		{
			yield return TypeName.FullNamespace;
		}
	}
}
