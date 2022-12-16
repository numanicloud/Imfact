using System.Diagnostics.CodeAnalysis;
using Imfact.Entities;
using Imfact.Steps.Filter;
using Imfact.Steps.Filter.Wrappers;
using Imfact.Test.Suite;
using Imfact.Test.SymbolMock;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Test;

public class FilterStepTest
{
    private AnnotationContext? _annotations;
    private FilterStep? _filterStep;
    private IAnnotationWrapper[]? _factoryAnnotationSet;

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

        _factoryAnnotationSet = new IAnnotationWrapper[]
        {
            new AnnotationMock(_annotations.FactoryAttribute, null, null)
        };

        TypeMock SingleUnboundType(string fullNamespace, string name)
        {
            return new TypeMock(fullNamespace, name, new[]
            {
                new TypeId("", "T", RecordArray<TypeId>.Empty)
            });
        }
    }

    [MemberNotNull(nameof(_annotations), nameof(_filterStep), nameof(_factoryAnnotationSet))]
    private void EnsureInitialized()
    {
        if (_annotations is null) throw new Exception();
        if (_filterStep is null) throw new Exception();
        if (_factoryAnnotationSet is null) throw new Exception();
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

        var root = CreateGeneralFactory("Test", "Hoge");

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .AssertThat(x => x.Type.FullNamespace, Is.EqualTo("Test"))
            .AssertThat(x => x.Type.Name, Is.EqualTo("Hoge"));
    }

    [Test]
    public void FactoryAttributeがついていないクラスは生成対象にならない()
    {
        EnsureInitialized();

        var root = new FactoryMock("Test", "Hoge");

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .IsNull();
    }

    [Test]
    public void 基底クラスがファクトリーなら勘定される()
    {
        EnsureInitialized();

        var baseType = new BaseFactoryMock("Test", "HogeBase")
        {
            AttributesMutable = _factoryAnnotationSet
        };

        var root = new FactoryMock("Test", "Hoge")
        {
            AttributesMutable = _factoryAnnotationSet,
            BaseType = baseType
        };

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .Do(x => Assert.That(x.BaseFactory?.Type.FullNamespace, Is.EqualTo("Test")))
            .Do(x => Assert.That(x.BaseFactory?.Type.Name, Is.EqualTo("HogeBase")));
    }

    [Test]
    public void 基底クラスがファクトリーでないなら除去される()
    {
        EnsureInitialized();

        var baseType = new BaseFactoryMock("Test", "HogeBase");

        var root = new FactoryMock("Test", "Hoge")
        {
            AttributesMutable = _factoryAnnotationSet,
            BaseType = baseType
        };

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .AssertThat(x => x.BaseFactory, Is.Null);
    }

    [Test]
    public void 基底クラスが2つともファクトリーならそれを通す()
    {
        EnsureInitialized();

        var base2 = new BaseFactoryMock("Test", "Base2")
        {
            AttributesMutable = _factoryAnnotationSet
        };

        var base1 = new BaseFactoryMock("Test", "Base1")
        {
            AttributesMutable = _factoryAnnotationSet,
            BaseType = base2
        };

        var root = new FactoryMock("Test", "Hoge")
        {
            AttributesMutable = _factoryAnnotationSet,
            BaseType = base1
        };

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

        var base2 = new BaseFactoryMock("Test", "Base2")
        {
            AttributesMutable = _factoryAnnotationSet
        };

        var base1 = new BaseFactoryMock("Test", "Base1")
        {
            BaseType = base2
        };

        var root = new FactoryMock("Test", "Hoge")
        {
            AttributesMutable = _factoryAnnotationSet,
            BaseType = base1
        };

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

        var root = new FactoryMock("Test", "Hoge")
        {
            AttributesMutable = _factoryAnnotationSet,
            Methods = new IResolverWrapper[]
            {
                new ResolverMock("Resolver1")
                {
                    IsIndirectResolverMutable = true,
                    ReturnType = new ReturnTypeMock("Client", "Foo")
                },
                new ResolverMock("Resolver2")
                {
                    IsIndirectResolverMutable = false,
                    ReturnType = new ReturnTypeMock("System", "Int32")
                }
            }
        };

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .OnSequence(a => a.Methods,
                a => Assert.Pass());
    }

    [Test]
    public void Resolutionsのうちファクトリーを返すものだけを採用する()
    {
        EnsureInitialized();
        
        var root = new FactoryMock("Test", "Hoge")
        {
            AttributesMutable = _factoryAnnotationSet,
            Resolutions = new IResolutionFactoryWrapper[]
            {
                new ResolutionMock("Client", "Return")
                {
                    AttributeMutable = _factoryAnnotationSet
                },
                new ResolutionMock("Client", "Return2")
                {
                    AttributeMutable = Array.Empty<IAnnotationWrapper>()
                }
            }
        };

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .OnSequence(a => a.ResolutionFactories,
                a => a.OnObject(b => b.Type,
                    b => b.AssertThat(c => c.FullNamespace, Is.EqualTo("Client"))
                        .AssertThat(c => c.Name, Is.EqualTo("Return"))));
    }

    [Test]
    public void 生成に関係ない属性は捨てられる()
    {
        EnsureInitialized();

        var root = new FactoryMock("Test", "Hoge")
        {
            AttributesMutable = _factoryAnnotationSet,
            Methods = new IResolverWrapper[]
            {
                new ResolverMock("Resolve")
                {
                    IsIndirectResolverMutable = true,
                    ReturnType = new ReturnTypeMock("Client", "Return"),
                    Annotations = new IAnnotationWrapper[]
                    {
                        new AnnotationMock(_annotations.CacheAttribute, null, null),
                        new AnnotationMock(_annotations.CachePerResolutionAttribute, null, null),
                        new AnnotationMock(new AttributeMock("Test", "Dummy"), null, null),
                        new AnnotationMock(_annotations.ResolutionAttribute,
                            new TypeMock("Client", "ResolutionHoge", new TypeId[0]),
                            null),
                        new AnnotationMock(_annotations.ResolutionAttributeT,
                            null,
                            new TypeMock("Client", "ResolutionHoge", new TypeId[0])),
                        new AnnotationMock(_annotations.HookAttribute,
                            null,
                            new TypeMock("Client", "HookHoge", new TypeId[0]))
                    }
                }
            }
        };

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
        
        var root = new FactoryMock("Test", "Hoge")
        {
            AttributesMutable = _factoryAnnotationSet,
            Methods = new IResolverWrapper[]
            {
                new ResolverMock("Resolve")
                {
                    IsIndirectResolverMutable = true,
                    ReturnType = new ReturnTypeMock("Client", "Resolved"),
                    Annotations = new IAnnotationWrapper[]
                    {
                        new AnnotationMock(_annotations.ResolutionAttribute, null, null)
                    }
                }
            }
        };

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
        
        var root = new FactoryMock("Test", "Hoge")
        {
            AttributesMutable = _factoryAnnotationSet,
            Methods = new IResolverWrapper[]
            {
                new ResolverMock("Resolve")
                {
                    IsIndirectResolverMutable = true,
                    ReturnType = new ReturnTypeMock("Client", "Resolved"),
                    Annotations = new IAnnotationWrapper[]
                    {
                        new AnnotationMock(_annotations.ResolutionAttributeT, null, null)
                    }
                }
            }
        };

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
        
        var root = new FactoryMock("Test", "Hoge")
        {
            AttributesMutable = _factoryAnnotationSet,
            Methods = new IResolverWrapper[]
            {
                new ResolverMock("Resolve")
                {
                    IsIndirectResolverMutable = true,
                    ReturnType = new ReturnTypeMock("Client", "Resolved"),
                    Annotations = new []
                    {
                        new AnnotationMock(_annotations.HookAttribute, null, null)
                    }
                }
            }
        };

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .OnSequence(a => a.Methods,
                a => a.OnSequence(b => b.Attributes));
    }

    [Test]
    public void 基底ファクトリーのリゾルバーのうちアクセシビリティにより見えないものは通さない()
    {
        EnsureInitialized();
        
        var root = new FactoryMock("Test", "Hoge")
        {
            AttributesMutable = _factoryAnnotationSet,
            BaseType = new BaseFactoryMock("Test", "Fuga")
            {
                AttributesMutable = _factoryAnnotationSet,
                Methods = new IResolverWrapper[]
                {
                    CreateMethodWithAccessibility(Accessibility.Private, "Private")
                }
            }
        };

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .OnObject(a => a.BaseFactory,
                a => a.NotNull()
                    .OnSequence(b => b.Methods));
    }

    [Test]
    public void 基底ファクトリーのリゾルバーのうちアクセシビリティにより見えるものは通す()
    {
        EnsureInitialized();

        var root = new FactoryMock("Test", "Hoge")
        {
            AttributesMutable = _factoryAnnotationSet,
            BaseType = new BaseFactoryMock("Test", "Fuga")
            {
                AttributesMutable = _factoryAnnotationSet,
                Methods = new IResolverWrapper[]
                {
                    CreateMethodWithAccessibility(Accessibility.Public, "M1"),
                    CreateMethodWithAccessibility(Accessibility.Protected, "M2"),
                    CreateMethodWithAccessibility(Accessibility.Internal, "M4"),
                    CreateMethodWithAccessibility(Accessibility.ProtectedOrInternal, "M8"),
                    CreateMethodWithAccessibility(Accessibility.ProtectedAndInternal, "M19")
                }
            }
        };

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .OnObject(a => a.BaseFactory,
                a => a.NotNull()
                    .OnSequence(b => b.Methods,
                        b => AssertMethodName(b, "M1"),
                        b => AssertMethodName(b, "M2"),
                        b => AssertMethodName(b, "M4"),
                        b => AssertMethodName(b, "M8"),
                        b => AssertMethodName(b, "M19")));
    }

    [Test]
    public void Factory属性のついていない委譲は取り除く()
    {
        EnsureInitialized();
        
        var root = new FactoryMock("Test", "Hoge")
        {
            AttributesMutable = _factoryAnnotationSet,
            Delegations = new IDelegationFactoryWrapper[]
            {
                new DelegationMock("Client", "Delegation1")
                {
                    AttributesMutable = Array.Empty<IAnnotationWrapper>()
                },
                new DelegationMock("Client", "Delegation2")
                {
                    AttributesMutable = _factoryAnnotationSet
                }
            }
        };

        var actual = _filterStep.Match((root, _annotations), CancellationToken.None);

        FluentAssertion.OnObject(actual)
            .NotNull()
            .OnSequence(a => a.Delegations,
                a => a.AssertThat(b => b.Type.FullNamespace, Is.EqualTo("Client"))
                    .AssertThat(b => b.Type.Name, Is.EqualTo("Delegation2")));
    }

    [Test]
    public void 委譲ファクトリーのリゾルバーのうちアクセシビリティにより見えないものは消す()
    {
        EnsureInitialized();

        var root = new FactoryMock("Test", "Hoge")
        {
            AttributesMutable = _factoryAnnotationSet,
            Delegations = new IDelegationFactoryWrapper[]
            {
                new DelegationMock("Client", "Delegation")
                {
                    AttributesMutable = _factoryAnnotationSet
                }
            }
        };
        
        // Delegationにメソッド情報を持たせる機構がまだない
        Assert.Fail();
    }

    private static IResolverWrapper CreateMethodWithAccessibility(Accessibility accessibility, string name)
    {
        return new ResolverMock(name)
        {
            IsIndirectResolverMutable = true,
            Accessibility = accessibility,
            ReturnType = new ReturnTypeMock("Client", "Resolution")
        };
    }

    private static void AssertMethodName(FluentAssertionContext<FilteredMethod> context, string name)
    {
        Assert.That(context.Context.Name, Is.EqualTo(name));
    }
    
    private static void AssertAnnotation(FluentAssertionContext<FilteredAttribute> context,
        string name)
    {
        context.AssertThat(a => a.Type.Name, Is.EqualTo(name));
    }
}

