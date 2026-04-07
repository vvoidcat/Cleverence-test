using System.Text.RegularExpressions;
using LogFormatter.Entities;

namespace LogFormatter.Parsers.LogEntry;

internal class LogEntryParser : ILogEntryParser
{
	private LogEntryParser() { }
	public static ILogEntryParser Create() => new LogEntryParser();

	public NormalizedEntry Parse(Entry entry)
	{
		var formatSpacesMatch = Regex.Match(entry.Content, IncomingFormat.Spaces);
		var formatVBarsMatch = Regex.Match(entry.Content, IncomingFormat.VerticalBars);

		if (formatSpacesMatch.Success)
		{
			return ParseData(entry, formatSpacesMatch);
		}
		else if (formatVBarsMatch.Success)
		{
			return ParseData(entry, formatVBarsMatch);
		}
		else
		{
			throw new InvalidOperationException("обнаружен неизвестный формат записи");
		}
	}

	private NormalizedEntry ParseData(Entry entry, Match format)
	{
		var date = ParseDate(format.Groups["date"].Value);
		var logLevel = ParseLogLevel(format.Groups["level"].Value);
		var callingMethod = ParseCallingMethod(format.Groups["method"].Value);
		var message = format.Groups["message"].Value.Trim();

		return new NormalizedEntry(entry, date, logLevel, callingMethod, message);
	}

	private DateTime ParseDate(string dateValue) =>
		DateTime.Parse(dateValue);

	private LogLevelType ParseLogLevel(string level) =>
		level.ToUpper() switch
		{
			"INFORMATION" or "INFO" => LogLevelType.INFO,
			"WARNING" or "WARN" => LogLevelType.WARN,
			"ERROR" => LogLevelType.ERROR,
			"DEBUG" => LogLevelType.DEBUG,
			_ => LogLevelType.NONE
		};

	private string ParseCallingMethod(string methodValue) =>
		string.IsNullOrWhiteSpace(methodValue) ? "DEFAULT" : methodValue;
}
