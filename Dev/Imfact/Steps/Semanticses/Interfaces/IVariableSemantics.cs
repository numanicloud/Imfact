using Imfact.Entities;

namespace Imfact.Steps.Semanticses.Interfaces;

internal interface IVariableSemantics
{
	TypeAnalysis Type { get; }
	string MemberName { get; }
}