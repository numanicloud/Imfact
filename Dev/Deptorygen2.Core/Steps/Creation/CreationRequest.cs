using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Creation
{
	internal record CreationRequest(TypeName TypeToResolve, GivenParameter[] GivenParameters);

	internal record MultipleCreationRequest(TypeName[] TypeToResolve,
		GivenParameter[] GivenParameters)
	{
		public CreationRequest OfType(TypeName type)
		{
			return new(type, GivenParameters);
		}
	}

	internal record GivenParameter(TypeName Type, string Name);
}
