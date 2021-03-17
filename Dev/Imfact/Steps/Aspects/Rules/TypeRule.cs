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
				.Select(x => TypeAnalysis.FromSymbol(x.Type))
				.ToArray() ?? new TypeAnalysis[0];

			if (symbol.IsUnboundGenericType)
			{
				symbol = symbol.ConstructedFrom.Construct(typeArguments);
			}

			return new TypeToCreate(TypeAnalysis.FromSymbol(symbol), args);
		}
	}
}
