using LogFormatter.Options;

namespace LogFormatter.Tests;

public class FileSystemOptionsTest
{
	[Fact]
	public void CheckDefaultValues()
	{
		var options = new FileSystemOptions();

		Assert.Equal(String.Empty, options.SourceDirectory);
		Assert.Equal(String.Empty, options.DestinationLogsDirectory);
		Assert.Equal(String.Empty, options.DestinationErrorLogs);
	}

	[Theory]
	[InlineData("", "", "", "не указан путь до исходной директории")]
	[InlineData("", "someval", "someval", "не указан путь до исходной директории")]
	[InlineData(" ", "someval", "someval", "не указан путь до исходной директории")]
	[InlineData(null, "someval", "someval", "не указан путь до исходной директории")]
	[InlineData("someval", "", "someval", "не указан путь до конечной папки для нормализованных логов")]
	[InlineData("someval", " ", "someval", "не указан путь до конечной папки для нормализованных логов")]
	[InlineData("someval", null, "someval", "не указан путь до конечной папки для нормализованных логов")]
	[InlineData("someval", "someval", "", "не указан путь до конечного файла для логов с ошибками")]
	[InlineData("someval", "someval", " ", "не указан путь до конечного файла для логов с ошибками")]
	[InlineData("someval", "someval", null, "не указан путь до конечного файла для логов с ошибками")]
	public void CheckThrowsOnFailedValidation(string? src, string? dest, string? destErr, string expectedException)
	{
		var options = new FileSystemOptions
		{
			SourceDirectory = src!,
			DestinationLogsDirectory = dest!,
			DestinationErrorLogs = destErr!
		};

		var ex = Assert.Throws<InvalidOperationException>(() => options.Validate());
		Assert.Equal(expectedException, ex.Message);
	}
}
