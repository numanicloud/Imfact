using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deptorygen2.Annotations;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Steps.Definitions.Methods;
using Deptorygen2.Core.Steps.Expressions;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using Attribute = Deptorygen2.Core.Steps.Definitions.Methods.Attribute;

namespace Deptorygen2.Core.Steps.Definitions
{
	internal sealed class MethodBuilder
	{
		private readonly SemanticsRoot _semantics;
		private readonly InjectionResult _injection;
		private readonly TypeNode _ctxType;

		private record Initialization(TypeNode Type, string Name, string ParamName);

		public MethodBuilder(DependencyRoot semantics)
		{
			_semantics = semantics.Semantics;
			_injection = semantics.Injection;
			_ctxType = TypeNode.FromRuntime(typeof(ResolutionContext));
		}

		public MethodInfo BuildConstructorInfo()
		{
			var fs = _injection.Dependencies
				.Select(x => new Initialization(x.TypeName, x.FieldName, x.FieldName))
				.Select(x => x with {ParamName = x.ParamName.TrimStart("_".ToCharArray())})
				.ToArray();

			var ps = _semantics.Factory.Delegations
				.Select(x => new Initialization(x.Type, x.PropertyName, x.PropertyName))
				.Select(x => x with {ParamName = x.ParamName.ToLowerCamelCase()})
				.ToArray();

			var signature = GetCtorSignature(fs, ps);
			var impl = GetCtorImpl(fs.Concat(ps).ToArray());
			return new MethodInfo(signature, new Attribute[0], impl);
		}

		private ConstructorSignature GetCtorSignature(Initialization[] fs, Initialization[] ps)
		{
			var parameters = fs.Concat(ps)
				.Select(x => BuildParameterNode(x.Type, x.ParamName))
				.ToArray();

			var baseParameters = _semantics.Factory.Inheritances
				.FirstOrDefault()?.Parameters
				.Select(x => BuildParameterNode(x.Type, x.ParameterName))
				.ToArray();

			var a = AnalyzerHelper.GetTypeAccessibilityMostStrict(
				ps.Select(x => x.Type.Accessibility).ToArray());

			return new ConstructorSignature(a,
				_semantics.Factory.Type.Name,
				parameters,
				baseParameters);
		}

		private InitializeImplementation GetCtorImpl(Initialization[] asignee)
		{
			var assignments = asignee.Select(x => new Assignment(x.Name, x.ParamName));
			var hooks = _semantics.Factory.Resolvers.Cast<IResolverSemantics>()
				.Concat(_semantics.Factory.MultiResolvers)
				.SelectMany(x => x.Hooks)
				.Select(x => new Assignment(x.FieldName, $"new {x.HookType.FullBoundName}()"));

			return new InitializeImplementation(assignments.Concat(hooks).ToArray());
		}

		public MethodInfo[] BuildResolverInfo()
		{
			return _semantics.Factory.Resolvers
				.Select(x =>
				{
					return BuildMethodCommon(x, hooks1 =>
					{
						var exp = _injection.Table[x].Root.Code;
						return new ExpressionImplementation(hooks1, new Type(x.ReturnType), exp);
					});
				}).ToArray();
		}

		public MethodInfo[] BuildEnumerableMethodInfo()
		{
			return _semantics.Factory.MultiResolvers
				.Select(x =>
				{
					return BuildMethodCommon(x, hooks1 =>
					{
						var exp = _injection.MultiCreation[x].Roots.Select(y => y.Code).ToArray();
						return new MultiExpImplementation(hooks1, new Type(x.ElementType), exp);
					});
				}).ToArray();
		}

		private MethodInfo BuildMethodCommon(IResolverSemantics x,
			Func<Hook[], Implementation> makeImpl)
		{
			var ps = x.Parameters.Select(
					p => BuildParameterNode(p.Type, p.ParameterName))
				.ToArray();
			var hooks = x.Hooks.Select(y => new Hook(new Type(y.HookType), y.FieldName))
				.ToArray();

			var signature = new OrdinalSignature(Accessibility.Internal,
				new Type(x.ReturnType), x.MethodName, ps, new string[0]);
			var attribute = new Attribute("[EditorBrowsable(EditorBrowsableState.Never)]");
			var impl = makeImpl(hooks);

			return new MethodInfo(signature, attribute.WrapByArray(), impl);
		}

		public MethodInfo[] BuildEntryMethodInfo()
		{
			return _semantics.Factory.EntryResolvers.Select(x =>
			{
				var ps = x.Parameters
					.Where(y => !y.Type.Record.Equals(_ctxType.Record))
					.Select(p => BuildParameterNode(p.Type, p.ParameterName))
					.ToArray();

				var signature = new OrdinalSignature(x.Accessibility, new Type(x.ReturnType),
					x.MethodName, ps, new[] { "partial" });
				var impl = new EntryImplementation(x.MethodName, ps);

				return new MethodInfo(signature, new Attribute[0], impl);
			}).ToArray();
		}

		public IEnumerable<MethodInfo> BuildDisposeMethodInfo()
		{
			var hooks = _semantics.Factory.Resolvers
				.Concat<IResolverSemantics>(_semantics.Factory.MultiResolvers)
				.SelectMany(x => x.Hooks);

			var disposables = _injection.Dependencies
				.Concat<IProbablyDisposable>(_semantics.Factory.Delegations)
				.Concat(hooks)
				.ToArray();

			var syncD = disposables
				.Where(x => x.Type.DisposableType.HasFlag(DisposableType.Disposable))
				.ToArray();
			if (syncD.Any())
			{
				yield return BuildDisposable(syncD, false);
			}

			var asyncD = disposables
				.Where(x => x.Type.DisposableType.HasFlag(DisposableType.AsyncDisposable))
				.ToArray();
			if (asyncD.Any())
			{
				yield return BuildDisposable(asyncD, true);
			}
		}

		private MethodInfo BuildDisposable(
			IEnumerable<IProbablyDisposable> disposables,
			bool isAsync)
		{
			var methodName = isAsync ? "DisposeAsync" : "Dispose";
			var modifiers = isAsync ? new[] {"async"} : new string[0];
			var returnType = isAsync ? typeof(ValueTask) : typeof(void);

			var signature = new OrdinalSignature(Accessibility.Public,
				new Type(TypeNode.FromRuntime(returnType)),
				methodName, new Parameter[0], modifiers);

			var impl = new DisposeImplementation(
				disposables.Select(x => x.MemberName).ToArray(),
				isAsync);

			return new MethodInfo(signature, new Attribute[0], impl);
		}

		private Parameter BuildParameterNode(TypeNode type, string name)
		{
			return new(new Type(type), name);
		}
	}
}
