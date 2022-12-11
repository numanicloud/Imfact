using Imfact.Incremental;

namespace Imfact.Steps.Ranking;

internal record RankedClass(FactoryCandidate Self,
	FactoryCandidate? Base,
	int Rank);