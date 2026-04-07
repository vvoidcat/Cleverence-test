using LogFormatter.Options;
using LogFormatter.Providers.FileSystem;

namespace LogFormatter.Providers.Factories;

internal class DataWriterFactory
{
	public static IDataWriter Create(IDataProviderOptions options)
	{
		return options switch
		{
			FileSystemOptions fs => FileSystemDataWriter.Create(fs.DestinationLogsDirectory, fs.DestinationErrorLogs),
			_ => throw new ArgumentException($"{nameof(DataWriterFactory)}: используется неподдерживаемый тип поставщика данных")
		};
	}
}
