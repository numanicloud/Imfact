using System;
using System.Diagnostics;
using Imfact.Annotations;

namespace Imfact.TestSubject.NoNeedImfact
{

	// Imfactを必要としていないクラスに対してコード生成しないことの確認

	// FactoryAttribute 以外の属性がついているクラスにコード生成しないこと
	[Obsolete]
	internal class Hoge
	{
	}

	// FactoryAttribute がついていても、partialでないならコード生成しないこと
	[Factory]
	internal class NotPartial
	{
	}

	// アクセス修飾子つき partial のメソッドだけを生成対象にすること
	[Factory]
	internal partial class NotPublicPartial
	{
		partial void Resolve();
	}
}