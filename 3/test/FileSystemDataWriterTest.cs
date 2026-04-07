using LogFormatter.Providers.FileSystem;
using LogFormatter.Entities;

namespace LogFormatter.Tests;

public class FileSystemDataWriterTest
{

	[Fact]
	public void CheckCreate()
	{
		var testDir = "somedir";
		var testErrFile = "some_problems.txt";

		var writer = FileSystemDataWriter.Create(testDir, testErrFile);

		Assert.NotNull(writer);
		Assert.IsType<FileSystemDataWriter>(writer);
	}

	[Fact]
	public async Task CheckTryWriteWithUnassignedDir_Throws()
	{
		var testDir = "";
		var testErrFile = "some_problems.txt";
		var entries = Array.Empty<NormalizedEntry>();

		var writer = FileSystemDataWriter.Create(testDir, testErrFile);

		var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => writer.WriteData(entries));
		Assert.Contains("не указан путь до конечной директории", ex.Message);
	}

	[Fact]
	public async Task CheckTryWriteCreatesNewDir()
	{
		var testDir = $"{nameof(CheckTryWriteCreatesNewDir)}_test";

		var entries = new NormalizedEntry[]
		{
			new NormalizedEntry(new Entry(0, "0", "testpath"), DateTime.Now, LogLevelType.NONE, "def", "msg"),
			new NormalizedEntry(new Entry(1, "1", "testpath"), DateTime.Now, LogLevelType.NONE, "def", "msg"),
			new NormalizedEntry(new Entry(2, "2", "testpath"), DateTime.Now, LogLevelType.NONE, "def", "msg"),
		};

		var writer = FileSystemDataWriter.Create(testDir, "");
		await writer.WriteData(entries);

		Assert.True(Directory.Exists(testDir));

		Directory.Delete(testDir, true);
	}

	[Fact]
	public async Task CheckTryWrite()
	{
		var testDir = $"{nameof(CheckTryWrite)}_test";
		var testErrFile = "";
		var file = $"{nameof(CheckTryWrite)}_test.txt";
		var outFile = $"norm_{file}";

		var entries = new NormalizedEntry[]
		{
			new NormalizedEntry(new Entry(0, "0", file), DateTime.Now, LogLevelType.NONE, "def", "msg"),
			new NormalizedEntry(new Entry(1, "1", file), DateTime.Now, LogLevelType.NONE, "def", "msg"),
			new NormalizedEntry(new Entry(2, "2", file), DateTime.Now, LogLevelType.NONE, "def", "msg"),
		};

		string expectedContent = string.Join('\n', entries.Select(x => x.ToString()));

		var writer = FileSystemDataWriter.Create(testDir, testErrFile);
		await writer.WriteData(entries);

		var writtenStr = await File.ReadAllTextAsync(Path.Combine(testDir, outFile));

		Assert.Equal(expectedContent, writtenStr);

		Directory.Delete(testDir, true);
	}

	[Fact]
	public async Task CheckTryWriteEmptyCollection()
	{
		var testDir = $"{nameof(CheckTryWriteEmptyCollection)}_test";
		var testErrFile = $"{nameof(CheckTryWriteEmptyCollection)}_test";
		var entries = Array.Empty<NormalizedEntry>();

		var writer = FileSystemDataWriter.Create(testDir, testErrFile);
		await writer.WriteData(entries);

		Assert.False(File.Exists(Path.Combine(testDir, testErrFile)));
		Assert.False(Directory.Exists(testDir));
	}

	[Fact]
	public async Task CheckTryWriteErrorsWithUnassignedDir_Throws()
	{
		var entries = Array.Empty<Entry>();
		var testDir = "";
		var testErrFile = "some_problems.txt";

		var writer = FileSystemDataWriter.Create(testDir, testErrFile);

		var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => writer.WriteErrorData(entries));
		Assert.Contains("не указан путь до конечной директории", ex.Message);
	}

	[Fact]
	public async Task CheckTryWriteErrorsWithUnassignedDestFile_Throws()
	{
		var entries = Array.Empty<Entry>();
		var testDir = $"{nameof(CheckTryWriteErrorsWithUnassignedDestFile_Throws)}_test";
		var testErrFile = "";

		var writer = FileSystemDataWriter.Create(testDir, testErrFile);

		var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => writer.WriteErrorData(entries));
		Assert.Equal("не указан путь до конечного файла для записи ошибок", ex.Message);
		Assert.False(Directory.Exists(testDir));
	}

	[Fact]
	public async Task CheckTryWriteErrorsCreatesNewDir()
	{
		var testDir = $"{nameof(CheckTryWriteErrorsCreatesNewDir)}_test";
		var testErrFile = "some_problems";

		var entries = new Entry[]
		{
			new Entry(0, "0", "testpath"),
			new Entry(1, "1", "testpath"),
			new Entry(2, "2", "testpath"),
		};

		var writer = FileSystemDataWriter.Create(testDir, testErrFile);
		await writer.WriteErrorData(entries);

		Assert.True(Directory.Exists(testDir));

		Directory.Delete(testDir, true);
	}

	[Fact]
	public async Task CheckTryWriteErrorsEmptyCollection()
	{
		var testDir = $"{nameof(CheckTryWriteErrorsEmptyCollection)}_test";
		var testErrFile = $"{nameof(CheckTryWriteErrorsEmptyCollection)}_test";
		var entries = Array.Empty<Entry>();

		var writer = FileSystemDataWriter.Create(testDir, testErrFile);
		await writer.WriteErrorData(entries);

		Assert.False(File.Exists(Path.Combine(testDir, testErrFile)));
		Assert.False(Directory.Exists(testDir));
	}

	[Fact]
	public async Task CheckTryWriteErrors()
	{
		var testDir = $"{nameof(CheckTryWrite)}_test";
		var testErrFile = $"{nameof(CheckTryWrite)}_problems.txt";
		var file = $"{nameof(CheckTryWrite)}_test.txt";

		var entries = new Entry[]
		{
			new Entry(0, "line_0", file),
			new Entry(1, "line_1", file),
			new Entry(2, "line_2", file),
		};

		string expectedContent = string.Join('\n', entries.Select(x => x.Content));

		var writer = FileSystemDataWriter.Create(testDir, testErrFile);
		await writer.WriteErrorData(entries);

		var writtenStr = await File.ReadAllTextAsync(Path.Combine(testDir, testErrFile));

		Assert.Equal(expectedContent, writtenStr);

		Directory.Delete(testDir, true);
	}
}
