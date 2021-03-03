using System;
using System.Collections.Generic;
using System.Text;

namespace Imfact.Annotations
{
	internal interface IHook<T> where T : class
	{
		void RegisterService(IResolverService service);
		T? Before();
		T After(T created);
	}

	internal class ResolutionContext
	{
		private static int _nextId = 0;

		public static int InvalidId = -1;

		public int Id { get; }

		public ResolutionContext()
		{
			Id = _nextId;
			_nextId++;
		}
	}

	internal interface IResolverService
	{
		int CurrentResolutionId { get; }
		int ResolutionDepth { get; }
	}

	internal class ResolverService : IResolverService
	{
		public int CurrentResolutionId { get; set; } = 0;
		public int ResolutionDepth { get; set; } = 0;

		public Scope Enter()
		{
			if (ResolutionDepth == 0)
			{
				CurrentResolutionId++;
			}
			ResolutionDepth++;
			return new Scope(this);
		}

		public class Scope : IDisposable
		{
			private readonly ResolverService _owner;

			public Scope(ResolverService owner)
			{
				_owner = owner;
			}

			public void Dispose()
			{
				_owner.ResolutionDepth--;
			}
		}
	}
}
