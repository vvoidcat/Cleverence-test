using LogFormatter.Entities;

namespace LogFormatter.Providers;

internal interface IDataReader
{
	public Task<Entry[]> GetData(CancellationToken cancellationToken = default);
}
