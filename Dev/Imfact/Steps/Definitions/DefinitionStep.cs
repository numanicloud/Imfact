using System.Linq;
using Imfact.Interfaces;
using Imfact.Steps.Definitions.Methods;
using Imfact.Steps.Dependency;
using Imfact.Steps.Semanticses;
using Imfact.Steps.Semanticses.Records;
using Imfact.Utilities;

namespace Imfact.Steps.Definitions
{
	internal class DefinitionStep
	{
		private readonly ClassBuilder _classBuilder;
		private readonly SemanticsRoot _semantics;
		private readonly DependencyRoot _dependency;

		public DefinitionStep(DependencyRoot dependency)
		{
			_dependency = dependency;
			_semantics = dependency.Semantics;
			_classBuilder = new ClassBuilder(dependency.Semantics, dependency,
				new MethodBuilder(dependency,
					new MethodService(dependency)));
		}

		public DefinitionStepResult Build()
		{
			var usings = _dependency.Usings
				.Select(x => new Using(x))
				.ToArray();

			var nss = new Namespace(
				_semantics.Factory.Type.FullNamespace, _classBuilder.BuildClass());

			var ctor = nss.Class.Methods.Select(x => x.Signature)
				.OfType<ConstructorSignature>()
				.First();

			return new DefinitionStepResult(new DefinitionRoot(usings, nss),
				BuildConstructorRecord(ctor));
		}

		private ConstructorRecord BuildConstructorRecord(ConstructorSignature signature)
		{
			var ps = signature.BaseParameters
				.WrapOrEmpty()
				.SelectMany(x => x)
				.Concat(signature.Parameters)
				.Select(x => new ParameterRecord(x.TypeAnalysis, x.Name))
				.ToArray();

			return new ConstructorRecord(_semantics.Factory.Type, signature.Accessibility, ps);
		}
	}
}
