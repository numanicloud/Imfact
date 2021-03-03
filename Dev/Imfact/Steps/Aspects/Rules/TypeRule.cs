using System.Linq;
using Imfact.Entities;
using Microsoft.CodeAnalysis;

namespace Imfact.Steps.Aspects.Rules
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
