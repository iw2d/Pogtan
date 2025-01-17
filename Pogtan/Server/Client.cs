using System.Buffers;
using System.Buffers.Binary;
using System.Net.Sockets;
using Pogtan.Packet;
using Pogtan.Util;

namespace Pogtan.Server;

public class Client(Server server, TcpClient inner, int headerType) : IDisposable
{
    private readonly byte[] headerBuffer = new byte[3];
    public readonly int HeaderType = headerType;
    private readonly NetworkStream stream = inner.GetStream();

    public readonly List<User> Users = new();

    public void Dispose()
    {
        stream.Dispose();
        inner.Dispose();
    }

    public async Task Read()
    {
        // Read header
        await stream.ReadExactlyAsync(headerBuffer, 0, 3);
        int length = BinaryPrimitives.ReadUInt16BigEndian(new Span<byte>(headerBuffer, 1, 2));

        // Read packet
        byte[] buffer = ArrayPool<byte>.Shared.Rent(length);
        try
        {
            await stream.ReadExactlyAsync(buffer, 0, length);
            ReceivedPacket packet = new(new ReadOnlyMemory<byte>(buffer, 0, length));

            // Handle packet
            ReceivedPacketType packetType =
                (ReceivedPacketType)HeaderConverter.DecodeHeader(packet.Decode1(), headerType);
            Console.WriteLine(
                $"[TCP In]  | {Helper.FormatEnum(packetType)} | {Helper.ReadableByteArray(buffer, 0, length)}");
            await server.AddTask(() => server.HandlePacket(packetType, packet, this));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public void Write(SendPacket packet)
    {
        // Complete header
        Memory<byte> packetData = packet.GetData();
        packetData.Span[0] = 0;
        BinaryPrimitives.WriteUInt16BigEndian(packetData.Span[1..], (ushort)(packetData.Length - 3));

        // Write packet
        stream.Write(packetData.Span);
        SendPacketType packetType = (SendPacketType)packetData.Span[3];
        Console.WriteLine(
            $"[TCP Out] | {Helper.FormatEnum(packetType)} | {Helper.ReadableByteArray(packetData, 3, packetData.Length - 3)}");
    }
}
