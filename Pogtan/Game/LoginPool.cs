using Pogtan.Packet;
using Pogtan.Server;

namespace Pogtan.Game;

public class LoginPool : UserPool
{
    private readonly List<ChannelInfo> channels = new() { new ChannelInfo(1, "TEST", ChannelType.FreeChannel) };

    public override void OnPacket(ReceivedPacketType packetType, ReceivedPacket packet, Client client)
    {
        switch (packetType)
        {
            case ReceivedPacketType.LoginRequest:
                OnLoginRequest(packet, client);
                break;
            case ReceivedPacketType.ChannelsRequest:
                OnChannelsRequest(packet, client);
                break;
            case ReceivedPacketType.ConfirmIdRequest:
                OnConfirmIdRequest(packet, client);
                break;
            case ReceivedPacketType.NewUserRequest:
                OnNewUserRequest(packet, client);
                break;
            case ReceivedPacketType.ConnectToServerRequest:
                OnConnectToServerRequest(packet, client);
                break;
            case ReceivedPacketType.CrashInfo:
                OnCrashInfo(packet, client);
                break;
            case ReceivedPacketType.OnlinePoll:
                OnOnlinePoll(packet, client);
                break;
            default:
                base.OnPacket(packetType, packet, client);
                break;
        }
    }

    private void OnLoginRequest(ReceivedPacket packet, Client client)
    {
        // CMainSystem::Login
        client.Users.Clear();
        int size = packet.Decode1();
        for (int i = 0; i < size; i++)
        {
            string id = packet.DecodeStr();
            string password = packet.DecodeStr();
            client.Users.Add(new User((uint)(1234 + i), id));
        }

        // CMenuStage::OnLoginResult
        using (SendPacket p = new(SendPacketType.LoginResult))
        {
            p.Encode1(0); // 0, 4 : success, 1 : unknown user or incorrect password, 2 : already logged in, 3 : error
            p.Encode1((byte)client.Users.Count);
            foreach (User user in client.Users)
            {
                p.Encode4(user.Sn);
                // CUser::SetUser
                p.EncodeStr(user.Id);
                p.Encode1(0);
                p.Encode2((ushort)user.Level);
                p.Encode4(0);
                // ~CUser::SetUser
                p.Encode4(0);
            }

            client.Write(p);
        }
    }

    private void OnChannelsRequest(ReceivedPacket packet, Client client)
    {
        // CMenuStage::OnOkClicked
        if (client.Users.Count == 0)
        {
            Console.WriteLine("Received ChannelsRequest before login");
            return;
        }

        // CFrontStage::OnChannelsInfo
        using (SendPacket p = new(SendPacketType.ChannelsInfo))
        {
            p.Encode4(channels.Count);
            foreach (ChannelInfo channel in channels)
            {
                p.EncodeStr(channel.Name);
                p.Encode4(0);
                p.Encode4(0);
                p.Encode2(0);
                p.Encode1((byte)channel.Type);
            }

            p.EncodeStr("");
            p.Encode2(0);
            p.Encode2(0);
            p.Encode2(0);

            p.Encode4(0); // int * (string, short)
            p.Encode4(0); // int * (byte, string) -> CFxMessage::InputMsg
            p.Encode2(0); // *(TSingleton<CGameMapInfoList>::ms_pInstance + 4)

            client.Write(p);
        }

        // CFrontStage::OnChannelsState
        using (SendPacket p = new(SendPacketType.ChannelsState))
        {
            p.Encode4(channels.Count);
            foreach (ChannelInfo channel in channels)
            {
                p.Encode4(0); // Users
            }

            client.Write(p);
        }
    }

    private void OnConfirmIdRequest(ReceivedPacket packet, Client client)
    {
        // CNewUserDlg::OnConfirmIdClicked
        string id = packet.DecodeStr();

        // CMenuStage::OnNewUserConfirmIdResult
        using (SendPacket p = new(SendPacketType.ConfirmIdResult))
        {
            p.Encode1(0); // 0 : success, 1 : already used, 2 : error
            client.Write(p);
        }
    }

    private void OnNewUserRequest(ReceivedPacket packet, Client client)
    {
        // CNewUserDlg::OnOkClicked
        packet.DecodeStr();
        packet.DecodeStr();
        packet.DecodeStr();
        packet.DecodeStr();
        packet.Decode1();
        packet.Decode1();
        packet.DecodeStr();
        packet.DecodeStr();
        packet.DecodeStr();
        packet.Decode1();

        // CMenuStage::OnNewIdResult
        using (SendPacket p = new(SendPacketType.NewUserResult))
        {
            p.Encode1(0); // 0 : success, 1 : already used, 2 : error
            client.Write(p);
        }
    }

    private void OnConnectToServerRequest(ReceivedPacket packet, Client client)
    {
        // CFrontStage::OnChannelSelected
        uint channelIndex = packet.Decode4();

        // CFrontStage::OnResConnectToSvr
        if (channels.Count > channelIndex)
        {
            ChannelInfo channelInfo = channels[(int)channelIndex];
            using (SendPacket p = new(SendPacketType.ConnectToServerResult))
            {
                p.Encode1(1); // 0 : cannot connect, 1 : success, 2 : cannot enter premium channel
                p.Encode4(channelInfo.Id);
                p.EncodeBuffer(ServerConfig.ServerHost);
                p.Encode2(ServerConfig.ChannelPort);
                client.Write(p); // TODO : submit migration from LoginServer -> GameServer
            }
        }
        else
        {
            using (SendPacket p = new(SendPacketType.ConnectToServerResult))
            {
                p.Encode1(0);
                client.Write(p);
            }
        }
    }

    private void OnCrashInfo(ReceivedPacket packet, Client client)
    {
        string crashInfo = packet.DecodeStr();
        Console.WriteLine($"Crash Info : {crashInfo}");
    }

    private void OnOnlinePoll(ReceivedPacket packet, Client client) => throw new NotImplementedException();
}
