using System;
using System.Linq;
using System.Text;

namespace Deptorygen2.Core.Utilities
{
	public class BlockStringBuilder : IDisposable
	{
		private readonly StringBuilder _target;
		private readonly string _indentPart;

		public BlockStringBuilder(string title, StringBuilder target, int indent)
		{
			_target = target;

			_indentPart = Enumerable.Range(0, indent).Aggregate("", (s, i) => s + "\t");
			_target.AppendLine(_indentPart + title);
			_target.AppendLine(_indentPart + "{");
		}

		public void Dispose()
		{
			_target.AppendLine(_indentPart + "}");
		}
	}
}