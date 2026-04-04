namespace Compression.Test;

public class DecompressUnitTest : IClassFixture<CompressorFixture>
{
    private readonly ICompressor _compressor;

    public DecompressUnitTest(CompressorFixture fixture)
    {
        _compressor = fixture.Compressor;
    }

    [Theory]
    [InlineData("", "Входящая строка не содержит символов")]
    [InlineData(null, "Входящая строка не содержит символов")]
    [InlineData("a   dd", "Входящая строка может содержать только буквы латинского алфавита или цифры")]
    [InlineData("a-A", "Входящая строка может содержать только буквы латинского алфавита или цифры")]
    [InlineData("m11111'lsldl", "Входящая строка может содержать только буквы латинского алфавита или цифры")]
    [InlineData(" ", "Входящая строка может содержать только буквы латинского алфавита или цифры")]
    [InlineData("1a", "Входящая строка имеет неверный формат и не может быть декодирована")]
    [InlineData("10", "Входящая строка имеет неверный формат и не может быть декодирована")]
    [InlineData("10a10", "Входящая строка имеет неверный формат и не может быть декодирована")]
    [InlineData("a0", "Входящая строка имеет неверный формат и не может быть декодирована")]
    [InlineData("a000", "Входящая строка имеет неверный формат и не может быть декодирована")]
    [InlineData("a2b0f40", "Входящая строка имеет неверный формат и не может быть декодирована")]
    [InlineData("a2b9f0", "Входящая строка имеет неверный формат и не может быть декодирована")]
    public void CheckThrowingErrors(string? input, string expectedErrMessage)
    {
        var ex = Assert.Throws<ArgumentException>(() => _compressor.Decompress(input!));
        Assert.Equal(expectedErrMessage, ex.Message);
    }

    [Theory]
    [InlineData("a", "a")]
    [InlineData("A", "A")]
    [InlineData("a1", "a")]
    [InlineData("A1", "A")]
    [InlineData("a4", "aaaa")]
    [InlineData("A4", "AAAA")]
    [InlineData("a2b3", "aabbb")]
    [InlineData("a2A2", "aaAA")]
    [InlineData("a4b2c3f10", "aaaabbcccffffffffff")]
    [InlineData("a4b2c1C2f10", "aaaabbcCCffffffffff")]
    [InlineData("abcd", "abcd")]
    [InlineData("aaaa", "aaaa")]
    [InlineData("aaAAaaAA", "aaAAaaAA")]
    [InlineData("aaaa1", "aaaa")]
    [InlineData("bb3aa", "bbbbaa")]
    [InlineData("bbc2aa", "bbccaa")]
    [InlineData("a4b2cC2f10", "aaaabbcCCffffffffff")]
    [InlineData("somestringthatshouldnotrequiredecompression", "somestringthatshouldnotrequiredecompression")]
    public void CheckDecompressionResult(string input, string expectedResult)
    {
        var result = _compressor.Decompress(input);
        Assert.Equal(expectedResult, result);
    }
}
