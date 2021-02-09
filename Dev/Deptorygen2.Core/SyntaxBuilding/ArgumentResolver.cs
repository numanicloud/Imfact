using System.Collections.Generic;
using System.Linq;
using Deptorygen2.Core.Syntaxes;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Coder
{
	internal class ArgumentResolver
	{
		private readonly FactorySemantics _factory;
		private readonly DependencyDefinition[] _fields;
		private readonly TypeName _factoryType;
		private readonly Dictionary<TypeName, string> _cache = new();

		public ArgumentResolver(FactorySemantics factory, DependencyDefinition[] fields)
		{
			_factory = factory;
			_fields = fields;
			_factoryType = TypeName.FromSymbol(factory.ItselfSymbol);
		}

		public string[] GetArgumentCodes(TypeName[] argumentTypes, ResolverParameterDefinition[] parameters)
		{
			var table = parameters.GroupBy(x => x.Type)
				.ToDictionary(x => x.Key, x => x.ToList());
			return argumentTypes.Select(t =>
			{
				if (table.GetValueOrDefault(t) is { } onType)
				{
					var result = onType[0].Name;
					onType.RemoveAt(0);
					return result;
				}
				else
				{
					return GetArgumentCode(t, parameters) ?? "<Error>";
				}
			}).ToArray();
		}

		public string? GetArgumentCode(TypeName typeName, ResolverParameterDefinition[] parameters)
		{
			if (_cache.GetValueOrDefault(typeName) is { } resolution)
			{
				return resolution;
			}

			if (typeName == _factoryType)
			{
				return _cache[typeName] = "this";
			}

			foreach (var delegation in _factory.Delegations)
			{
				if (typeName == delegation.TypeName)
				{
					return _cache[typeName] = delegation.PropertyName;
				}
			}

			foreach (var delegation in _factory.Delegations)
			{
				foreach (var resolver in delegation.Resolvers)
				{
					if (typeName == resolver.ReturnTypeName)
					{
						var argTypes = resolver.Parameters.Select(x => x.TypeName).ToArray();
						var argList = GetArgumentCodes(argTypes, parameters)
							.Join(", ");
						return _cache[typeName] = $"{delegation.PropertyName}.{resolver}({argList})";
					}
				}
			}

			foreach (var resolver in _factory.Resolvers)
			{
				if (typeName == resolver.ReturnTypeName)
				{
					var argTypes = resolver.Parameters.Select(x => x.TypeName).ToArray();
					var argList = GetArgumentCodes(argTypes, parameters)
						.Join(", ");
					return _cache[typeName] = $"{resolver}({argList})";
				}
			}

			foreach (var field in _fields)
			{
				if (typeName == field.FieldType)
				{
					return _cache[typeName] = field.FieldName;
				}
			}

			return null;
		}
	}
}