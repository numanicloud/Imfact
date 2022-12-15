using System.Diagnostics.CodeAnalysis;
using Imfact.Entities;
using Imfact.Steps.Filter;
using Imfact.Steps.Filter.Wrappers;
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
    private void EnsureInitialized()
    {
        if (_annotations is null) throw new Exception();
        if (_filterStep is null) throw new Exception();
    }

    private FactoryMock CreateGeneralFactory(string fullNamespace, string name)
    {
        EnsureInitialized();
        return new FactoryMock(fullNamespace, name)
        {
            AttributesMutable = new[]
            {
                new AnnotationMock(_annotations.FactoryAttribute, null, null)
            }
        };
    }

    [Test]
    public void FactoryAttributeのついたクラスは生成対象になる()
    {
        EnsureInitialized();

        var root = FilteredType.New(CreateGeneralFactory("Test", "Hoge"));

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .AssertThat(x => x.Symbol, Is.EqualTo(root.Symbol));
    }

    [Test]
    public void FactoryAttributeがついていないクラスは生成対象にならない()
    {
        EnsureInitialized();

        var root = FilteredType.New(
            new FactoryMock("Test", "Hoge"));

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .IsNull();
    }

    [Test]
    public void 基底クラスがファクトリーなら勘定される()
    {
        EnsureInitialized();

        var baseType = new FilteredBaseType(
            new BaseFactoryMock("Test", "HogeBase")
            {
                AttributesMutable = new[]
                {
                    new AnnotationMock(_annotations.FactoryAttribute, null, null)
                }
            },
            Array.Empty<FilteredMethod>(),
            null);

        var root = FilteredType.New(CreateGeneralFactory("Test", "Hoge"),
            baseFactory: baseType);

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .AssertThat(x => x.BaseFactory, Is.EqualTo(baseType));
    }

    [Test]
    public void 基底クラスがファクトリーでないなら除去される()
    {
        EnsureInitialized();

        var baseType = new FilteredBaseType(new BaseFactoryMock("Test", "HogeBase"),
            Array.Empty<FilteredMethod>(),
            null);

        var root = FilteredType.New(CreateGeneralFactory("Test", "Hoge"),
            baseFactory: baseType);

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .AssertThat(x => x.BaseFactory, Is.Null);
    }

    [Test]
    public void 基底クラスが2つともファクトリーならそれを通す()
    {
        EnsureInitialized();

        var factoryAnnotation = new[]
        {
            new AnnotationMock(_annotations.FactoryAttribute, null, null)
        };

        var base2 = new FilteredBaseType(new BaseFactoryMock("Test", "Base2")
        {
            AttributesMutable = factoryAnnotation
        },
            Array.Empty<FilteredMethod>(),
            null);

        var base1 = new FilteredBaseType(new BaseFactoryMock("Test", "Base1")
        {
            AttributesMutable = factoryAnnotation
        },
            Array.Empty<FilteredMethod>(),
            base2);

        var root = FilteredType.New(CreateGeneralFactory("Test", "Hoge"),
            baseFactory: base1);

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .OnObject(a => a.BaseFactory,
                a => a.NotNull()
                    .OnObject(b => b.BaseFactory,
                        b => b.NotNull()));
    }

    [Test]
    public void 最初の基底クラスがファクトリーでないなら以降も全て通さない()
    {
        EnsureInitialized();

        var base2 = new FilteredBaseType(new BaseFactoryMock("Test", "Base2")
        {
            AttributesMutable = new[]
                {
                    new AnnotationMock(_annotations.FactoryAttribute, null, null)
                }
        },
            Array.Empty<FilteredMethod>(),
            null);

        var base1 = new FilteredBaseType(new BaseFactoryMock("Test", "Base1"),
            Array.Empty<FilteredMethod>(),
            base2);

        var root = FilteredType.New(CreateGeneralFactory("Test", "Hoge"),
            baseFactory: base1);

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .OnObject(a => a.BaseFactory,
                a => a.IsNull());
    }

    [Test]
    public void クラスをインスタンス化するリゾルバーは通す()
    {
        EnsureInitialized();

        var resolver1 = new ResolverMock { IsIndirectResolverMutable = true };
        var resolver2 = new ResolverMock { IsIndirectResolverMutable = false };

        var root = FilteredType.New(CreateGeneralFactory("Test", "Hoge"),
            new FilteredMethod[]
            {
                new FilteredMethod(resolver1,
                    new ReturnTypeMock("Client", "Foo"),
                    Array.Empty<FilteredAttribute>()),
                new FilteredMethod(resolver2,
                    new ReturnTypeMock("System", "Int32"),
                    Array.Empty<FilteredAttribute>())
            });

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .OnSequence(a => a.Methods,
                a => a.OnObject(b => b.Symbol,
                    b => b.AssertType<ResolverMock>()));
    }

    [Test]
    public void Resolutionsのうちファクトリーを返すものだけを採用する()
    {
        EnsureInitialized();

        var factoryAnnotation = new[]
        {
            new AnnotationMock(_annotations.FactoryAttribute, null, null)
        };

        var root = FilteredType.New(CreateGeneralFactory("Test", "Hoge"),
            resolutions: new[]
            {
                new FilteredResolution(new ReturnTypeMock("Client", "Return")
                {
                    AttributesMutable = factoryAnnotation,
                    IsConstructableClass = true
                }),
                new FilteredResolution(new ReturnTypeMock("Client", "Return2")
                {
                    IsConstructableClass = true
                })
            });

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .OnSequence(a => a.ResolutionFactories,
                b => b.OnObject(c => c.Type,
                    c => c.AssertType<ReturnTypeMock>(
                        d => d.AssertThat(e => e.TypeAnalysisMutable.FullNamespace,
                                Is.EqualTo("Client"))
                            .AssertThat(e => e.TypeAnalysisMutable.Name,
                                Is.EqualTo("Return")))));
    }

    [Test]
    public void 生成に関係ない属性は捨てられる()
    {
        EnsureInitialized();

        var root = FilteredType.New(CreateGeneralFactory("Test", "Hoge"),
            new FilteredMethod[]
            {
                new(new ResolverMock()
                    {
                        IsIndirectResolverMutable = true
                    },
                    new ReturnTypeMock("Client", "Return"),
                    new FilteredAttribute[]
                    {
                        new(new AnnotationMock(_annotations.CacheAttribute, null, null),
                            AnnotationKind.CacheHookPreset),
                        new(new AnnotationMock(_annotations.CachePerResolutionAttribute,
                                null,
                                null),
                            AnnotationKind.CachePrHookPreset),
                        new(new AnnotationMock(new AttributeMock("Test", "Attr"), null, null),
                            AnnotationKind.Hook),
                        new(new AnnotationMock(_annotations.ResolutionAttribute,
                                new TypeMock("Client", "ResolutionHoge", new TypeId[0]),
                                null),
                            AnnotationKind.Resolution),
                        new(new AnnotationMock(_annotations.ResolutionAttributeT,
                                null,
                                new TypeMock("Client", "ResolutionHoge", new TypeId[0])),
                            AnnotationKind.Resolution),
                        new(new AnnotationMock(_annotations.HookAttribute,
                            null,
                            new TypeMock("Client", "HookHoge", new TypeId[0])),
                            AnnotationKind.Hook)
                    })
            });

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .OnSequence(a => a.Methods,
                b => b.OnSequence(c => c.Attributes,
                    c => AssertAnnotation(c, "CacheAttribute"),
                    c => AssertAnnotation(c, "CachePerResolutionAttribute"),
                    c => AssertAnnotation(c, "ResolutionAttribute"),
                    c => AssertAnnotation(c, "ResolutionAttribute"),
                    c => AssertAnnotation(c, "HookAttribute")));
    }

    [Test]
    public void 引数が指定されていないResolution属性は無視される()
    {
        EnsureInitialized();

        var root = FilteredType.New(CreateGeneralFactory("Test", "Hoge"),
            new FilteredMethod[]
            {
                new(new ResolverMock()
                    {
                        IsIndirectResolverMutable = true
                    },
                    new ReturnTypeMock("Client", "Resolved"),
                    new FilteredAttribute[]
                    {
                        new(new AnnotationMock(_annotations.ResolutionAttribute,
                                null,
                                null),
                            AnnotationKind.Resolution)
                    })
            });

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .OnSequence(a => a.Methods,
                a => a.OnSequence(b => b.Attributes));
    }

    [Test]
    public void 型引数が指定されていないResolution属性は無視される()
    {
        EnsureInitialized();

        var root = FilteredType.New(CreateGeneralFactory("Test", "Hoge"),
            new FilteredMethod[]
            {
                new(new ResolverMock
                    {
                        IsIndirectResolverMutable = true
                    },
                    new ReturnTypeMock("Client", "Resolved"),
                    new FilteredAttribute[]
                    {
                        new(new AnnotationMock(_annotations.ResolutionAttributeT,
                                null,
                                null),
                            AnnotationKind.Resolution)
                    })
            });

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .OnSequence(a => a.Methods,
                a => a.OnSequence(b => b.Attributes));
    }

    [Test]
    public void 型引数が指定されていないHook属性は無視される()
    {
        EnsureInitialized();

        var root = FilteredType.New(CreateGeneralFactory("Test", "Hoge"),
            new FilteredMethod[]
            {
                new(new ResolverMock()
                    {
                        IsIndirectResolverMutable = true,
                    },
                    new ReturnTypeMock("Client", "Resolved"),
                    new FilteredAttribute[]
                    {
                        new(new AnnotationMock(_annotations.HookAttribute,
                                null,
                                null),
                            AnnotationKind.Hook)
                    })
            });

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .OnSequence(a => a.Methods,
                a => a.OnSequence(b => b.Attributes));
    }
    
    static void AssertAnnotation(FluentAssertionContext<FilteredAttribute> context,
        string name)
    {
        context.OnObject(a => a.Wrapper,
            a => a.AssertType<AnnotationMock>(b =>
                b.AssertThat(c => c.Attribute.Name, Is.EqualTo(name))));
    }
}