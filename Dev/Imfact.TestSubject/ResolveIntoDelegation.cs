using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imfact.Annotations;

namespace Imfact.TestSubject.ResolveIntoDelegation;

internal class Service
{
	public Service(IResolveIntoDelegationBase delegation)
	{
	}
}

internal interface IResolveIntoDelegationBase
{
}

internal class ResolveIntoDelegationBase : IResolveIntoDelegationBase
{
}

// 委譲したファクトリーの実装するインターフェースを注入に使いたい時が来たらコンパイル通るようにする
[Factory]
internal partial class ResolveIntoDelegationFactory
{
	//public ResolveIntoDelegationBase Delegation { get; }

	public partial Service ResolveService();
}