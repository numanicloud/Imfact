using System.Linq;
using Deptorygen2.Annotations;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Creation;
using Deptorygen2.Core.Steps.Definitions.Methods;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Definitions
{
	internal class DefinitionTreeBuilder
	{
		private readonly Generation _semantics;
		private readonly MethodBuilder _methodBuilder;

		public DefinitionTreeBuilder(Generation semantics)
		{
			_semantics = semantics; 
			_methodBuilder = new MethodBuilder(_semantics);
		}

		public SourceTreeDefinition Build()
		{
			var usings = _semantics.RequiredNamespaces
				.Select(x => new Using(x))
				.ToArray();

			var nss = new Namespace(
				_semantics.Factory.Type.FullNamespace,
				BuildClass());
			
			return new SourceTreeDefinition(new DefinitionRoot(usings, nss),
				new CreationAggregator(_semantics));
		}

		private Class BuildClass()
		{
			return new Class(_semantics.Factory.Type.Name,
				BuildMethods(),
				BuildPropertyNodes(),
				BuildFieldNode(),
				_semantics.DisposableInfo);
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
			var deps = _semantics.Dependencies
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
