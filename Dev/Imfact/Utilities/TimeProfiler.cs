﻿using System;
using System.Diagnostics;

namespace Imfact.Utilities
{
	internal class TimeProfiler : IDisposable
	{
		private readonly string _title;
		private readonly DateTime _start;

		private TimeProfiler(string title, DateTime start)
		{
			_title = title;
			_start = start;
		}

		public void Dispose()
		{
			var span = DateTime.Now - _start;
			if (span.TotalMilliseconds > 5)
			{
				Debug.WriteLine($"{_title}: {span.TotalMilliseconds}ms", "Imfact");
			}
		}

		public static TimeProfiler Create(string title)
		{
			return new TimeProfiler(title, DateTime.Now);
		}
	}
}
