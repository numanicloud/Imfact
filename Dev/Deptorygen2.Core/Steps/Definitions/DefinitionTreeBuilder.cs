using System.Linq;
using Deptorygen2.Annotations;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Creation;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Definitions
{
	internal class DefinitionTreeBuilder
	{
		private readonly Generation _semantics;
		private readonly TypeName _resolutionCtxType;

		public DefinitionTreeBuilder(Generation semantics)
		{
			_semantics = semantics;
			_resolutionCtxType = TypeName.FromType(typeof(ResolutionContext));
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
				BuildConstructorNode(),
				BuildMethodNode(),
				BuildEnumerableMethods(),
				BuildPropertyNodes(),
				BuildFieldNode(),
				BuildEntryMethodNodes());
		}

		private Constructor BuildConstructorNode()
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

			var parameters = fs.Concat(ps).Select(x => BuildParameterNode(x.type, x.param));
			var assignments = fs.Concat(ps).Select(x => new Assignment(x.name, x.param));
			var hooks = _semantics.Factory.Resolvers.Cast<IResolverSemantics>()
				.Concat(_semantics.Factory.MultiResolvers)
				.SelectMany(x => x.Hooks)
				.Select(x => new Assignment(x.FieldName, $"new {x.HookType.Name}()"));

			return new Constructor(_semantics.Factory.Type.Name,
				parameters.ToArray(),
				assignments.Concat(hooks).ToArray());
		}

		private Method[] BuildMethodNode()
		{
			return _semantics.Factory.Resolvers
				.Select(x =>
				{
					var ps = x.Parameters.Select(
						p => BuildParameterNode(p.TypeName, p.ParameterName));

					var resolution = x.Resolutions.FirstOrDefault()?.TypeName ?? x.ReturnType;
					var hooks = x.Hooks.Select(y => new Hook(new Type(y.HookType), y.FieldName))
						.ToArray();

					return new Method(x.Accessibility,
						new Type(x.ReturnType),
						x.MethodName,
						ps.ToArray(),
						resolution,
						hooks);
				}).ToArray();
		}

		private EnumMethod[] BuildEnumerableMethods()
		{
			return _semantics.Factory.MultiResolvers
				.Select(x =>
				{
					var ps = x.Parameters.Select(
						p => BuildParameterNode(p.TypeName, p.ParameterName));
					var hooks = x.Hooks.Select(y => new Hook(new Type(y.HookType), y.FieldName))
						.ToArray();

					return new EnumMethod(x.Accessibility,
						new Type(x.ElementType),
						x.MethodName,
						ps.ToArray(),
						x.Resolutions.Select(x => x.TypeName).ToArray(),
						hooks);
				}).ToArray();
		}

		private EntryMethod[] BuildEntryMethodNodes()
		{
			var rs = _semantics.Factory.EntryResolvers;

			return rs.Select(x =>
			{
				var ps = x.Parameters.Select(p =>
					BuildParameterNode(p.TypeName, p.ParameterName));

				return new EntryMethod(x.Accessibility,
					new Type(x.ReturnType),
					x.MethodName,
					ps.ToArray());
			}).ToArray();
		}

		private Parameter BuildParameterNode(TypeName type, string name)
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
				.Select(x => new Field(new Type(x.TypeName), x.FieldName));

			var hooks = _semantics.Factory.Resolvers
				.SelectMany(x => x.Hooks)
				.Select(x => new Field(new Type(x.HookType), x.FieldName));

			var hooks2 = _semantics.Factory.MultiResolvers
				.SelectMany(x => x.Hooks)
				.Select(x => new Field(new Type(x.HookType), x.FieldName));

			return deps.Concat(hooks).Concat(hooks2).ToArray();
		}
	}
}
