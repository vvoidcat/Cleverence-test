namespace LogFormatter.Entities;

internal readonly struct NormalizedEntry
{
	public Entry InitialEntry { get; init; }

	public DateTime Date { get; init; }
	public LogLevelType LogLevel { get; init; }
	public string CallingMethod { get; init; }
	public string Message { get; init; }

	public NormalizedEntry
	(
		Entry initialEntry,
		DateTime date,
		LogLevelType logLevel,
		string callingMethod,
		string message
	)
	{
		InitialEntry = initialEntry;
		Date = date;
		LogLevel = logLevel;
		CallingMethod = callingMethod;
		Message = message;
	}

	public override string ToString() =>
		$"{Date.ToString("dd-MM-yyyy")}\t" +
		$"{Date.ToString(@"HH\:mm\:ss\.fffffff").TrimEnd('0').TrimEnd('.')}\t" +
		$"{LogLevel}\t" +
		$"{CallingMethod}\t" +
		$"{Message}";
}
