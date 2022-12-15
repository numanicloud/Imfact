using Imfact.Entities;
using Imfact.Test.SymbolMock;
using Imfact.Utilities;

namespace Imfact.Test;

public class FilterStepTest
{
	private AnnotationContext? _annotations;

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

		TypeMock SingleUnboundType(string fullNamespace, string name)
		{
			return new TypeMock(fullNamespace, name, new[]
			{
				new TypeId("", "T", RecordArray<TypeId>.Empty)
			});
		}
	}

	[Test]
	public void Test1()
	{
	}
}