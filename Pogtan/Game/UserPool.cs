using Pogtan.Packet;
using Pogtan.Server;

namespace Pogtan.Game;

public abstract class UserPool
{
    public readonly List<Client> Clients = new();

    public virtual void OnPacket(ReceivedPacketType packetType, ReceivedPacket packet, Client client)
    {
        switch (packetType)
        {
            case ReceivedPacketType.EstimateRttRequest:
                OnEstimateRttRequest(packet, client);
                break;
            default:
                OnUnhandledPacket(packetType, packet, client);
                break;
        }
    }

    protected void BroadcastPacket(SendPacket packet)
    {
        foreach (Client client in Clients)
        {
            client.Write(packet);
        }
    }

    private void OnEstimateRttRequest(ReceivedPacket packet, Client client)
    {
        // TODO
    }

    private void OnUnhandledPacket(ReceivedPacketType packetType, ReceivedPacket packet, Client client) =>
        Console.WriteLine($"Unhandled packet {packetType}");
}
