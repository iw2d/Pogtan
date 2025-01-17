using Pogtan.Packet;
using Pogtan.Server;

namespace Pogtan.Game;

public abstract class ChatPool : UserPool
{
    public override void OnPacket(ReceivedPacketType packetType, ReceivedPacket packet, Client client)
    {
        switch (packetType)
        {
            case ReceivedPacketType.ChatMessage:
                OnChatMessage(packet, client);
                break;
            default:
                base.OnPacket(packetType, packet, client);
                break;
        }
    }

    private void OnChatMessage(ReceivedPacket packet, Client client)
    {
        // CChatStage::HandleChatMsg
        string message = packet.DecodeStr();
        string sender = string.Join(", ", client.Users.Select(user => user.Id));

        // CChatStage::OnChatMsg
        using (SendPacket p = new(SendPacketType.ChatMessage))
        {
            p.EncodeStr($"{sender} : {message}");
            BroadcastPacket(p);
        }
    }
}
