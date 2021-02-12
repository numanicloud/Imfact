using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Creation
{
	internal record CreationRequest(TypeName TypeToResolve,
		GivenParameter[] GivenParameters,
		bool IsRootRequest);

	internal record MultipleCreationRequest(TypeName[] TypeToResolve,
		GivenParameter[] GivenParameters,
		bool IsRootRequest)
	{
		public CreationRequest OfType(TypeName type)
		{
			return new(type, GivenParameters, IsRootRequest);
		}
	}

	internal record GivenParameter(TypeName Type, string Name);
}
