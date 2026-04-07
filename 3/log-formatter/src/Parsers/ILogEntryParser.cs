using LogFormatter.Entities;

namespace LogFormatter.Parsers;

internal interface ILogEntryParser
{
	public NormalizedEntry Parse(Entry entry);
}
