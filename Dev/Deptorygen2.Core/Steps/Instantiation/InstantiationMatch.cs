using System;
using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Steps.Definitions;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Instantiation
{
	internal interface IInstantiationCoder
	{
		InstantiationMethod Method { get; }
		string? GetCode(InstantiationRequest request, IInstantiationResolver resolver);
	}

	internal record InstantiationMatch<T>(InstantiationMethod Method,
		IEnumerable<T> Source,
		Func<T, TypeName> TypeSelector,
		CodeGetter<T> CodeGetter)
		: IInstantiationCoder
	{

		public string? GetCode(InstantiationRequest request, IInstantiationResolver resolver)
		{
			return GetCode(request.TypeToResolve, request.GivenParameters, resolver);
		}

		public string? GetCode(TypeName type, ResolverParameterDefinition[] parameters, IInstantiationResolver resolver)
		{
			return Source.Where(x => TypeSelector(x) == type)
				.Select(x => CodeGetter(x, parameters, resolver))
				.FirstOrDefault();
		}
	}

	internal delegate string CodeGetter<in T>(T subject, ResolverParameterDefinition[] given, IInstantiationResolver resolver);
}