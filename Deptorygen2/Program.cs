using System;
using Deptorygen2.Core;

namespace Deptorygen2
{
	class Program
	{
		static void Main(string[] args)
		{
			var sample = new SampleFactory().Create();
			var renderer = new FactorySourceBuilder();

			Console.WriteLine(renderer.Build(sample));
		}
	}
}
