using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Semanticses;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Parser
{
	internal delegate FactorySemantics FactoryContentsLoader(FactoryFact fact);

	internal record FactoryFact(ClassDeclarationSyntax Syntax,
		TypeName Type,
		MethodDeclarationSyntax[] Methods,
		PropertyDeclarationSyntax[] Properties)
	{
	}

	internal class FactoryLoader
	{
		private readonly IAnalysisContext _context;

		public FactoryLoader(IAnalysisContext context)
		{
			_context = context;
		}

		public FactorySemantics? BuildFactorySyntaxAsync(
			ClassDeclarationSyntax classDeclarationSyntax,
			FactoryContentsLoader loadContents)
		{
			if (GetFact(classDeclarationSyntax) is not {} factory)
			{
				return null;
			}

			return loadContents.Invoke(factory);
		}

		private FactoryFact? GetFact(ClassDeclarationSyntax syntax)
		{
			var factoryAttribute = new AttributeName("FactoryAttribute");
			if (!syntax.AttributeLists.HasAttribute(factoryAttribute))
			{
				return null;
			}

			if (_context.GetNamedTypeSymbol(syntax) is not {} symbol)
			{
				return null;
			}

			var typeName = TypeName.FromSymbol(symbol);

			var methods = syntax.Members.OfType<MethodDeclarationSyntax>()
				.ToArray();

			var properties = syntax.Members.OfType<PropertyDeclarationSyntax>()
				.ToArray();

			return new FactoryFact(syntax, typeName, methods, properties);
		}
	}
}
