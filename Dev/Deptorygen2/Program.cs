using System;
using Deptorygen2.Core;

namespace Deptorygen2
{
	class Program
	{
		static void Main(string[] args)
		{
			var analyzer = new Analyzer();
			analyzer.Analyze(SampleSourceCode.Factory1);
		}
	}
}
