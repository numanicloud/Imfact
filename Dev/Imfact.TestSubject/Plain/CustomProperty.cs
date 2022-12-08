using Imfact.Annotations;

namespace Imfact.TestSubject.CustomProperty;

internal class Service
{
}

[Factory]
internal interface IGoldFactory
{
	Service ResolveService();
}

internal class GoldFactory : IGoldFactory
{
	public Service ResolveService() => new Service();
}

[Factory]
internal partial class CustomPropertyFactory
{
	// プロパティをカスタマイズできるようにしたい
	// その際、生成されるコードはプロパティに代入しないようにする必要がある
	// それによって余計なコンストラクタ引数が作られるのも防げる
	public IGoldFactory Gold => new GoldFactory();

	public partial Service ResolveService();
}

internal class CustomPropertyProgram
{
	public void Main()
	{
		var factory = new CustomPropertyFactory();
	}
}