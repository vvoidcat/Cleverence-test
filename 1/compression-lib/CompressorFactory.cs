namespace Compression;

public static class CompressorFactory
{
    public static ICompressor Build() => Compressor.Create();
}
