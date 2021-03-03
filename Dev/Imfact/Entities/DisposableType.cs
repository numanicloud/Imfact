using System;

namespace Imfact.Entities
{
	[Flags]
	internal enum DisposableType
	{
		NonDisposable = 0,
		Disposable = 1,
		AsyncDisposable = 2
	}
}