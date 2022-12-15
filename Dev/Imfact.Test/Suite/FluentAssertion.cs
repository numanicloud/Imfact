using NUnit.Framework.Constraints;

namespace Imfact.Test.Suite;

internal class FluentAssertion
{
	public static FluentAssertionContext<T> OnObject<T>(T context)
	{
		return new FluentAssertionContext<T> { Context = context };
	}
}

internal class FluentAssertionContext<T>
{
    public required T Context { get; init; }

    public FluentAssertionContext<T> AssertThat<TActual>(
		Func<T, TActual> selector,
		Constraint constraint)
	{
        Assert.That(selector(Context), constraint);
		return this;
	}


    public FluentAssertionContext<T> OnObject<TNext>(
        Func<T, TNext> selector,
        Action<FluentAssertionContext<TNext>> assertion)
    {
        assertion(new FluentAssertionContext<TNext> { Context = selector(Context) });
        return this;
    }
}