using System.Buffers.Binary;
using System.Text;

namespace Pogtan.Packet;

public class ReceivedPacket(ReadOnlyMemory<byte> data)
{
    private int offset;

    public byte Decode1()
    {
        byte value = data.Span[offset];
        offset += 1;
        return value;
    }

    public ushort Decode2()
    {
        ushort value = BinaryPrimitives.ReadUInt16BigEndian(data.Span.Slice(offset, 2));
        offset += 2;
        return value;
    }

    public uint Decode4()
    {
        uint value = BinaryPrimitives.ReadUInt32BigEndian(data.Span.Slice(offset, 4));
        offset += 4;
        return value;
    }

    public string DecodeStr()
    {
        ushort length = Decode2();
        string value = Encoding.ASCII.GetString(data.Span.Slice(offset, length));
        offset += length;
        return value;
    }
}
