using System.Net;
using Pogtan.Game;
using Pogtan.Packet;

namespace Pogtan.Server;

public class GameServer(int port) : Server(port)
{
    public readonly ChannelPool ChannelPool = new();

    public override void OnConnect(Client client) => ChannelPool.Clients.Add(client);

    public override void OnDisconnect(Client client) => ChannelPool.Clients.Remove(client);

    public override void HandlePacket(ReceivedPacketType packetType, ReceivedPacket packet, Client client) =>
        ChannelPool.OnPacket(packetType, packet, client);

    public override void HandlePacket(UdpPacketType packetType, ReceivedPacket packet, IPEndPoint endpoint) =>
        throw new NotImplementedException();
}
