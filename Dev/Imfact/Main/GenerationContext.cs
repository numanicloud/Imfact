using Imfact.Entities;

namespace Imfact.Main;

internal class GenerationContext
{
	public Dictionary<TypeId, ConstructorRecord> Constructors { get; } = new();
    public required Logger Logger { get; init; }
}

internal class StepSample
{
	private static StepSample? _instance;
	public static StepSample Instance => _instance ??= new StepSample();

	private LoggerWrapSample _wrap;
	private SubStepSample _subStep;

	public StepSample()
	{
		_wrap = new LoggerWrapSample();
		_subStep = new SubStepSample(_wrap);
	}

	public void Run(ILoggerSample logger)
	{
		_wrap.Inner = logger;
		_subStep.Run();
	}
}

internal class SubStepSample
{
	private readonly ILoggerSample _logger;

	public SubStepSample(ILoggerSample logger)
	{
		_logger = logger;
	}

	public void Run()
	{
		Console.WriteLine(_logger.ToString());
	}
}

internal interface ILoggerSample
{
}

internal class LoggerWrapSample : ILoggerSample
{
	public ILoggerSample Inner { get; set; } = new NothingLoggerSample();
}

internal class NothingLoggerSample : ILoggerSample
{
}