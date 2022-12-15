using Imfact.Entities;
using Imfact.Steps.Filter.Wrappers;
using Imfact.Test.SymbolMock;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Test;

internal class FactoryMock : IFactoryClassWrapper
{
	private readonly AnnotationMock _annotation;
	private readonly TypeAnalysis _analysis;

	public IEnumerable<ITypeWrapper> AllInterfaces => Array.Empty<ITypeWrapper>();

	public FactoryMock(string fullNamespace, string name, IAttributeWrapper attribute)
	{
		_annotation = new AnnotationMock(attribute, null, null);
		_analysis = new TypeAnalysis(new TypeId(fullNamespace, name, RecordArray<TypeId>.Empty),
			Accessibility.Public,
			DisposableType.NonDisposable);
	}

	public IEnumerable<IAnnotationWrapper> GetAttributes()
	{
		yield return _annotation;
	}

	public TypeAnalysis GetTypeAnalysis() => _analysis;
}
