using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace Pogtan.Packet;

public class SendPacket(int initialSize) : IDisposable
{
    private byte[] buffer = ArrayPool<byte>.Shared.Rent(initialSize);
    private int offset;

    public SendPacket(SendPacketType packetType, int initialSize = 0x40) : this(3, initialSize) =>
        Encode1((byte)packetType);

    public SendPacket(UdpPacketType packetType, int initialSize = 0x40) : this(5, initialSize) =>
        Encode1((byte)packetType); // TODO : extract SendPacket interface, create separate impl for UDP packets

    private SendPacket(int headerSize, int initialSize) : this(initialSize)
    {
        if (initialSize < headerSize)
        {
            throw new ArgumentException("send packet initial size too small");
        }

        offset = headerSize; // reserved space for header
    }

    public void Dispose() => ArrayPool<byte>.Shared.Return(buffer);

    public Memory<byte> GetData() => new(buffer, 0, offset);

    public void EnsureSize(int size)
    {
        if (buffer.Length - offset >= size)
        {
            return;
        }

        byte[] newBuffer = ArrayPool<byte>.Shared.Rent(Math.Max(buffer.Length * 2, buffer.Length + size));
        Buffer.BlockCopy(buffer, 0, newBuffer, 0, buffer.Length);
        Dispose();
        buffer = newBuffer;
    }

    public Memory<byte> GetBuffer() => new Memory<byte>(buffer)[..offset];

    public void Encode1(byte value)
    {
        EnsureSize(1);
        buffer[offset] = value;
        offset += 1;
    }

    public void Encode2(ushort value)
    {
        EnsureSize(2);
        BinaryPrimitives.WriteUInt16BigEndian(new Span<byte>(buffer, offset, 2), value);
        offset += 2;
    }

    public void Encode2(int value)
    {
        EnsureSize(2);
        BinaryPrimitives.WriteInt16BigEndian(new Span<byte>(buffer, offset, 2), (short)value);
        offset += 2;
    }

    public void Encode4(uint value)
    {
        EnsureSize(4);
        BinaryPrimitives.WriteUInt32BigEndian(new Span<byte>(buffer, offset, 4), value);
        offset += 4;
    }

    public void Encode4(int value)
    {
        EnsureSize(4);
        BinaryPrimitives.WriteInt32BigEndian(new Span<byte>(buffer, offset, 4), value);
        offset += 4;
    }

    public void EncodeBuffer(byte[] value)
    {
        EnsureSize(value.Length);
        Buffer.BlockCopy(value, 0, buffer, offset, value.Length);
        offset += value.Length;
    }

    public void EncodeStr(string value)
    {
        Encode2((ushort)value.Length);
        EncodeBuffer(Encoding.ASCII.GetBytes(value));
    }
}
