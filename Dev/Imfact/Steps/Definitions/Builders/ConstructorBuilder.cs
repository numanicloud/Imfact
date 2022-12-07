using System.Linq;
using Imfact.Steps.Definitions.Methods;
using Imfact.Steps.Definitions.Methods.Implementations;
using Imfact.Steps.Dependency;
using Imfact.Utilities;

namespace Imfact.Steps.Definitions.Builders;

internal sealed class ConstructorBuilder
{
	private readonly DependencyResult _dependency;
	private readonly MethodService _service;

	public ConstructorBuilder(DependencyResult dependency, MethodService service)
	{
		_dependency = dependency;
		_service = service;
	}

	public MethodInfo BuildConstructorInfo()
	{
		var fs = _dependency.Dependencies
			.Select(x => new Initialization(x.TypeName, x.FieldName, x.FieldName))
			.Select(x => x with { ParamName = x.ParamName.TrimStart("_".ToCharArray()) })
			.ToArray();

		var ps = _dependency.Delegations
			.Where(x => x.NeedsInitialize)
			.Select(x => new Initialization(x.Type, x.PropertyName, x.PropertyName))
			.Select(x => x with { ParamName = x.ParamName.ToLowerCamelCase() })
			.ToArray();

		var signature = GetCtorSignature(fs, ps);
		var impl = GetCtorImpl(fs.Concat(ps).ToArray());
		return new MethodInfo(signature, impl);
	}

	private ConstructorSignature GetCtorSignature(Initialization[] fs, Initialization[] ps)
	{
		var parameters = fs.Concat(ps)
			.Select(x => _service.BuildParameter(x.Type, x.ParamName))
			.OrderBy(x => x.Name)
			.ToArray();

		var baseParameters = _dependency.Inheritances
			.FirstOrDefault()?.Parameters
			.Select(x => _service.BuildParameter(x.Type, x.ParameterName))
			.ToArray();

		var accessibilities = parameters.Concat(baseParameters ?? new Parameter[0])
			.Select(x => x.TypeAnalysis.Accessibility)
			.ToArray();
		var a = AnalyzerHelper.GetTypeAccessibilityMostStrict(accessibilities);

		return new ConstructorSignature(a, _dependency.Factory.Type.Name,
			parameters,
			baseParameters);
	}

	private Implementation GetCtorImpl(Initialization[] asignee)
	{
		return new ConstructorImplementation(asignee, _service.ExtractHooks());
	}
}