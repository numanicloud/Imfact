using Deptorygen2.Core.Utilities;

namespace Deptorygen2.Core.Steps.Instantiation
{
	internal record InstantiationRequest(TypeName TypeToResolve,
		GivenParameter[] GivenParameters);

	internal record MultipleInstantiationRequest(TypeName[] TypeToResolve,
		GivenParameter[] GivenParameters)
	{
		public InstantiationRequest OfType(TypeName type)
		{
			return new InstantiationRequest(type, GivenParameters);
		}
	}

	internal record GivenParameter(TypeName Type, string Name);
}
