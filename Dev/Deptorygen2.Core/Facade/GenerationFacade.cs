using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deptorygen2.Core.Definitions;
using Deptorygen2.Core.Parser;
using Deptorygen2.Core.Semanticses;
using Deptorygen2.Core.SyntaxBuilding;
using Deptorygen2.Core.Writing;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Facade
{
	public class GenerationFacade
	{
		private SourceLoader _loader = new();
		private SourceSyntaxBuilder _syntaxBuilder = new();
		private SourceCodeBuilder _writer = new();

		public async Task<DeptorygenSemantics> ConvertToSemanticsAsync(ClassDeclarationSyntax classDeclaration)
		{
			// TODO: Analyzerとは違って、Document などの情報が無い。Syntaxだけでやりくりする必要があるかも
			var semantics = await _loader.LoadAsync(classDeclaration, null);
			return new DeptorygenSemantics(semantics);
		}

		public DeptorygenDefinition ConvertToDefinition(DeptorygenSemantics semantics)
		{
			var definition = _syntaxBuilder.Encode(semantics.Semantics);
			return new DeptorygenDefinition(definition);
		}

		public SourceFile ConvertToSourceCode(DeptorygenDefinition definition)
		{
			return _writer.Build(definition.Definition);
		}
	}

	public class DeptorygenSemantics
	{
		internal FactorySemantics Semantics { get; }

		internal DeptorygenSemantics(FactorySemantics semantics)
		{
			Semantics = semantics;
		}
	}

	public class DeptorygenDefinition
	{
		internal SourceCodeDefinition Definition { get; }

		internal DeptorygenDefinition(SourceCodeDefinition definition)
		{
			Definition = definition;
		}
	}
}
