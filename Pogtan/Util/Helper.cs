namespace Pogtan.Util;

public static class Helper
{
    public static string FormatEnum(Enum value) => string.Format($"{value.ToString()} (0x{value:X})");

    public static string ReadableByteArray(ReadOnlyMemory<byte> data, int start, int length)
    {
        string[] bytes = new string[length];
        for (int i = 0; i < length; i++)
        {
            bytes[i] = string.Format($"{data.Span[start + i]:X2}");
        }

        return string.Join(' ', bytes);
    }
}
