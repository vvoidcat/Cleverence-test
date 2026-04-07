using System.Collections.Concurrent;
using LogFormatter.Entities;

namespace LogFormatter.Providers.FileSystem;

internal class FileSystemDataReader : IDataReader
{
	private readonly string _sourceDir = String.Empty;

	private FileSystemDataReader(string sourceDir)
	{
		_sourceDir = sourceDir;
	}

	public static IDataReader Create(string sourceDir) => new FileSystemDataReader(sourceDir);

	#region IDataReader

	public async Task<Entry[]> GetData(CancellationToken cancellationToken = default)
	{
		if (!Directory.Exists(_sourceDir))
			throw new InvalidOperationException($"указан несуществующий путь до исходной директории: {_sourceDir}");

		var files = Array.Empty<string>();
		var contents = new ConcurrentBag<Entry>();

		try
		{
			files = Directory.GetFiles(_sourceDir, "*.txt");
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"ошибка при попытке доступа к директории {_sourceDir}: " + ex.Message);
		}

		await Parallel.ForEachAsync
		(
			files,
			new ParallelOptions
			{
				MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1),
				CancellationToken = cancellationToken
			},
			async (filePath, token) =>
			{
				var entries = await File.ReadAllLinesAsync(filePath, token);

				for (int i = 0; i < entries.Length; i++)
				{
					token.ThrowIfCancellationRequested();

					var entry = entries[i];

					if (!String.IsNullOrWhiteSpace(entry))
					{
						contents.Add(new Entry(i, entry, Path.GetFileName(filePath)));
					}
				}
			}
		);

		return contents.ToArray();
	}

	#endregion
}
