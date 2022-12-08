
// ReSharper disable CheckNamespace
namespace Imfact.TestSubject.Executable;

public static class Program
{
	public static void Main()
	{
		int successCount = 0;
		int errorCount = 0;

		RunTest<ResolveSingle.Test>();

		Console.ForegroundColor = ConsoleColor.White;
		Console.WriteLine($"{errorCount} tests failed, {successCount} tests passed.");
		Console.ForegroundColor = ConsoleColor.Gray;
		
		void RunTest<T>() where T : ITest, new()
		{
			Console.WriteLine($"## {typeof(T).FullName}");
			try
			{
				var test = new T();
				test.Run();
				++successCount;
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"### Error!");
				Console.WriteLine(ex);
				Console.ForegroundColor = ConsoleColor.Gray;
				++errorCount;
			}
		}
	}
}

internal interface ITest
{
    void Run();
}

internal static class Assert
{
    public static void AssertEquals<T>(this T actual, T expected) where T : IEquatable<T>
    {
        if (!actual.Equals(expected))
        {
            throw new Exception($"{actual} is not equals to {expected}.");
        }
    }
}