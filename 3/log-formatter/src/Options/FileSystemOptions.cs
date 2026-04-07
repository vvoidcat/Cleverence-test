namespace LogFormatter.Options;

internal class FileSystemOptions : IDataProviderOptions
{
	public string SourceDirectory { get; set; } = String.Empty;
	public string DestinationLogsDirectory { get; set; } = String.Empty;
	public string DestinationErrorLogs { get; set; } = String.Empty;

	public void Validate()
	{
		if (string.IsNullOrWhiteSpace(SourceDirectory))
			throw new InvalidOperationException("не указан путь до исходной директории");

		if (string.IsNullOrWhiteSpace(DestinationLogsDirectory))
			throw new InvalidOperationException("не указан путь до конечной папки для нормализованных логов");

		if (string.IsNullOrWhiteSpace(DestinationErrorLogs))
			throw new InvalidOperationException("не указан путь до конечного файла для логов с ошибками");
	}
}
