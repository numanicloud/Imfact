using System.Linq;
using Imfact.Entities;
using Imfact.Main;
using Imfact.Steps.Definitions.Builders;
using Imfact.Steps.Definitions.Methods;
using Imfact.Steps.Dependency;
using Imfact.Steps.Semanticses;
using Imfact.Utilities;

namespace Imfact.Steps.Definitions;

internal class DefinitionStep
{
	private readonly ClassBuilder _classBuilder;
	private readonly GenerationContext _genContext;
	private readonly SemanticsResult _semantics;
	private readonly DependencyResult _dependency;

	public DefinitionStep(DependencyResult dependency,
		ClassBuilder classBuilder,
		GenerationContext genContext)
	{
		_dependency = dependency;
		_semantics = dependency.Semantics;
		_classBuilder = classBuilder;
		_genContext = genContext;
	}

	public DefinitionResult Build()
	{
		using var profiler = _genContext.Profiler.GetScope();

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