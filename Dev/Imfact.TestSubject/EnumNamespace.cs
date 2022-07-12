using Imfact.Annotations;

namespace Imfact.TestSubject.EnumNamespace
{
	internal class Service<T> where T : struct
	{
		public Service(Enum2.MyEnum2 value)
		{
		}

		public T GetDefault() => default;
	}

	[Factory]
	internal partial class EnumNamespaceFactory
	{
		// 必要な時が来たら、戻り値をジェネリックに出来るようにする
		public partial Service<Enum.MyEnum> Resolve(Enum2.MyEnum2 arg);
	}
}

namespace Imfact.TestSubject.EnumNamespace.Enum
{
	public enum MyEnum
	{
		Hoge, Fuga
	}
}

namespace Imfact.TestSubject.EnumNamespace.Enum2
{
	public enum MyEnum2
	{
		Hoge, Fuga
	}
}
