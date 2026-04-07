using LogFormatter.Entities;

namespace LogFormatter.Tests;

public class NormalizedEntryTest
{
	[Theory]
	[InlineData("2024-01-15", "14:30:45.1", "INFO", "UserService", "User logged in", "15-01-2024", "14:30:45.1")]
	[InlineData("2024-12-25", "08:15:30.22", "INFO", "CacheService", "High memory usage", "25-12-2024", "08:15:30.22")]
	[InlineData("2024-03-10", "23:45:12.333", "INFO", "PaymentGateway", "Transaction failed", "10-03-2024", "23:45:12.333")]
	[InlineData("2024-07-04", "12:00:00.4444", "INFO", "LoggerService", "Debug message", "04-07-2024", "12:00:00.4444")]
	[InlineData("2024-11-30", "19:20:33.555678", "INFO", "EmailService", "Email sent", "30-11-2024", "19:20:33.555678")]
	public void CheckToString
	(
		string dateString,
		string timeString,
		string logLevel,
		string callingMethod,
		string message,
		string expectedDatePart,
		string expectedTimePart
	)
	{
		var date = DateTime.Parse($"{dateString} {timeString}");
		var initialEntry = new Entry(0, "content", "test.log");

		var entry = new NormalizedEntry(
			initialEntry,
			date,
			LogLevelType.INFO,
			callingMethod,
			message
		);

		var result = entry.ToString();

		var expectedPrefix = $"{expectedDatePart}\t{expectedTimePart}\t";
		Assert.Contains(expectedPrefix, result);
		Assert.Contains(logLevel.ToString(), result);
		Assert.Contains(callingMethod, result);
		Assert.Contains(message, result);

		var parts = result.Split('\t');
		Assert.Equal(5, parts.Length);
	}
}
