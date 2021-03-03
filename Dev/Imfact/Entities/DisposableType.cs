using System;

namespace Deptorygen2.Core.Entities
{
	[Flags]
	internal enum DisposableType
	{
		NonDisposable = 0,
		Disposable = 1,
		AsyncDisposable = 2
	}
}