using Imfact.Steps.Aspects;
using Imfact.Steps.Cacheability;
using Imfact.Steps.Filter;

namespace Imfact.Main;

internal class StepRepository
{
    private static StepRepository? _instance;
    public static StepRepository Instance => _instance ??= new StepRepository();

	public FilterStep Filter { get; }
	public CacheabilityStep Cacheability { get; }
	public AspectStep Aspect { get; }

	private StepRepository()
	{
		// FilterStep
		Filter = new FilterStep
		{
			ClassRule = new Steps.Filter.Rules.ClassRule
			{
				MethodRule = new Steps.Filter.Rules.MethodRule()
			}
		};

		// CacheabilityStep
		Cacheability = new CacheabilityStep();

		// AspectStep
		var typeRule = new Steps.Aspects.Rules.TypeRule();
		var methodRule = new Steps.Aspects.Rules.MethodRule()
		{
			AttributeRule = new Steps.Aspects.Rules.AttributeRule(typeRule),
			TypeRule = typeRule
		};
		Aspect = new AspectStep
		{
			ClassRule = new Steps.Aspects.Rules.ClassRule
			{
				MethodRule = methodRule,
				PropertyRule = new Steps.Aspects.Rules.PropertyRule
				{
					Rule = methodRule
				}
			},
			ClassRuleAlt = new Steps.Aspects.Rules.ClassRuleAlt()
			{
				MethodRule = methodRule,
				PropertyRule = new Steps.Aspects.Rules.PropertyRule()
				{
					Rule = methodRule
				}
			}
		};
	}
}