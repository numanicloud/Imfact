using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Deptorygen2.Generator
{
	class AnnotationGenerator
	{
		private static readonly string FactoryAttributeCode = @"
using System;

namespace Deptorygen
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	sealed class FactoryAttribute : Attribute
	{
	}
}
";

		private static readonly string ResolutionAttributeCode = @"
using System;

namespace Deptorygen
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	sealed class ResolutionAttribute : Attribute
	{
		public Type Type { get; }

		public ResolutionAttribute(Type type)
		{
			Type = type;
		}
	}
}
";

		public static void AddSource(in GeneratorExecutionContext context)
		{
			var code1 = SourceText.From(FactoryAttributeCode, Encoding.UTF8);
			context.AddSource("FactoryAttribute.g", code1);

			var code2 = SourceText.From(ResolutionAttributeCode, Encoding.UTF8);
			context.AddSource("ResolutionAttribute.g", code2);
		}
	}
}
