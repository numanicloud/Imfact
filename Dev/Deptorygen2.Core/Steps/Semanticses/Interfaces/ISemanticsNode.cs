using System.Collections.Generic;

namespace Deptorygen2.Core.Steps.Semanticses.Interfaces
{
	internal interface ISemanticsNode
	{
		IEnumerable<ISemanticsNode> Traverse();
	}

	internal static class SemanticsHelper
	{
		public static IEnumerable<ISemanticsNode> TraverseDeep(this ISemanticsNode root)
		{
			yield return root;
			foreach (var node in root.Traverse())
			{
				foreach (var node2 in TraverseDeep(node))
				{
					yield return node2;
				}
			}
		}
	}
}
