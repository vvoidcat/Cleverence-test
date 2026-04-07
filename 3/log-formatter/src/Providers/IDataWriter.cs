using LogFormatter.Entities;

namespace LogFormatter.Providers;

internal interface IDataWriter
{
	public Task WriteData(NormalizedEntry[] logEntries, CancellationToken cancellationToken = default);
	public Task WriteErrorData(Entry[] errorLogEntries, CancellationToken cancellationToken = default);
}
