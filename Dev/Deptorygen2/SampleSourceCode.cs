using System.Collections.Generic;
using System.Linq;
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
			var cds = tree.GetRoot().ChildNodes()
				.OfType<NamespaceDeclarationSyntax>()
				.FirstOrDefault()
				?.ChildNodes()
				.OfType<ClassDeclarationSyntax>()
				.FirstOrDefault(x => x.Identifier.ValueText == "PikaPika");
		}
	}
}
