using LogFormatter.Entities;
using LogFormatter.Parsers.LogEntry;
using LogFormatter.Parsers;

namespace LogFormatter.Tests;

public class LogEntryParserTest
{
	private readonly ILogEntryParser _parser;

	public LogEntryParserTest()
	{
		_parser = LogEntryParser.Create();
	}

	[Theory]
	[InlineData("10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'", "10.03.2025 15:14:49.523", "INFO", "DEFAULT", "Версия программы: '3.4.0.48729'")]
	[InlineData("15.01.2026 14:30:45.00 INFO Authenticated user", "15.01.2026 14:30:45.00", "INFO", "DEFAULT", "Authenticated user")]
	[InlineData("25.12.2026    08:15:30.11    WARNING  Cache   ", "25.12.2026 08:15:30.11", "WARN", "DEFAULT", "Cache")]
	[InlineData("10.03.2026  23:45:12.222 ERROR Timeout", "10.03.2026 23:45:12.222", "ERROR", "DEFAULT", "Timeout")]
	[InlineData("04.07.2026 12:00:00.3333   DEBUG Log", "04.07.2026 12:00:00.3333", "DEBUG", "DEFAULT", "Log")]
	[InlineData("30.11.2026 19:20:33.4   INFORMATION     Email", "30.11.2026 19:20:33.4", "INFO", "DEFAULT", "Email")]
	[InlineData("01.05.2026 06:00:00.55555 WARNING   Job", "01.05.2026 06:00:00.55555", "WARN", "DEFAULT", "Job")]
	[InlineData("    18.09.2026 10:10:10.666666  INFORMATION  message", "18.09.2026 10:10:10.666666", "INFO", "DEFAULT", "message")]
	public void CheckSpacesFormat_ValidEntries
	(
		string content,
		string expectedDateString,
		string expectedLogLevelText,
		string expectedMethod,
		string expectedMessage
	)
	{
		var entry = new Entry { Content = content };
		var expectedDate = DateTime.Parse(expectedDateString);

		var result = _parser.Parse(entry);

		Assert.Equal(expectedDate, result.Date);
		Assert.Equal(expectedLogLevelText, result.LogLevel.ToString());
		Assert.Equal(expectedMethod, result.CallingMethod);
		Assert.Equal(expectedMessage, result.Message);
	}

	[Theory]
	[InlineData("2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'", "2025-03-10 15:14:51.5882", "INFO", "MobileComputer.GetDeviceId", "Код устройства: '@MINDEO-M40-D-410244015546'")]
	[InlineData("      2024-01-15 14:30:45.1|INFO|101010|UserService|msg1", "2024-01-15 14:30:45.1", "INFO", "UserService", "msg1")]
	[InlineData("2024-12-25 08:15:30.22  |  WARNING |  00001  |  CacheService |   msg2", "2024-12-25 08:15:30.22", "WARN", "CacheService", "msg2")]
	[InlineData("2024-03-10 23:45:12.333|ERROR|123456789|PaymentGateway|msg3      ", "2024-03-10 23:45:12.333", "ERROR", "PaymentGateway", "msg3")]
	[InlineData(" 2024-07-04 12:00:00.4444 |DEBUG |1 |LoggerService |msg4", "2024-07-04 12:00:00.4444", "DEBUG", "LoggerService", "msg4")]
	[InlineData("2024-11-30 19:20:33.55555| INFORMATION| 2| EmailService| msg5", "2024-11-30 19:20:33.55555", "INFO", "EmailService", "msg5")]
	[InlineData("2024-05-01     06:00:00.666666|WARN|0000|SchedulerService|msg6", "2024-05-01 06:00:00.666666", "WARN", "SchedulerService", "msg6")]
	public void CheckVerticalBarsFormat_ValidEntries
	(
		string content,
		string expectedDateString,
		string expectedLogLevelText,
		string expectedMethod,
		string expectedMessage
	)
	{
		var entry = new Entry { Content = content };
		var expectedDate = DateTime.Parse(expectedDateString);

		var result = _parser.Parse(entry);

		Assert.Equal(expectedDate, result.Date);
		Assert.Equal(expectedLogLevelText, result.LogLevel.ToString());
		Assert.Equal(expectedMethod, result.CallingMethod);
		Assert.Equal(expectedMessage, result.Message);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData("INFO aboba Test bebra")]
	[InlineData("INFO Test message")]
	[InlineData("2024/01/15 10:30:45.0 INFO Test")]
	[InlineData("2024.01.15:::::10:30:45.0000 INFO Test")]
	[InlineData("15-01-2024 10:30:45.0 INFO Test")]
	[InlineData("2024-01-15 10:30:45.0")]
	[InlineData("2024.01.15 10:30:45 INFO Test")]
	[InlineData("2024-01-15|10:30:45 INFO Test")]
	[InlineData("randomtext")]
	[InlineData("2024-01-15 10:30:45|INFO|Test|Test")]
	[InlineData("10-03-2025 15:14:49.523 UNKNOWN Версия программы: '3.4.0.48729'")]
	[InlineData("2025.03.10 15:14:51.5882| UNKNOWN|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'")]
	public void CheckUnknownFormat_Trows(string? content)
	{
		var entry = new Entry { Content = content! };

		var ex = Assert.Throws<InvalidOperationException>(() => _parser.Parse(entry));
		Assert.Equal("обнаружен неизвестный формат записи", ex.Message);
	}

	[Theory]
	[InlineData("15.01.2024 10:30:45.000 UNKNOWN Test message")]
	[InlineData("15.01.2024 10:30:45.000 CRITICAL Test message")]
	[InlineData("15.01.2024 10:30:45.000 VERBOSE Test message")]
	[InlineData("2024-01-15 10:30:45.000|UNKNOWN|TestMethod|Test message")]
	[InlineData("2024-01-15 10:30:45.000|CRITICAL|TestMethod|Test message")]
	[InlineData("2024-01-15 10:30:45.000|VERBOSE|TestMethod|Test message")]
	public void CheckWithUnsupportedLogLevel_Throws(string content)
	{
		var entry = new Entry { Content = content };

		var ex = Assert.Throws<InvalidOperationException>(() => _parser.Parse(entry));
		Assert.Equal("обнаружен неизвестный формат записи", ex.Message);
	}
}
