using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deptorygen2.Core
{
	public interface ISymbolFinder
	{
		Type FindType(string fullName);
	}

	partial class MyPartialBase
	{
	}

	partial class MyPartial : MyPartialBase, ISymbolFinder
	{
		public partial int Add(int x, int y);
		public Type FindType(string fullName)
		{
			throw new NotImplementedException();
		}
	}
}

/* デコレーションクラスを示す属性をメソッドに付けたい。どう検知する？
 *
 * デコレーションクラスが以下を満たしていることは仮定してよい：
 *		型引数Tを1つだけ持ち、属性をつけたメソッドの戻り値の型を代入できる。
 *		Hookメソッドを持つ。
 *			戻り値は型引数と同じ型である。
 *			引数は、ResolutionContextと、Func<T>である。
 *
 * [Hook(typeof(ResolutionCache<>))] みたいに書くとよさそう
 */