namespace Compression;

public interface ICompressor
{
	public string Compress(string input, bool caseSensitive = false);
	public string Decompress(string compressedString);
}
