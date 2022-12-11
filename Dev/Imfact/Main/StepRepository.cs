using Imfact.Steps.Aspects;
using Imfact.Steps.Aspects.Rules;
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
		Filter = new FilterStep();
		Cacheability = new CacheabilityStep();

		var typeRule = new TypeRule();
		var methodRule = new MethodRule()
		{
			AttributeRule = new AttributeRule(typeRule, new AnnotationContext()),
			TypeRule = typeRule
		};
		Aspect = new AspectStep
		{
			ClassRule = new ClassRule
			{
				MethodRule = methodRule,
				PropertyRule = new PropertyRule
				{
					Rule = methodRule
				}
			}
		};
	}
}