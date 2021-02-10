using System;
using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Definitions
{
	internal class ArgumentResolver
	{
		private readonly InjectionMatch<FactorySemantics> _factoryMatch;
		private readonly InjectionMatch<DelegationSemantics> _delegationMatch;
		private readonly InjectionMatch<(DelegationSemantics, ResolverSemantics)> _delegationResolverMatch;
		private readonly InjectionMatch<(DelegationSemantics, CollectionResolverSemantics)> _delegationCollectionMatch;
		private readonly InjectionMatch<ResolverSemantics> _resolverMatch;
		private readonly InjectionMatch<CollectionResolverSemantics> _collectionResolverMatch;
		private readonly InjectionMatch<DependencyDefinition> _fieldMatch;

		public ArgumentResolver(FactorySemantics factory, DependencyDefinition[] fields)
		{
			_factoryMatch = Create(factory.AsEnumerable(),
				f => TypeName.FromSymbol(factory.ItselfSymbol),
				(f, ps) => "this");

			_delegationMatch = Create(factory.Delegations,
				delegation => delegation.TypeName,
				(delegation, ps) => delegation.PropertyName);
			
			_delegationResolverMatch = Create(Dig(d => d.Resolvers),
				t => t.resolver.ReturnTypeName,
				(t, ps) => $"{t.delegation.PropertyName}.{GetMethodInvocation(t.resolver, ps)}");

			_delegationCollectionMatch = Create(Dig(d => d.CollectionResolvers),
				t => t.resolver.CollectionType,
				(t, ps) => $"{t.delegation.PropertyName}.{GetMethodInvocation(t.resolver, ps)}");

			_resolverMatch = Create(factory.Resolvers,
				resolver => resolver.ReturnTypeName,
				(resolver, ps) => GetMethodInvocation(resolver, ps));

			_collectionResolverMatch = Create(factory.CollectionResolvers,
				resolver => resolver.CollectionType,
				(resolver, ps) => GetMethodInvocation(resolver, ps));

			_fieldMatch = Create(fields,
				field => field.FieldType,
				(field, ps) => field.FieldName);

			IEnumerable<(DelegationSemantics delegation, T resolver)> Dig<T>(
				Func<DelegationSemantics, IEnumerable<T>> selector)
			{
				return factory.Delegations.SelectMany(delegation =>
					selector(delegation).Select(inner => (delegation, inner)));
			}

			string GetMethodInvocation(IResolverSemantics resolver, ResolverParameterDefinition[] given)
			{
				var argTypes = resolver.Parameters.Select(x => x.TypeName).ToArray();
				var argList = GetArgumentCodes(argTypes, given).Join(", ");
				return $"{resolver.MethodName}({argList})";
			}
		}

		public string[] GetArgumentCodes(TypeName[] argumentTypes, ResolverParameterDefinition[] parameters)
		{
			var table = parameters.GroupBy(x => x.Type)
				.ToDictionary(x => x.Key, x => x.ToList());
			return argumentTypes.Select(t =>
			{
				if (table.GetValueOrDefault(t) is { } onType)
				{
					var result = onType[0].Name;
					onType.RemoveAt(0);
					return result;
				}
				else
				{
					return GetInjection(t, parameters) ?? "<Error>";
				}
			}).ToArray();
		}

		public string? GetInjection(TypeName typeName, ResolverParameterDefinition[] parameters)
		{
			string? Try<T>(InjectionMatch<T> injection)
			{
				return injection.GetCode(typeName, parameters);
			}

			return Try(_factoryMatch) ?? Try(_delegationMatch)
				?? Try(_delegationResolverMatch) ?? Try(_delegationCollectionMatch)
				?? Try(_resolverMatch) ?? Try(_collectionResolverMatch)
				?? Try(_fieldMatch);
		}

		private static InjectionMatch<T> Create<T>(IEnumerable<T> source,
			Func<T, TypeName> typeSelector,
			Func<T, ResolverParameterDefinition[], string> codeGetter)
		{
			return new(source, typeSelector, codeGetter);
		}

		private record InjectionMatch<T>(IEnumerable<T> Source, Func<T, TypeName> TypeSelector,
			Func<T, ResolverParameterDefinition[], string> CodeGetter)
		{
			public string? GetCode(TypeName type, ResolverParameterDefinition[] parameters)
			{
				return Source.Where(x => TypeSelector(x) == type)
					.Select(x => CodeGetter(x, parameters))
					.FirstOrDefault();
			}
		}
	}
}