internal class DelegationMock : IDelegationFactoryWrapper
{
    public bool IsConstructableClass { get; set; } = true;
    public IEnumerable<IAnnotationWrapper> AttributesMutable { get; set; }
    public TypeAnalysis TypeAnalysisMutable { get; set; }

    public DelegationMock(string fullNamespace, string name)
    {
        AttributesMutable = Array.Empty<IAnnotationWrapper>();
        TypeAnalysisMutable = new TypeAnalysis(
            new TypeId(fullNamespace, name, RecordArray<TypeId>.Empty),
            Accessibility.Public,
            DisposableType.NonDisposable);
    }

    public IEnumerable<IAnnotationWrapper> GetAttributes() => AttributesMutable;

    public TypeAnalysis GetTypeAnalysis() => TypeAnalysisMutable;
}

internal class ResolutionMock : IResolutionFactoryWrapper
{
    public bool IsConstructableClass { get; } = true;

    public IEnumerable<IAnnotationWrapper> AttributeMutable { get; set; } =
        Array.Empty<IAnnotationWrapper>();

    public TypeAnalysis AnalysisMutable { get; set; }

    public ResolutionMock(string fullNamespace, string name)
    {
        AnalysisMutable = new TypeAnalysis(
            new TypeId(fullNamespace, name, RecordArray<TypeId>.Empty),
            Accessibility.Public,
            DisposableType.NonDisposable);
    }

    public IEnumerable<IAnnotationWrapper> GetAttributes() => AttributeMutable;

    public TypeAnalysis GetTypeAnalysis() => AnalysisMutable;
}