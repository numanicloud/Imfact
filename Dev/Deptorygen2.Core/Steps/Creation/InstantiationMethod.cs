using System;

namespace Deptorygen2.Core.Steps.Instantiation
{
	[Flags]
	internal enum InstantiationMethod
	{
		None = 0,
		FactoryItself = 2 << 1,
		DelegationItself = 2 << 2,
		DelegatedResolver = 2 << 3,
		DelegatedCollectionResolver = 2 << 4,
		Resolver = 2 << 5,
		CollectionResolver = 2 << 6,
		Field = 2 << 7,
		Constructor = 2 << 8,
	}
}
