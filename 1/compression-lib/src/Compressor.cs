using System.Text;
using System.Text.RegularExpressions;

namespace Compression;

internal class Compressor : ICompressor
{
	private Compressor() { }
	public static ICompressor Create() => new Compressor();

	#region ICompressor

	public string Compress(string input, bool caseSensitive = false)
	{
		if (String.IsNullOrEmpty(input))
			throw new ArgumentException("Входящая строка не содержит символов");

		if (input.Any(x => !char.IsAsciiLetter(x)))
			throw new ArgumentException("Входящая строка может содержать только буквы латинского алфавита");

		var str = caseSensitive ? input : input.ToLower();
		var strBuilder = new StringBuilder();

		var count = 0;

		for (int i = 0; i < str.Length; i++)
		{
			if (count == 0)
			{
				strBuilder.Append(str[i]);
			}

			count++;

			if (i + 1 == str.Length || str[i] != str[i + 1])
			{
				if (count > 1)
				{
					strBuilder.Append(count);
				}
				count = 0;
			}
		}

		return strBuilder.ToString();
	}

	public string Decompress(string compressedString)
	{
		if (String.IsNullOrEmpty(compressedString))
			throw new ArgumentException("Входящая строка не содержит символов");

		if (compressedString.Any(x => !char.IsAsciiLetterOrDigit(x)))
			throw new ArgumentException("Входящая строка может содержать только буквы латинского алфавита или цифры");

		if (!Regex.IsMatch(compressedString, @"^[a-zA-Z]+(?:(?:[1-9][0-9]*)[a-zA-Z]*)*(?:[1-9][0-9]*)*$"))
			throw new ArgumentException("Входящая строка имеет неверный формат и не может быть декодирована");

		var resBuilder = new StringBuilder();
		var numBuilder = new StringBuilder();

		char focusedChr = compressedString[0];

		for (int i = 0; i < compressedString.Length; i++)
		{
			var chr = compressedString[i];
			var isPatternEnder = i == compressedString.Length - 1 || !char.IsDigit(compressedString[i + 1]);

			if (char.IsDigit(chr))
			{
				numBuilder.Append(chr);

				if (isPatternEnder)
				{
					var repCount = numBuilder.Length > 0 ? int.Parse(numBuilder.ToString()) : 0;
					numBuilder.Clear();

					for (int j = 0; j < repCount; j++)
					{
						resBuilder.Append(focusedChr);
					}
				}
			}
			else
			{
				focusedChr = chr;

				if (isPatternEnder)
				{
					resBuilder.Append(focusedChr);
				}
			}
		}

		return resBuilder.ToString();
	}

	#endregion
}
