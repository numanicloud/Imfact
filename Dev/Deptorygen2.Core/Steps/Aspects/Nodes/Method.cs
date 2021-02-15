using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Steps.Aspects.Nodes
{
	internal record Method(MethodDeclarationSyntax Syntax, IMethodSymbol Symbol)
	{
		public bool IsSingleResolver()
		{
			return !TypeName.FromSymbol(Symbol.ReturnType).IsCollectionType();
		}

		public bool IsCollectionResolver()
		{
			return TypeName.FromSymbol(Symbol.ReturnType).IsCollectionType();
		}

		public ReturnType? GetReturnType(IAnalysisContext context)
		{
			if (context.GetTypeSymbol(Syntax.ReturnType) is INamedTypeSymbol symbol)
			{
				return new(Syntax.ReturnType, symbol);
			}

			return null;
		}

		public Attribute[] GetAttributes()
		{
			return Symbol.GetAttributes()
				.Select(x => new Attribute(x))
				.ToArray();
		}

		public Parameter[] GetParameters()
		{
			return Syntax.ParameterList.Parameters
				.Select(x => new Parameter(x))
				.ToArray();
		}
	}
}
