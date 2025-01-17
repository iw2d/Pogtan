using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using Pogtan.Packet;
using Pogtan.Util;

namespace Pogtan.Server;

public class UdpAcceptor(Server server, int port) : IDisposable
{
    private readonly UdpClient client = new(port);

    public void Dispose() => client.Dispose();

    public async Task Start()
    {
        Console.WriteLine($"UdpAcceptor listening on port : {port}");
        try
        {
            while (true)
            {
                UdpReceiveResult result = await client.ReceiveAsync();

                // Decode packet
                ReceivedPacket packet = new(result.Buffer);
                packet.Decode1(); // size
                packet.Decode4(); // crc

                // Handle packet
                UdpPacketType packetType = (UdpPacketType)packet.Decode1();
                Console.WriteLine(
                    $"[UDP In]  | {Helper.FormatEnum(packetType)} | {Helper.ReadableByteArray(result.Buffer, 5, result.Buffer.Length - 5 - 4)}");
                await server.AddTask(() => server.HandlePacket(packetType, packet, result.RemoteEndPoint));
            }
        }
        finally
        {
            client.Close();
        }
    }

    public void Write(SendPacket packet, IPEndPoint endPoint)
    {
        // Complete header
        Memory<byte> packetData = packet.GetData();
        packetData.Span[0] = (byte)packetData.Length;
        BinaryPrimitives.WriteUInt32BigEndian(packetData.Span[1..], 0);
        // Encode crc and send packet
        packet.Encode4(Crc32.UpdateCrc(0, packetData.Span));
        client.Send(packet.GetData().Span, endPoint);

        UdpPacketType packetType = (UdpPacketType)packetData.Span[5];
        Console.WriteLine(
            $"[UDP Out] | {Helper.FormatEnum(packetType)} | {Helper.ReadableByteArray(packetData, 5, packetData.Length - 5)}");
    }
}
