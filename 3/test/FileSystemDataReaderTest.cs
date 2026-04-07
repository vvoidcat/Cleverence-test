using LogFormatter.Providers.FileSystem;

namespace LogFormatter.Tests;

public class FileSystemDataReaderTest
{
	private static string _testDir => "FileSamples";

	public FileSystemDataReaderTest()
	{
		if (!Directory.Exists(_testDir))
		{
			Directory.CreateDirectory(_testDir);
		}
	}

	[Fact]
	public void CheckCreate()
	{
		var reader = FileSystemDataReader.Create(_testDir);

		Assert.NotNull(reader);
		Assert.IsType<FileSystemDataReader>(reader);
	}

	[Fact]
	public async Task CheckTryReadWithUnassignedSourceDir_Throws()
	{
		var reader = FileSystemDataReader.Create("");

		var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => reader.GetData());
		Assert.Contains("указан несуществующий путь до исходной директории", ex.Message);
	}

	[Fact]
	public async Task CheckTryRead()
	{
		string mockContent = "first\r\nsecond\r\nthird\r\n";
		string[] mockLines = { "first", "second", "third" };
		var file = $"{nameof(CheckTryRead)}_test.txt";
		var path = Path.Combine(_testDir, file);

		await File.WriteAllTextAsync(path, mockContent);

		var reader = FileSystemDataReader.Create(_testDir);

		var entries = await reader.GetData();
		var fileWasRead = entries.Any(x => x.FileName == file);
		var entriesIntersectionCount = entries.Where(x => x.FileName == file).Select(x => x.Content).Intersect(mockLines);

		Assert.True(entries.Length > 0);
		Assert.True(fileWasRead);
		Assert.True(entriesIntersectionCount.Count() == mockLines.Length);

		File.Delete(path);
	}

	[Theory]
	[InlineData("", 0)]
	[InlineData(" \r\n", 0)]
	[InlineData("\r\n\r\n\r\n\r\n", 0)]
	[InlineData("\t\t\t\t", 0)]
	public async Task CheckTryReadEmptyFile(string mockContent, int expectedEntryCount)
	{
		var file = $"{nameof(CheckTryReadEmptyFile)}_test.txt";
		var path = Path.Combine(_testDir, file);

		await File.WriteAllTextAsync(path, mockContent);

		var reader = FileSystemDataReader.Create(_testDir);

		var entries = await reader.GetData();

		Assert.Equal(expectedEntryCount, entries.Where(x => x.FileName == file).ToArray().Length);

		File.Delete(path);
	}
}
