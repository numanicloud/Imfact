using System.Collections.Generic;
using System.Linq;
using Imfact.Entities;
using Imfact.Steps.Semanticses.Interfaces;
using Imfact.Steps.Semanticses.Records;
using Imfact.Utilities;

namespace Imfact.Steps.Dependency.Components;

internal class FactoryDependencyContext
{
	// TODO: TypeAnalysisではなくTypeIdをKeyに使う
	private readonly Dictionary<TypeAnalysis, List<UnsatisfiedField>> _factoryFields = new();

	public void RegisterFactoryDelegation(Factory semantics)
	{
		var fields = semantics.Delegations.Where(x => x.NeedsInitialize)
			.Select(x => new UnsatisfiedField(x.Type, x.MemberName));

		if (!_factoryFields.ContainsKey(semantics.Type))
		{
			_factoryFields[semantics.Type] = new List<UnsatisfiedField>();
		}

		_factoryFields[semantics.Type].AddRange(fields);
	}

	public UnsatisfiedField RegisterUnsatisfied(TypeAnalysis type, CreationContext context)
	{
		var field = new UnsatisfiedField(type, ToFieldName(type));
		if (!_factoryFields.ContainsKey(context.FactoryType))
		{
			_factoryFields[context.FactoryType] = new List<UnsatisfiedField>();
		}
		_factoryFields[context.FactoryType].Add(field);
		return field;

		static string ToFieldName(TypeAnalysis type)
		{
			return "_" + type.Name.ToLowerCamelCase();
		}
	}

	public CreationContext MergeContext(Resolution resolution, CreationContext source)
	{
		// ユーザーはファクトリークラスにコンストラクタは持たせられないので、
		// 依存関係以外のコンストラクタ引数が0個である前提で処理する
		if (_factoryFields.ContainsKey(resolution.TypeName))
		{
			var dependencies = _factoryFields[resolution.TypeName]
				.Select(x => x.Type)
				.Distinct();
			return source with
			{
				TypeToResolve = source.TypeToResolve
					.Concat(dependencies)
					.ToArray()
			};
		}
		
		return source;
	}
}