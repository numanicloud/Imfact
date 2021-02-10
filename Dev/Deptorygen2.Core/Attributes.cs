using System;

namespace Deptorygen2.Core
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class FactoryAttribute : Attribute
	{
		// See the attribute guidelines at 
		//  http://go.microsoft.com/fwlink/?LinkId=85236
		public FactoryAttribute()
		{
		}
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	public sealed class ResolutionAttribute : Attribute
	{
		// See the attribute guidelines at 
		//  http://go.microsoft.com/fwlink/?LinkId=85236
		public ResolutionAttribute(Type type)
		{
		}
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class DelegationAttribute : Attribute
	{
		// See the attribute guidelines at 
		//  http://go.microsoft.com/fwlink/?LinkId=85236
		public DelegationAttribute()
		{
		}
	}
}
