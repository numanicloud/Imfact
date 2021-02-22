using System;

namespace Deptorygen2.Core.Utilities
{
	[Flags]
	internal enum DisposableType
	{
		NonDisposable = 0,
		Disposable = 1,
		AsyncDisposable = 2
	}
}