namespace LogFormatter.Entities;

internal readonly struct Entry
{
	public int Id { get; init; }
	public string Content { get; init; }
	public string FileName { get; init; }

	public Entry(int id, string content, string fileName)
	{
		Id = id;
		Content = content;
		FileName = fileName;
	}
}
