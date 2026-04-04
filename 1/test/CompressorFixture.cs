namespace Compression.Test;

public class CompressorFixture
{
    public ICompressor Compressor { get; private set; }

    public CompressorFixture()
    {
        Compressor = CompressorFactory.Build();
    }
}
