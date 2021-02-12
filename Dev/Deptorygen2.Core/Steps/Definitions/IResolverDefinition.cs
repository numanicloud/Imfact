using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deptorygen2.Core.Steps.Definitions
{
	internal interface IResolverDefinition
	{
		string MethodName { get; }
		ResolverParameterDefinition[] Parameters { get; }
	}
}
