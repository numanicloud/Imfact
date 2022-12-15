using System.Diagnostics.CodeAnalysis;
using Imfact.Entities;
using Imfact.Steps.Filter;
using Imfact.Test.Suite;
using Imfact.Test.SymbolMock;
using Imfact.Utilities;

namespace Imfact.Test;

public class FilterStepTest
{
	private AnnotationContext? _annotations;
	private FilterStep? _filterStep;

	[SetUp]
	public void Setup()
	{
		var ns = "Imfact.Annotations";
		_annotations = new AnnotationContext()
		{
			FactoryAttribute = new AttributeMock(ns, "FactoryAttribute"),
			ResolutionAttribute = new AttributeMock(ns, "ResolutionAttribute"),
			ResolutionAttributeT = new AttributeMock(ns, "ResolutionAttribute"),
			HookAttribute = new AttributeMock(ns, "HookAttribute"),
			CacheAttribute = new AttributeMock(ns, "CacheAttribute"),
			CachePerResolutionAttribute = new AttributeMock(ns, "CachePerResolutionAttribute"),
			TransientAttribute = new AttributeMock(ns, "TransientAttribute"),
			ExporterAttribute = new AttributeMock(ns, "ExporterAttribute"),
			Cache = SingleUnboundType(ns, "Cache"),
			CachePerResolution = SingleUnboundType(ns, "CachePerResolution")
		};

		_filterStep = new FilterStep
		{
			ClassRule = new Imfact.Steps.Filter.Rules.ClassRule
			{
				MethodRule = new Imfact.Steps.Filter.Rules.MethodRule()
			}
		};

		TypeMock SingleUnboundType(string fullNamespace, string name)
		{
			return new TypeMock(fullNamespace, name, new[]
			{
				new TypeId("", "T", RecordArray<TypeId>.Empty)
			});
		}
	}

	[MemberNotNull(nameof(_annotations), nameof(_filterStep))]
	private void CheckNull()
	{
		if (_annotations is null) throw new Exception();
		if (_filterStep is null) throw new Exception();
	}

	[Test]
	public void FactoryAttributeのついたクラスは生成対象になる()
	{
		CheckNull();

		var root = new FilteredType(
			new FactoryMock("Test", "Hoge", _annotations.FactoryAttribute),
			Array.Empty<FilteredMethod>(),
			null,
			Array.Empty<FilteredResolution>(),
			Array.Empty<FilteredDelegation>());

		var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

		FluentAssertion.OnObject(actual)
			.NotNull()
			.AssertThat(x => x.Symbol, Is.EqualTo(root.Symbol));
	}

	[Test]
	public void FactoryAttributeがついていないクラスは生成対象にならない()
	{
		CheckNull();
		
		var root = new FilteredType(
			new FactoryMock("Test", "Hoge", _annotations.TransientAttribute),
			Array.Empty<FilteredMethod>(),
			null,
			Array.Empty<FilteredResolution>(),
			Array.Empty<FilteredDelegation>());

		var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

		FluentAssertion.OnObject(actual)
			.IsNull();
	}
}