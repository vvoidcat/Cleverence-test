using System.Collections.Concurrent;
using LogFormatter.Providers;
using LogFormatter.Parsers;
using LogFormatter.Entities;

namespace LogFormatter.Services;

internal class LogNormalizerService
{
	private readonly IDataReader _dataReader;
	private readonly IDataWriter _dataWriter;
	private readonly ILogEntryParser _dataParser;

	public LogNormalizerService
	(
		IDataReader dataReader,
		IDataWriter dataWriter,
		ILogEntryParser dataParser
	)
	{
		_dataReader = dataReader;
		_dataWriter = dataWriter;
		_dataParser = dataParser;
	}

	public async Task RunAsync(CancellationToken cancellationToken = default)
	{
		var logs = await _dataReader.GetData(cancellationToken);

		Console.WriteLine($"Обнаружено файлов в исходной директории: {logs.GroupBy(x => x.FileName).Count()}");
		Console.WriteLine($"Обнаружено записей: {logs.Length}\n");

		var result = new ConcurrentBag<NormalizedEntry>();
		var resultErr = new ConcurrentBag<Entry>();

		Parallel.ForEach(logs, new ParallelOptions
		{
			MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1),
			CancellationToken = cancellationToken
		}, (log) =>
		{
			try
			{
				var normalizedLog = _dataParser.Parse(log);
				result.Add(normalizedLog);
			}
			catch (Exception ex)
			{
				resultErr.Add(log);
				Console.WriteLine($"{log.FileName}, строка {log.Id + 1}: " + ex.Message);
			}
		});

		await _dataWriter.WriteData(result.ToArray(), cancellationToken);
		await _dataWriter.WriteErrorData(resultErr.ToArray(), cancellationToken);

		Console.WriteLine("-----------------------------------------------------------------");
		Console.WriteLine($"Всего обработано записей логов: {result.Count + resultErr.Count}\n");
		Console.WriteLine($"Успешно нормализованные записи: {result.Count}");
		Console.WriteLine($"Записи с ошибками: {resultErr.Count}");
	}
}
