using System.Linq;
using Imfact.Annotations;
using Imfact.Entities;
using Imfact.Interfaces;
using Imfact.Steps.Definitions.Methods;
using Imfact.Steps.Dependency;
using Imfact.Steps.Semanticses;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Definitions
{
	internal class DefinitionStep
	{
		private readonly SemanticsRoot _semantics;
		private readonly DependencyRoot _dependency;
		private readonly MethodBuilder _methodBuilder;

		public DefinitionStep(DependencyRoot dependency)
		{
			_semantics = dependency.Semantics;
			_dependency = dependency;
			_methodBuilder = new MethodBuilder(dependency);
		}

		public DefinitionStepResult Build()
		{
			var usings = _dependency.Usings
				.Select(x => new Using(x))
				.ToArray();

			var nss = new Namespace(
				_semantics.Factory.Type.FullNamespace,
				BuildClass());

			var ctor = nss.Class.Methods.Select(x => x.Signature)
				.OfType<ConstructorSignature>()
				.First();

			return new DefinitionStepResult(new DefinitionRoot(usings, nss),
				BuildConstructorRecord(ctor));
		}

		private ConstructorRecord BuildConstructorRecord(ConstructorSignature signature)
		{
			var ps = signature.BaseParameters
				.WrapOrEmpty()
				.SelectMany(x => x)
				.Concat(signature.Parameters)
				.Select(x => new ParameterRecord(x.Type.TypeName, x.Name))
				.ToArray();

			return new ConstructorRecord(_semantics.Factory.Type, signature.Accessibility, ps);
		}

		private Class BuildClass()
		{
			return new Class(_semantics.Factory.Type.Name,
				BuildMethods(),
				BuildPropertyNodes(),
				BuildFieldNode(),
				_dependency.DisposableInfo);
		}

		private MethodInfo[] BuildMethods()
		{
			return _methodBuilder.BuildConstructorInfo().WrapByArray()
				//.Concat(_methodBuilder.BuildEntryMethodInfo())
				.Concat(_methodBuilder.BuildRegisterServiceMethodInfo().WrapOrEmpty())
				.Concat(_methodBuilder.BuildResolverInfo())
				.Concat(_methodBuilder.BuildEnumerableMethodInfo())
				.Concat(_methodBuilder.BuildDisposeMethodInfo())
				.ToArray();
		}

		private Property[] BuildPropertyNodes()
		{
			return _semantics.Factory.Delegations
				.Select(x => new Property(new Type(x.Type), x.PropertyName))
				.ToArray();
		}

		private Field[] BuildFieldNode()
		{
			var deps = _dependency.Injection.Dependencies
				.Select(x => (t: x.TypeName, f: x.FieldName));

			var hooks = _semantics.Factory.Resolvers
				.SelectMany(x => x.Hooks)
				.Select(x => (t: x.HookType, f: x.FieldName));

			var hooks2 = _semantics.Factory.MultiResolvers
				.SelectMany(x => x.Hooks)
				.Select(x => (t: x.HookType, f: x.FieldName));

			var fields = deps.Concat(hooks).Concat(hooks2)
				.Select(x => new Field(new Type(x.t), x.f, x.t.DisposableType));

			if (!_semantics.Factory.Inheritances.Any())
			{
				fields = fields.Append(new Field(
					new Type(TypeNode.FromRuntime(typeof(ResolverService))),
					"__resolverService", DisposableType.NonDisposable, false,
					Accessibility.ProtectedAndInternal));
			}

			return fields.ToArray();
		}
	}
}
