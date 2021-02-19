using System.Linq;
using Deptorygen2.Core.Entities;
using Microsoft.CodeAnalysis;

namespace Deptorygen2.Core.Steps.Aspects
{
	internal sealed class TypeRule
	{
		public TypeToCreate ExtractTypeToCreate(INamedTypeSymbol symbol, params ITypeSymbol[] typeArguments)
		{
			var args = symbol.Constructors.FirstOrDefault()?.Parameters
				.Select(x => TypeNode.FromSymbol(x.Type))
				.ToArray() ?? new TypeNode[0];

			if (symbol.IsUnboundGenericType)
			{
				symbol = symbol.ConstructedFrom.Construct(typeArguments);
			}

			return new TypeToCreate(TypeNode.FromSymbol(symbol), args);
		}
	}
}
