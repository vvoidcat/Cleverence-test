namespace LogFormatter.Entities;

internal static class IncomingFormat
{
	public static string Spaces =>
		@"^\s*(?<date>\d{2}\.\d{2}\.\d{4}\s+\d{2}:\d{2}:\d{2}\.\d+)\s+(?<level>INFORMATION|INFO|WARNING|WARN|ERROR|DEBUG)\s+(?<message>.+)";
	public static string VerticalBars =>
		@"^\s*(?<date>\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2}\.\d+)\s*\|\s*(?<level>INFORMATION|INFO|WARNING|WARN|ERROR|DEBUG)\s*\|\s*\d+\s*\|\s*(?<method>.+?)\s*\|\s*(?<message>.+)";
}
