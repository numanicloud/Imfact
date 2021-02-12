using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Deptorygen2.Core;
using Deptorygen2.Generator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2
{
	class SampleSourceCode
	{
		public static readonly string Code1 = @"
namespace HogeHoge
{
	class PikaPika
	{
		public int Prop { get; }

		public void Run()
		{
			DoAction(4);
		}
	}
}
";

		public static readonly string Factory1 = @"
using Pika;

namespace Fuwa
{
	public class Client
	{
		public Client(Service service)
		{
		}
	}

	[Factory]
	partial class MyFactory
	{
		internal partial Client ResolveClient();
	}
}

namespace Pika
{
	public class Service
	{
	}
}
";
	}

	class Analyzer
	{
		public IEnumerable<ClassDeclarationSyntax> FindClasses(string code)
		{
			var tree = CSharpSyntaxTree.ParseText(code);
			return tree.GetRoot().ChildNodes()
				.OfType<NamespaceDeclarationSyntax>()
				.SelectMany(ns => ns.ChildNodes()
					.OfType<ClassDeclarationSyntax>());
		}

		public void Analyze(string code)
		{
			var tree = CSharpSyntaxTree.ParseText(code);
			var root = tree.GetCompilationUnitRoot();

			var compilation = CSharpCompilation.Create("Test")
				.AddReferences(MetadataReference.CreateFromFile(
					typeof(string).Assembly.Location))
				.AddSyntaxTrees(tree);

			var semanticModel = compilation.GetSemanticModel(tree);
			var analysisContext = new CompilationAnalysisContext(semanticModel);

			var generation = new GenerationFacade(analysisContext);

			var classes = from ns in tree.GetRoot().ChildNodes().OfType<NamespaceDeclarationSyntax>()
					from type in ns.ChildNodes().OfType<ClassDeclarationSyntax>()
					select type;

			foreach (var classDeclarationSyntax in classes)
			{
				var newSource = generation.RunGeneration(classDeclarationSyntax);
				Console.WriteLine(newSource?.Contents);
				Console.WriteLine();
			}
		}
	}
}
