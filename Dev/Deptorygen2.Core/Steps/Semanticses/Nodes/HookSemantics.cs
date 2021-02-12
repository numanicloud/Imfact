using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Deptorygen2.Core.Interfaces;
using Deptorygen2.Core.Steps.Aggregation;
using Deptorygen2.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Steps.Semanticses.Nodes
{
	internal record HookSemantics(TypeName HookType, string FieldName)
	{
		public static HookSemantics? Build(AttributeToAnalyze attribute,
			IAnalysisContext context,
			string methodName)
		{
			if (!attribute.IsHook())
			{
				return null;
			}

			var arg = attribute.Data.ConstructorArguments[0].Value;
			if (arg is not Type t)
			{
				return null;
			}

			var isHook = t.GetInterfaces().Any(
				x => x.IsGenericType && x.GetGenericTypeDefinition().Name == "IHook`1");
			if (!isHook)
			{
				return null;
			}

			// IHook<T> _Method_HookType = new HookType<T>();

			var name = $"_{methodName}_{t.Name}";
			return new HookSemantics(TypeName.FromType(t), name);
		}
	}
}
