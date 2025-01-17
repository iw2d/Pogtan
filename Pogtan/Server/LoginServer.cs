using System.Net;
using Pogtan.Game;
using Pogtan.Packet;

namespace Pogtan.Server;

public class LoginServer(int port) : Server(port)
{
    public readonly LoginPool LoginPool = new();

    public override void OnConnect(Client client) => LoginPool.Clients.Add(client);

    public override void OnDisconnect(Client client) => LoginPool.Clients.Remove(client);

    public override void HandlePacket(ReceivedPacketType packetType, ReceivedPacket packet, Client client) =>
        LoginPool.OnPacket(packetType, packet, client);

    public override void HandlePacket(UdpPacketType packetType, ReceivedPacket packet, IPEndPoint endpoint)
    {
        if (packetType != UdpPacketType.CheckUdpComm)
        {
            Console.WriteLine($"[LoginServer] Unexpected UDP packet {packetType}");
            return;
        }

        using (SendPacket p = new(UdpPacketType.CheckUdpCommResult))
        {
            UdpAcceptor.Write(p, endpoint);
        }
    }
}
