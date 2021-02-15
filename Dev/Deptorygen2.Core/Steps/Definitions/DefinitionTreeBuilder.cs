using System.Linq;
using Deptorygen2.Annotations;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Creation;
using Deptorygen2.Core.Steps.Definitions.Syntaxes;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Definitions
{
	internal class DefinitionTreeBuilder
	{
		private readonly GenerationSemantics _semantics;
		private readonly TypeName _resolutionCtxType;

		public DefinitionTreeBuilder(GenerationSemantics semantics)
		{
			_semantics = semantics;
			_resolutionCtxType = TypeName.FromType(typeof(ResolutionContext));
		}

		public SourceTreeDefinition Build()
		{
			var usings = _semantics.RequiredNamespaces
				.Select(x => new UsingNode(x))
				.ToArray();

			var nss = new NamespaceNode(
				_semantics.Factory.Type.FullNamespace,
				BuildClass());
			
			return new SourceTreeDefinition(new RootNode(usings, nss),
				new CreationAggregator(_semantics));

		}

		private ClassNode BuildClass()
		{
			return new ClassNode(_semantics.Factory.Type.Name,
				BuildConstructorNode(),
				BuildMethodNode(),
				BuildPropertyNodes(),
				BuildFieldNode(),
				BuildEntryMethodNodes());
		}

		private ConstructorNode BuildConstructorNode()
		{
			var fs = _semantics.Dependencies
				.Select(x => (type: x.TypeName,
					name: x.FieldName,
					param: x.FieldName.TrimStart("_".ToCharArray())))
				.ToArray();
			var ps = _semantics.Factory.Delegations
				.Select(x => (type: x.TypeName,
					name: x.PropertyName,
					param: x.PropertyName.ToLowerCamelCase()))
				.ToArray();

			var parameters = fs.Concat(ps).Select(x => BuildParameterNode(x.type, x.param));
			var assignments = fs.Concat(ps).Select(x => new AssignmentNode(x.name, x.param));
			var hooks = _semantics.Factory.Resolvers.SelectMany(x => x.Hooks)
				.Select(x => new AssignmentNode(x.FieldName, $"new {x.HookType.Name}()"));

			return new ConstructorNode(_semantics.Factory.Type.Name,
				parameters.ToArray(),
				assignments.Concat(hooks).ToArray());
		}

		private MethodNode[] BuildMethodNode()
		{
			var rs = _semantics.Factory.Resolvers;
			var crs = _semantics.Factory.CollectionResolvers;
			
			return rs.Cast<IResolverSemantics>().Concat(crs)
				.Select(x =>
				{
					var ps = x.Parameters.Select(
						p => BuildParameterNode(p.TypeName, p.ParameterName));

					var resolution = x.Resolutions.FirstOrDefault()?.TypeName ?? x.ReturnType;
					var hooks = x.Hooks.Select(y => new HookNode(new TypeNode(y.HookType), y.FieldName))
						.ToArray();

					return new MethodNode(x.Accessibility,
						new TypeNode(x.ReturnType),
						x.MethodName,
						ps.ToArray(),
						resolution,
						hooks);
				}).ToArray();
		}

		private EntryMethodNode[] BuildEntryMethodNodes()
		{
			var rs = _semantics.Factory.EntryResolvers;

			return rs.Select(x =>
			{
				var ps = x.Parameters.Select(p =>
					BuildParameterNode(p.TypeName, p.ParameterName));

				return new EntryMethodNode(x.Accessibility,
					new TypeNode(x.ReturnType),
					x.MethodName,
					ps.ToArray());
			}).ToArray();
		}

		private ParameterNode BuildParameterNode(TypeName type, string name)
		{
			return new ParameterNode(new TypeNode(type), name);
		}

		private PropertyNode[] BuildPropertyNodes()
		{
			return _semantics.Factory.Delegations
				.Select(x => new PropertyNode(new TypeNode(x.TypeName), x.PropertyName))
				.ToArray();
		}

		private FieldNode[] BuildFieldNode()
		{
			var deps = _semantics.Dependencies
				.Select(x => new FieldNode(new TypeNode(x.TypeName), x.FieldName));

			var hooks = _semantics.Factory.Resolvers
				.SelectMany(x => x.Hooks)
				.Select(x => new FieldNode(new TypeNode(x.HookType), x.FieldName));

			return deps.Concat(hooks).ToArray();
		}
	}
}
