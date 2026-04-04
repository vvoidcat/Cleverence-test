namespace Compression.Factory;

public static class CompressorFactory
{
	public static ICompressor Create() => Compressor.Create();
}
