using System.Linq;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Definitions.Methods;
using Deptorygen2.Core.Steps.Expressions;
using Deptorygen2.Core.Steps.Semanticses;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Definitions
{
	internal class DefinitionTreeBuilder
	{
		private readonly SemanticsRoot _semantics;
		private readonly ResolutionRoot _resolution;
		private readonly MethodBuilder _methodBuilder;

		public DefinitionTreeBuilder(ResolutionRoot resolution)
		{
			_semantics = resolution.Semantics;
			_resolution = resolution;
			_methodBuilder = new MethodBuilder(resolution);
		}

		public SourceTreeDefinition Build()
		{
			var usings = _resolution.Usings
				.Select(x => new Using(x))
				.ToArray();

			var nss = new Namespace(
				_semantics.Factory.Type.FullNamespace,
				BuildClass());

			var ctor = nss.Class.Methods.Select(x => x.Signature)
				.OfType<ConstructorSignature>()
				.First();

			return new SourceTreeDefinition(new DefinitionRoot(usings, nss),
				BuildConstructorRecord(ctor));
		}

		private ConstructorRecord BuildConstructorRecord(ConstructorSignature signature)
		{
			var ps = signature.BaseParameters
				.AsEnumerable()
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
				_resolution.DisposableInfo);
		}

		private MethodInfo[] BuildMethods()
		{
			return _methodBuilder.BuildConstructorInfo().WrapByArray()
				.Concat(_methodBuilder.BuildEntryMethodInfo())
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
			var deps = _resolution.Injection.Dependencies
				.Select(x => (t: x.TypeName, f: x.FieldName));

			var hooks = _semantics.Factory.Resolvers
				.SelectMany(x => x.Hooks)
				.Select(x => (t: x.HookType, f: x.FieldName));

			var hooks2 = _semantics.Factory.MultiResolvers
				.SelectMany(x => x.Hooks)
				.Select(x => (t: x.HookType, f: x.FieldName));

			return deps.Concat(hooks).Concat(hooks2)
				.Select(x => new Field(new Type(x.t), x.f, x.t.DisposableType))
				.ToArray();
		}
	}
}
