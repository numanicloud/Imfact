using System.Collections.Generic;
using Deptorygen2.Annotations;
using Deptorygen2.Core.Entities;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aspects.Nodes;
using Deptorygen2.Core.Steps.Semanticses.Interfaces;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record Hook(TypeNode HookType, string FieldName) : ISemanticsNode
	{
		public static Hook? Build(Attribute attribute,
			IAnalysisContext context,
			Method method)
		{
			string methodName = method.Symbol.Name;

			if (FromHookAttribute() is {} hook)
			{
				return hook;
			}

			if (FromPresetAttribute() is {} hook2)
			{
				return hook2;
			}
			
			return null;

			Hook? FromHookAttribute()
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

				// symbol.IsUnboundGenericType == true だとしても、
				// Constructメソッドは「別の構築済みの型から、型を構築することができません」の例外を投げる。なぜ？

				if (!symbol.ConstructedFrom.IsImplementing(typeof(IHook<>)))
				{
					return null;
				}

				if (symbol.IsUnboundGenericType)
				{
					symbol = symbol.ConstructedFrom.Construct(method.Symbol.ReturnType);
				}

				var name = $"_{methodName}_{symbol.Name}";
				return new Hook(TypeNode.FromSymbol(symbol), name);
			}

			Hook? FromPresetAttribute()
			{
				if (attribute.IsCacheHook())
				{
					var type = typeof(Cache<>);
					var arg = TypeNode.FromSymbol(method.Symbol.ReturnType);
					var typeName = TypeNode.FromRuntime(type, arg.WrapByArray());

					var name = $"_{methodName}_Cache";
					return new Hook(typeName, name);
				}

				if (attribute.IsCachePerResolutionHook())
				{
					var type = typeof(CachePerResolution<>);
					var arg = TypeNode.FromSymbol(method.Symbol.ReturnType);
					var typeName = TypeNode.FromRuntime(type, arg.WrapByArray());

					var name = $"_{methodName}_CachePerResolution";
					return new Hook(typeName, name);
				}

				return null;
			}
		}

		public IEnumerable<ISemanticsNode> Traverse()
		{
			yield break;
		}
	}
}
