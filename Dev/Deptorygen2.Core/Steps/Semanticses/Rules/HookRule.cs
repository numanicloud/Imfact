using Deptorygen2.Annotations;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Steps.Aspects.Nodes;
using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using NacHelpers.Extensions;
using Attribute = Deptorygen2.Core.Steps.Aspects.Nodes.Attribute;

namespace Deptorygen2.Core.Steps.Semanticses.Rules
{
	internal sealed class HookRule
	{
		public Hook? Extract(Attribute attribute, Method method)
		{
			return FromHookAttribute(attribute, method) is { } hook ? hook
				: FromPresetAttribute(attribute, method) is { } preset ? preset
				: null;
		}

		private Hook? FromHookAttribute(Attribute attribute, Method method)
		{
			if (!attribute.IsHook())
			{
				return null;
			}

			var arg = attribute.Data.ConstructorArguments[0].Value;
			if (arg is not INamedTypeSymbol symbol)
			{
				return null;
			}

			if (!symbol.ConstructedFrom.IsImplementing(typeof(IHook<>)))
			{
				return null;
			}

			if (symbol.IsUnboundGenericType)
			{
				symbol = symbol.ConstructedFrom.Construct(method.Symbol.ReturnType);
			}

			var name = $"_{method.Symbol.Name}_{symbol.Name}";
			return new Hook(TypeNode.FromSymbol(symbol), name);
		}

		private Hook? FromPresetAttribute(Attribute attribute, Method method)
		{
			var type = attribute.IsCacheHook() ? typeof(Cache<>)
				: attribute.IsCachePerResolutionHook() ? typeof(CachePerResolution<>)
				: null;

			if (type is null)
			{
				return null;
			}

			var arg = TypeNode.FromSymbol(method.Symbol.ReturnType);
			var typeName = TypeNode.FromRuntime(type, arg.WrapByArray());
			var name = $"_{method.Symbol.Name}_{typeName.Name}";

			return new Hook(typeName, name);
		}
	}
}
