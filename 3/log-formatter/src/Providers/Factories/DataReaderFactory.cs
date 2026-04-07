using LogFormatter.Options;
using LogFormatter.Providers.FileSystem;

namespace LogFormatter.Providers.Factories;

internal class DataReaderFactory
{
	public static IDataReader Create(IDataProviderOptions options)
	{
		return options switch
		{
			FileSystemOptions fs => FileSystemDataReader.Create(fs.SourceDirectory),
			_ => throw new ArgumentException($"{nameof(DataReaderFactory)}: используется неподдерживаемый тип поставщика данных")
		};
	}
}
