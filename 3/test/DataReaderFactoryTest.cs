using LogFormatter.Options;
using LogFormatter.Providers.FileSystem;
using LogFormatter.Providers.Factories;
using LogFormatter.Tests.MockClasses;

namespace LogFormatter.Tests;

public class DataReaderFactoryTest
{
	[Fact]
	public void CheckWithUnsupportedOptions()
	{
		var options = new UnsupportedDataProviderOptions();

		var exception = Assert.Throws<ArgumentException>(
			() => DataReaderFactory.Create(options)
		);

		Assert.Contains("неподдерживаемый тип поставщика данных", exception.Message);
	}

	[Fact]
	public void CheckReturnsNewInstances()
	{
		var options = new FileSystemOptions();

		var result1 = DataReaderFactory.Create(options);
		var result2 = DataReaderFactory.Create(options);

		Assert.NotSame(result1, result2);
		Assert.IsType<FileSystemDataReader>(result1);
		Assert.IsType<FileSystemDataReader>(result2);
	}
}
