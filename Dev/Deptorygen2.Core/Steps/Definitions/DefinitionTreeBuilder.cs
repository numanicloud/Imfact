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
		private readonly TypeNode _ctxType;

		public DefinitionTreeBuilder(Generation semantics)
		{
			_semantics = semantics;
			_ctxType = TypeNode.FromRuntime(typeof(ResolutionContext));
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
			var ctor = BuildConstructorInfo();
			var resolvers = BuildResolverInfo();
			var multiResolvers = BuildEnumerableMethodInfo();
			var entryMethods = BuildEntryMethodInfo();
			var methods = ctor.WrapByArray()
				.Concat(entryMethods)
				.Concat(resolvers)
				.Concat(multiResolvers)
				.ToArray();

			return new Class(_semantics.Factory.Type.Name,
				methods,
				BuildPropertyNodes(),
				BuildFieldNode(),
				_semantics.DisposableInfo);
		}

		private MethodInfo BuildConstructorInfo()
		{
			var fs = _semantics.Dependencies
				.Select(x => (type: x.TypeName,
					name: x.FieldName,
					param: x.FieldName.TrimStart("_".ToCharArray())))
				.ToArray();
			var ps = _semantics.Factory.Delegations
				.Select(x => (type: x.Type,
					name: x.PropertyName,
					param: x.PropertyName.ToLowerCamelCase()))
				.ToArray();

			var parameters = fs.Concat(ps).Select(x => BuildParameterNode(x.type, x.param)).ToArray();
			var assignments = fs.Concat(ps).Select(x => new Assignment(x.name, x.param));
			var hooks = _semantics.Factory.Resolvers.Cast<IResolverSemantics>()
				.Concat(_semantics.Factory.MultiResolvers)
				.SelectMany(x => x.Hooks)
				.Select(x => new Assignment(x.FieldName, $"new {x.HookType.FullBoundName}()"));

			var a = Helpers.GetTypeAccessibilityMostStrict(
				ps.Select(x => x.type.Accessibility).ToArray());

			var signature = new ConstructorSignature(a, _semantics.Factory.Type.Name, parameters);
			var impl = new InitializeImplementation(assignments.Concat(hooks).ToArray());
			return new MethodInfo(signature, new Attribute[0], impl);
		}

		private MethodInfo[] BuildResolverInfo()
		{
			return _semantics.Factory.Resolvers
				.Select(x =>
				{
					var ps = x.Parameters.Select(
						p => BuildParameterNode(p.Type, p.ParameterName))
						.ToArray();
					var resolution = x.Resolutions.FirstOrDefault()?.TypeName ?? x.ReturnType;
					var hooks = x.Hooks.Select(y => new Hook(new Type(y.HookType), y.FieldName))
						.ToArray();

					var signature = new OrdinalSignature(Accessibility.Internal,
						new Type(x.ReturnType), x.MethodName, ps, new string[0]);
					var attribute = new Attribute("[EditorBrowsable(EditorBrowsableState.Never)]");
					var impl = new ResolveImplementation(hooks, new Type(resolution),
						new Type(x.ReturnType), ps);

					return new MethodInfo(signature, attribute.WrapByArray(), impl);
				}).ToArray();
		}

		private MethodInfo[] BuildEnumerableMethodInfo()
		{
			return _semantics.Factory.MultiResolvers
				.Select(x =>
				{
					var ps = x.Parameters.Select(
						p => BuildParameterNode(p.Type, p.ParameterName)).ToArray();
					var hooks = x.Hooks.Select(y => new Hook(new Type(y.HookType), y.FieldName))
						.ToArray();
					var resolutions = x.Resolutions.Select(y => new Type(y.TypeName)).ToArray();

					var signature = new OrdinalSignature(Accessibility.Internal,
						new Type(x.ReturnType), x.MethodName, ps, new string[0]);
					var attribute = new Attribute("[EditorBrowsable(EditorBrowsableState.Never)]");
					var impl = new MultiResolveImplementation(hooks, new Type(x.ElementType),
						resolutions, ps);

					return new MethodInfo(signature, attribute.WrapByArray(), impl);
				}).ToArray();
		}

		private MethodInfo[] BuildEntryMethodInfo()
		{
			return _semantics.Factory.EntryResolvers.Select(x =>
			{
				var ps = x.Parameters
					.Where(y => !y.Type.Record.Equals(_ctxType.Record))
					.Select(p => BuildParameterNode(p.Type, p.ParameterName))
					.ToArray();

				var signature = new OrdinalSignature(x.Accessibility, new Type(x.ReturnType),
					x.MethodName, ps, new []{"partial"});
				var impl = new EntryImplementation(x.MethodName, ps);

				return new MethodInfo(signature, new Attribute[0], impl);
			}).ToArray();
		}

		private Parameter BuildParameterNode(TypeNode type, string name)
		{
			return new Parameter(new Type(type), name);
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
