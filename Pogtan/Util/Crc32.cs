namespace Pogtan.Util;

public class Crc32
{
    private static readonly uint[] s_crcTable;

    static Crc32()
    {
        s_crcTable = new uint[256];
        for (uint i = 0; i < 256; i++)
        {
            uint crc = i << 24;
            for (uint j = 0; j < 8; j++)
            {
                if ((crc & 0x80000000) != 0)
                {
                    crc = (crc << 1) ^ 0x4C11DB7;
                }
                else
                {
                    crc <<= 1;
                }
            }

            s_crcTable[i] = crc;
        }
    }

    public static uint UpdateCrc(uint init, ReadOnlySpan<byte> data)
    {
        uint crc = init;
        for (int i = 0; i < data.Length; i++)
        {
            crc = s_crcTable[(data[i] & 0xFF) ^ (crc >> 24)] ^ (crc << 8);
        }

        return crc;
    }
}
