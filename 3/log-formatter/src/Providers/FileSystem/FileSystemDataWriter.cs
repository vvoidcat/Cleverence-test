using LogFormatter.Entities;

namespace LogFormatter.Providers.FileSystem;

internal class FileSystemDataWriter : IDataWriter
{
	private readonly string _destDir = String.Empty;
	private readonly string _destErrFile = String.Empty;

	private FileSystemDataWriter(string destDir, string destErrFile)
	{
		_destDir = destDir;
		_destErrFile = destErrFile;
	}

	public static IDataWriter Create(string destDir, string destErrFile) =>
		new FileSystemDataWriter(destDir, destErrFile);

	#region IDataWriter

	public async Task WriteData(NormalizedEntry[] entries, CancellationToken cancellationToken = default)
	{
		if (String.IsNullOrWhiteSpace(_destDir))
			throw new InvalidOperationException($"{nameof(WriteData)}: не указан путь до конечной директории");

		if (entries.Length == 0)
			return;

		var prefix = "norm_";

		CreateDirectoryIfNotExists(_destDir);

		await Parallel.ForEachAsync
		(
			entries.GroupBy(x => x.InitialEntry.FileName),
			new ParallelOptions
			{
				MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1),
				CancellationToken = cancellationToken
			},
			async (fileGroup, token) =>
			{
				var content = String.Join('\n', fileGroup.OrderBy(x => x.InitialEntry.Id).Select(x => x.ToString()));
				var path = Path.Combine(_destDir, prefix + fileGroup.Key);

				await File.WriteAllTextAsync(path, content, token);
			}
		);
	}

	public async Task WriteErrorData(Entry[] errorEntries, CancellationToken cancellationToken = default)
	{
		if (String.IsNullOrWhiteSpace(_destDir))
			throw new InvalidOperationException($"{nameof(WriteErrorData)}: не указан путь до конечной директории");
		if (String.IsNullOrWhiteSpace(_destErrFile))
			throw new InvalidOperationException("не указан путь до конечного файла для записи ошибок");

		if (errorEntries.Length == 0)
			return;

		CreateDirectoryIfNotExists(_destDir);

		var path = Path.Combine(_destDir, _destErrFile);
		var content = String.Join('\n', errorEntries.OrderBy(x => x.FileName).ThenBy(x => x.Id).Select(x => x.Content));

		await File.WriteAllTextAsync(path, content, cancellationToken);
	}

	#endregion

	private void CreateDirectoryIfNotExists(string dir)
	{
		if (!Directory.Exists(dir))
		{
			try
			{
				Directory.CreateDirectory(dir);
				Console.WriteLine($"Создана новая директория по пути: {dir}");
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"ошибка при попытке создать новую директорию по пути {dir}: " + ex.Message);
			}
		}
	}
}
