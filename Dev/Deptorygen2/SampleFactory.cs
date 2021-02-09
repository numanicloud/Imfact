using System;
using Deptorygen2.Core;
using Deptorygen2.Core.Definitions;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2
{
	class SampleFactory
	{
		public FactoryDefinition Create()
		{
			TypeName Make(string name, string @namespace = "MyDomain")
			{
				return new TypeName(@namespace, name, Accessibility.Internal);
			}

			return new FactoryDefinition("MyFactory",
				new ResolverDefinition[]
				{
					new(Accessibility.Public, "Hoge", Make("Service"),
						new ResolutionDefinition(Make("Service"), new string[0]),
						new ResolverParameterDefinition[0],
						new HookDefinition[]
						{
							new(Make("LogHook")),
							new(Make("CacheHook")),
						}),
					new(Accessibility.Private, "Fuga", Make("Client"),
						new ResolutionDefinition(Make("Client"), new[]{ "hoge", "_service" }),
						new ResolverParameterDefinition[]
						{
							new(Make("Int32", "System"), "hoge")
						},
						new HookDefinition[0]),
				},
				new CollectionResolverDefinition[0],
				new DelegationDefinition[0],
				new DependencyDefinition[]
				{
					new(Make("Other"), "_other")
				});
		}
	}

	public interface IHook<T>
	{
		T Hook(Func<T> generation);
	}

	public class ResolutionCache<T> : IHook<T> where T : class
	{
		private T? _cache = null;

		public T Hook(Func<T> generation)
		{
			return _cache ??= generation();
		}
	}

	public interface IHookAttribute
	{
		Type HookType { get; }
	}

	public class CacheHookAttribute : Attribute, IHookAttribute
	{
		public Type HookType => typeof(ResolutionCache<>);
	}
}

namespace MyDomain
{
	public class Service
	{

	}

	public class Client
	{
		public Client(Service service, int hoge)
		{
		}
	}
}