using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imfact.Annotations;
using Imfact.Entities;
using Imfact.Steps.Definitions.Methods;
using Imfact.Steps.Dependency;
using Imfact.Steps.Semanticses;
using Imfact.Steps.Semanticses.Interfaces;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;
using Attribute = Imfact.Steps.Definitions.Methods.Attribute;

namespace Imfact.Steps.Definitions
{
	internal record Initialization(TypeNode Type, string Name, string ParamName);

	internal sealed class MethodBuilder
	{
		private readonly SemanticsRoot _semantics;
		private readonly InjectionResult _injection;

		public MethodBuilder(DependencyRoot semantics)
		{
			_semantics = semantics.Semantics;
			_injection = semantics.Injection;
		}

		public MethodInfo BuildConstructorInfo()
		{
			var fs = _injection.Dependencies
				.Select(x => new Initialization(x.TypeName, x.FieldName, x.FieldName))
				.Select(x => x with { ParamName = x.ParamName.TrimStart("_".ToCharArray()) })
				.ToArray();

			var ps = _semantics.Factory.Delegations
				.Select(x => new Initialization(x.Type, x.PropertyName, x.PropertyName))
				.Select(x => x with { ParamName = x.ParamName.ToLowerCamelCase() })
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

		private Implementation GetCtorImpl(Initialization[] asignee)
		{
			var h = _semantics.Factory.Resolvers.Cast<IResolverSemantics>()
				.Concat(_semantics.Factory.MultiResolvers)
				.SelectMany(x => x.Hooks)
				.Select(x => new Hook(new Type(x.HookType), x.FieldName))
				.ToArray();

			return new ConstructorImplementation(asignee, h);
		}

		public static readonly Type ResolverServiceType = new(TypeNode.FromRuntime(typeof(ResolverService)));

		public MethodInfo? BuildRegisterServiceMethodInfo()
		{
			if (_semantics.Factory.Inheritances.Any())
			{
				return null;
			}

			var signature = new OrdinalSignature(Accessibility.Internal,
				new Type(TypeNode.FromRuntime(typeof(void))),
				"RegisterService",
				new[] { new Parameter(ResolverServiceType, "service", false) },
				new string[0]);

			var p = _semantics.Factory.Delegations
				.Select(x => new Property(new Type(x.Type), x.PropertyName))
				.ToArray();
			var h = _semantics.Factory.Resolvers
				.Concat<IResolverSemantics>(_semantics.Factory.MultiResolvers)
				.SelectMany(x => x.Hooks)
				.Select(x => new Hook(new Type(x.HookType), x.FieldName))
				.ToArray();

			var impl = new RegisterServiceImplementation(p, h);
			return new MethodInfo(signature, new Attribute[0], impl);
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

			var signature = new OrdinalSignature(x.Accessibility,
				new Type(x.ReturnType), x.MethodName, ps, new string[]{ "partial" });
			var attribute = new Attribute("[EditorBrowsable(EditorBrowsableState.Never)]");
			var impl = makeImpl(hooks);

			return new MethodInfo(signature, new Attribute[0], impl);
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
			var modifiers = isAsync ? new[] { "async" } : new string[0];
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
			return new(new Type(type), name, false);
		}
	}
}
