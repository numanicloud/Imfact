using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Imfact.Main;

namespace Imfact.Utilities;

internal static class AggregationProfiler
{
	private static readonly Dictionary<string, List<TimeSpan>> Profiles = new ();

	public static void Clear()
	{
		Profiles.Clear();
	}

	public static IDisposable GetScope(string subTitle = "",
		[CallerMemberName]string memberName = "",
		[CallerFilePath]string filePath = "")
	{
		var fileName = Path.GetFileName(filePath);
		var title = $"{fileName}:{memberName}:{subTitle}";

		if (!Profiles.ContainsKey(title))
		{
			Profiles[title] = new List<TimeSpan> ();
		}
		return Scope.Start(title, AddProfile);
	}

	public static void WriteStats(Logger logger)
	{
		var builder = new StringBuilder();
		builder.AppendLine("Profiled data:");

		foreach (var profile in Profiles)
		{
			var sum = profile.Value.Sum(x => x.Milliseconds);
            var average = sum / profile.Value.Count;
			var max = profile.Value.Max(x => x.Milliseconds);
			var text = $"{profile.Key}: Sum={sum}, Average={average}, Max={max}";

			builder.AppendLine(text);
		}

        logger.Debug(builder.ToString());
	}

	private static void AddProfile(string title, TimeSpan time)
	{
		Profiles[title].Add(time);
	}

    private sealed class Scope : IDisposable
	{
		private readonly string _title;
        private readonly DateTime _startTime;
        private readonly Action<string, TimeSpan> _onFinished;
        private bool _isFinished = false;

        private Scope(DateTime startTime, Action<string, TimeSpan> onFinished, string title)
        {
            _startTime = startTime;
            _onFinished = onFinished;
			_title = title;
		}

        public void Dispose()
        {
            if (!_isFinished)
            {
                var timeElapsed = DateTime.Now - _startTime;
                _onFinished(_title, timeElapsed);
                _isFinished = true;
            }
        }

        public static Scope Start(string title, Action<string, TimeSpan> onFinished)
        {
#if DEBUG
            return new Scope(DateTime.Now, onFinished, title);
#else
            return NullDisposable.Instance;
#endif
        }
    }
}
