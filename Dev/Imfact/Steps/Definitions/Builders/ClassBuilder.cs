using System.Linq;
using Imfact.Annotations;
using Imfact.Entities;
using Imfact.Steps.Definitions.Methods;
using Imfact.Steps.Dependency;
using Imfact.Steps.Semanticses.Interfaces;
using Imfact.Steps.Semanticses.Records;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Definitions
{
	internal class ClassBuilder
	{
		private readonly SemanticsRoot _semantics;
		private readonly DependencyRoot _dependency;
		private readonly MethodBuilder _methodBuilder;

		public ClassBuilder(SemanticsRoot semantics, DependencyRoot dependency, MethodBuilder methodBuilder)
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
				_dependency.DisposableInfo);
		}

		private MethodInfo[] BuildMethods()
		{
			return _methodBuilder.ConstructorBuilder.BuildConstructorInfo().WrapByArray()
				.Concat(_methodBuilder.BuildRegisterServiceMethodInfo().WrapOrEmpty())
				.Concat(_methodBuilder.BuildResolverInfo())
				.Concat(_methodBuilder.BuildEnumerableMethodInfo())
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
