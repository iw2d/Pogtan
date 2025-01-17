using Pogtan.Packet;
using Pogtan.Server;

namespace Pogtan.Game;

public class ChannelPool : ChatPool
{
    public override void OnPacket(ReceivedPacketType packetType, ReceivedPacket packet, Client client)
    {
        switch (packetType)
        {
            case ReceivedPacketType.ConnectToChannelRequest:
                OnConnectToChannelRequest(packet, client);
                break;
            case ReceivedPacketType.DisconnectFromChannelRequest:
                OnDisconnectFromChannelRequest(packet, client);
                break;
            default:
                base.OnPacket(packetType, packet, client);
                break;
        }
    }

    private void OnConnectToChannelRequest(ReceivedPacket packet, Client client)
    {
        // CFrontStage::OnResConnectToSvr
        client.Users.Clear();
        int size = packet.Decode1();
        for (int i = 0; i < size; i++)
        {
            string id = packet.DecodeStr();
            uint sn = packet.Decode4();
            packet.Decode4(); // last int in CUser::SetUser
            client.Users.Add(new User(sn, id)); // TODO : proper migration from LoginServer -> GameServer
        }

        using (SendPacket p = new(SendPacketType.EnterChannel))
        {
            client.Write(p);
        }
    }

    private void OnDisconnectFromChannelRequest(ReceivedPacket packet, Client client)
    {
        using (SendPacket p = new(SendPacketType.EnterChannel))
        {
            client.Write(p);
        }
    }
}
