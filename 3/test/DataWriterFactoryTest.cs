using LogFormatter.Options;
using LogFormatter.Providers.Factories;
using LogFormatter.Providers.FileSystem;
using LogFormatter.Tests.MockClasses;

namespace LogFormatter.Tests;

public class DataWriterFactoryTest
{
	[Fact]
	public void CheckWithUnsupportedOptions()
	{
		var options = new UnsupportedDataProviderOptions();

		var exception = Assert.Throws<ArgumentException>(
			() => DataWriterFactory.Create(options)
		);

		Assert.Contains("неподдерживаемый тип поставщика данных", exception.Message);
	}

	[Fact]
	public void CheckReturnsNewInstances()
	{
		var options = new FileSystemOptions();

		var result1 = DataWriterFactory.Create(options);
		var result2 = DataWriterFactory.Create(options);

		Assert.NotSame(result1, result2);
		Assert.IsType<FileSystemDataWriter>(result1);
		Assert.IsType<FileSystemDataWriter>(result2);
	}
}
