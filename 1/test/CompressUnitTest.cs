namespace Compression.Test;

public class CompressUnitTest : IClassFixture<CompressorFixture>
{
    private readonly ICompressor _compressor;

    public CompressUnitTest(CompressorFixture fixture)
    {
        _compressor = fixture.Compressor;
    }

    [Theory]
    [InlineData("", "Входящая строка не содержит символов")]
    [InlineData(null, "Входящая строка не содержит символов")]
    [InlineData("a   dd", "Входящая строка может содержать только буквы латинского алфавита")]
    [InlineData("аппмломсджавалаввылд", "Входящая строка может содержать только буквы латинского алфавита")]
    [InlineData("a-A", "Входящая строка может содержать только буквы латинского алфавита")]
    [InlineData("m11111", "Входящая строка может содержать только буквы латинского алфавита")]
    [InlineData(" ", "Входящая строка может содержать только буквы латинского алфавита")]
    public void CheckThrowingErrors(string? input, string expectedErrMessage)
    {
        var ex = Assert.Throws<ArgumentException>(() => _compressor.Compress(input!));
        Assert.Equal(expectedErrMessage, ex.Message);
    }

    [Theory]
    [InlineData ("a", "a", false)] 
    [InlineData ("A", "a", false)]
    [InlineData ("A", "A", true)]
    [InlineData ("AA", "a2", false)]
    [InlineData ("aaaaa", "a5", false)]
    [InlineData ("aaaaaaaaaa", "a10", false)]
    [InlineData ("aaAAA", "a5", false)]
    [InlineData ("aaAAA", "a2A3", true)]
    [InlineData ("aabbbccffffffffff", "a2b3c2f10", true)]
    [InlineData ("aabbbccCCCffffffffff", "a2b3c2C3f10", true)]
    [InlineData ("aabbbccCCCffffffffff", "a2b3c5f10", false)]
    [InlineData ("abcd", "abcd", false)]
    [InlineData ("aabcd", "a2bcd", false)]
    [InlineData("aabbbccCCCf", "a2b3c5f", false)]
    public void CheckCompressionResult(string input, string expectedResult, bool caseSensitive)
    {
        var result = _compressor.Compress(input, caseSensitive);
        Assert.Equal(expectedResult, result);
    }
}
