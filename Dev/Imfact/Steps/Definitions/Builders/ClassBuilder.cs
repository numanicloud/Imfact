using System.Linq;
using Imfact.Annotations;
using Imfact.Entities;
using Imfact.Steps.Definitions.Methods;
using Imfact.Steps.Dependency;
using Imfact.Steps.Semanticses;
using Imfact.Steps.Semanticses.Interfaces;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Definitions.Builders
{
    internal class ClassBuilder
	{
		private readonly SemanticsResult _semantics;
		private readonly DependencyResult _dependency;
		private readonly MethodBuilder _methodBuilder;

		public ClassBuilder(SemanticsResult semantics, DependencyResult dependency, MethodBuilder methodBuilder)
		{
			_semantics = semantics;
			_dependency = dependency;
			_methodBuilder = methodBuilder;
		}

		public Class BuildClass()
		{
			return new Class(_semantics.Factory.Type.Name,
				BuildMethods(),
				BuildFieldNode(),
				_dependency.DisposableInfo,
				BuildExporters(),
				_semantics.Factory.Inheritances.Any());
		}

		private ExporterItem[] BuildExporters()
		{
			using var profiler = TimeProfiler.Create("Extract-Exporter-Definitions");
			return _dependency.Resolvers
				.Where(x => !x.Parameters.Any())
				.Select(x =>
					new ExporterItem(x.ReturnType, x.ActualResolution.TypeName, x.MethodName))
				.ToArray();
		}

		private MethodInfo[] BuildMethods()
		{
			var registerServiceMethod = _methodBuilder.BuildRegisterServiceMethodInfo(
				_semantics.Factory.Inheritances,
				_semantics.Factory.Delegations);

			return _methodBuilder.ConstructorBuilder.BuildConstructorInfo().WrapByArray()
				.Concat(registerServiceMethod.WrapOrEmpty())
				.Concat(_methodBuilder.BuildResolverInfo(_semantics.Factory.Resolvers))
				.Concat(_methodBuilder.BuildEnumerableMethodInfo(_semantics.Factory.MultiResolvers))
				.Concat(_methodBuilder.DisposeMethodBuilder.BuildDisposeMethodInfo())
				.ToArray();
		}

		private Field[] BuildFieldNode()
		{
			var deps = _dependency.Injection.Dependencies;
			var hook1 = _semantics.Factory.Resolvers.SelectMany(x => x.Hooks);
			var hook2 = _semantics.Factory.MultiResolvers.SelectMany(x => x.Hooks);

			var fields = deps.Concat<IVariableSemantics>(hook1).Concat(hook2)
				.Select(x => new Field(x.Type, x.MemberName, x.Type.DisposableType));

			if (!_semantics.Factory.Inheritances.Any())
			{
				fields = fields.Append(new Field(
					TypeAnalysis.FromRuntime(typeof(ResolverService)),
					"__resolverService", DisposableType.NonDisposable, false,
					Accessibility.ProtectedAndInternal));
			}

			return fields.ToArray();
		}
	}
}
