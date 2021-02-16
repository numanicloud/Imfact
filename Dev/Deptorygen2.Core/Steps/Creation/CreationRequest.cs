using Deptorygen2.Core.Steps.Semanticses.Nodes;
using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Creation
{
	internal record CreationRequest(TypeNode TypeToResolve,
		GivenParameter[] GivenParameters,
		bool IsRootRequest);

	internal record MultipleCreationRequest(TypeNode[] TypeToResolve,
		GivenParameter[] GivenParameters,
		bool IsRootRequest)
	{
		public CreationRequest OfType(TypeNode type)
		{
			return new(type, GivenParameters, IsRootRequest);
		}
	}

	internal record GivenParameter(TypeNode Type, string Name);
}
