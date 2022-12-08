using System.Linq;
using Imfact.Entities;
using Imfact.Steps.Definitions.Builders;
using Imfact.Steps.Definitions.Methods;
using Imfact.Steps.Dependency;
using Imfact.Steps.Semanticses;
using Imfact.Utilities;

namespace Imfact.Steps.Definitions;

internal class DefinitionStep
{
	private readonly ClassBuilder _classBuilder;
	private readonly SemanticsResult _semantics;
	private readonly DependencyResult _dependency;

	public DefinitionStep(DependencyResult dependency, ClassBuilder classBuilder)
	{
		_dependency = dependency;
		_semantics = dependency.Semantics;
		_classBuilder = classBuilder;
	}

	public DefinitionStep(DependencyResult dependency)
	{
		_dependency = dependency;
		_semantics = dependency.Semantics;


		var methodService = new MethodService(dependency);
		_classBuilder = new ClassBuilder(dependency.Semantics, dependency,
			new MethodBuilder(
				methodService,
				dependency.Injection,
				new DisposeMethodBuilder(dependency),
				new ConstructorBuilder(dependency, methodService)));
	}

	public DefinitionResult Build()
	{
		var usings = _dependency.Usings
			.Select(x => new Using(x))
			.ToArray();

		var nss = new Namespace(
			_semantics.Factory.Type.FullNamespace, _classBuilder.BuildClass());

		var ctor = nss.Class.Methods.Select(x => x.Signature)
			.OfType<ConstructorSignature>()
			.First();

		return new DefinitionResult(new DefinitionRoot(usings, nss),
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